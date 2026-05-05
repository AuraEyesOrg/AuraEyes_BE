using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Application.Common.Helpers;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Application.Common.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Application.Scheduling.Appointments.Commands.CreateClinicAppointment;

public class CreateClinicAppointmentCommandHandler
    : ICommandHandler<CreateClinicAppointmentCommand, CreateClinicAppointmentResult>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;
    private readonly ISlotAssignmentRepository _slotAssignmentRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IPayOSService _payOSService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateClinicAppointmentCommandHandler> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Deposit ratio: patient pays 30% of the full slot price upfront via PayOS.
    /// </summary>
    private const decimal DepositRatio = 0.30m;

    private const decimal BASE_CLINIC_PRICE = 50000m; // Base clinic price for auto-assign

    public CreateClinicAppointmentCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IAppointmentRepository appointmentRepository,
        IPatientVisitRepository patientVisitRepository,
        IRepository<Patient> patientRepository,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        ICurrentUserService currentUser,
        IIdentityService identityService,
        INotificationService notificationService,
        ISlotAssignmentRepository slotAssignmentRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IPayOSService payOSService,
        IUnitOfWork unitOfWork,
        ILogger<CreateClinicAppointmentCommandHandler> logger,
        IConfiguration configuration)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _appointmentRepository = appointmentRepository;
        _patientVisitRepository = patientVisitRepository;
        _patientRepository = patientRepository;
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _currentUser = currentUser;
        _identityService = identityService;
        _notificationService = notificationService;
        _slotAssignmentRepository = slotAssignmentRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _payOSService = payOSService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<Result<CreateClinicAppointmentResult>> Handle(CreateClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue) return Result<CreateClinicAppointmentResult>.Forbidden("Unable to resolve user identity for payment operation.");

        var roleResolution = await ResolveBookingRoleAsync(request);
        if (!roleResolution.IsSuccess) return Result<CreateClinicAppointmentResult>.Failure(roleResolution.ErrorMessage);

        var patientResolution = await ResolveTargetPatientAsync(request, cancellationToken);
        if (!patientResolution.IsSuccess) return Result<CreateClinicAppointmentResult>.Failure(patientResolution.ErrorMessage);

        var patientData = patientResolution.Data;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await ProcessBookingTransactionAsync(request, patientData, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result<CreateClinicAppointmentResult>.Success(result);
        }
        catch (Domain.Common.ConcurrencyException)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<CreateClinicAppointmentResult>.Conflict("Slot was updated by another request. Please retry.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            if (ex is InvalidOperationException) return Result<CreateClinicAppointmentResult>.Failure(ex.Message);
            throw;
        }
    }

    private async Task<Result> ResolveBookingRoleAsync(CreateClinicAppointmentCommand request)
    {
        var isStaffBooking = await _identityService.IsInRoleAsync(_currentUser.UserId!.Value, Roles.ClinicStaff) ||
                             await _identityService.IsInRoleAsync(_currentUser.UserId.Value, Roles.SystemAdmin);

        if (request.PatientId.HasValue && !isStaffBooking)
            return Result.Failure("Only clinic staff can book for another patient.");

        return Result.Success();
    }

    private async Task<CreateClinicAppointmentResult> ProcessBookingTransactionAsync(
        CreateClinicAppointmentCommand request,
        (Guid ProfileId, Guid OrderUserId, Guid? NotifyUserId, bool IsWalkIn) patientData,
        CancellationToken cancellationToken)
    {
        var slotValidation = await ValidateSlotAndBookingAsync(request.SlotId, patientData.ProfileId, cancellationToken);
        if (!slotValidation.IsSuccess) throw new InvalidOperationException(slotValidation.ErrorMessage);
        var slot = slotValidation.Data;

        var pricingResult = await ResolvePricingAsync(request, slot, patientData.ProfileId, cancellationToken);
        if (!pricingResult.IsSuccess) throw new InvalidOperationException(pricingResult.ErrorMessage);
        var (price, finalDoctorId, finalPricingType) = pricingResult.Data;

        decimal? depositAmount = patientData.IsWalkIn ? price : Math.Max(1, Math.Round(price * DepositRatio, 0));

        slot.BookWithCapacity();
        var appointment = new Appointment(patientData.ProfileId, request.SlotId, price, finalPricingType, finalDoctorId, request.VisitReason);
        await _appointmentRepository.AddAsync(appointment, cancellationToken);
        await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);

        var order = await CreateOrderAndPaymentAsync(appointment, patientData, price, depositAmount, slot, cancellationToken);
        // var visit = await CreateVisitIfWalkInAsync(appointment, patientData.IsWalkIn, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        string? paymentUrl = null;
        if (!patientData.IsWalkIn)
        {
            var payments = await _paymentRepository.GetByOrderIdAsync(order.Id, cancellationToken);
            var payment = payments.FirstOrDefault();
            paymentUrl = await TryCreatePaymentLinkAsync(payment!, order, appointment, depositAmount!.Value, order.Description, cancellationToken);
        }

        _logger.LogInformation("Created {Type} clinic appointment {AppointmentId} for patient {PatientId}. Order {OrderId}",
            patientData.IsWalkIn ? "Walk-in" : "Online", appointment.Id, patientData.ProfileId, order.Id);

        await SendBookingNotificationAsync(appointment, patientData.NotifyUserId, slot, request.VisitReason, patientData.IsWalkIn, cancellationToken);
        // if (visit != null) await SendQueueNotificationAsync(appointment, visit, cancellationToken);

        return new CreateClinicAppointmentResult
        {
            AppointmentId = appointment.Id,
            VisitId = null,
            Status = appointment.Status.ToString(),
            PaymentUrl = paymentUrl,
            OrderId = order.Id,
            DepositAmount = depositAmount,
        };
    }

    private async Task<Order> CreateOrderAndPaymentAsync(Appointment appointment, (Guid ProfileId, Guid OrderUserId, Guid? NotifyUserId, bool IsWalkIn) patientData, decimal price, decimal? depositAmount, AppointmentSlot slot, CancellationToken cancellationToken)
    {
        var typeLabel = patientData.IsWalkIn ? "Thanh toán đủ" : "Đặt cọc";
        var orderDescription = $"{typeLabel} khám {slot.Date:dd/MM} {slot.StartTime:HH:mm}";
        var order = new Order(patientData.OrderUserId, price, depositAmount, orderDescription, appointment.Id);
        await _orderRepository.AddAsync(order, cancellationToken);

        Payment payment = patientData.IsWalkIn 
            ? new Payment(order.Id, price, PaymentMethod.Cash, orderDescription)
            : new Payment(order.Id, depositAmount!.Value, PaymentMethod.PayOS, orderDescription);
        await _paymentRepository.AddAsync(payment, cancellationToken);
        
        return order;
    }

    private async Task<PatientVisit?> CreateVisitIfWalkInAsync(Appointment appointment, bool isWalkIn, CancellationToken cancellationToken)
    {
        if (!isWalkIn) return null;
        var visit = PatientVisit.CreateFromAppointment(appointment);
        await _patientVisitRepository.AddAsync(visit, cancellationToken);
        return visit;
    }

    private async Task<Result<(Guid ProfileId, Guid OrderUserId, Guid? NotifyUserId, bool IsWalkIn)>> ResolveTargetPatientAsync(
        CreateClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (request.PatientId.HasValue)
        {
            var targetPatient = await _patientRepository.GetByIdAsync(request.PatientId.Value, cancellationToken);
            if (targetPatient == null) return Result<(Guid, Guid, Guid?, bool)>.NotFound($"Patient profile '{request.PatientId}' not found.");
            return Result<(Guid, Guid, Guid?, bool)>.Success((targetPatient.Id, targetPatient.UserId ?? _currentUser.UserId!.Value, targetPatient.UserId, true));
        }

        if (_currentUser.ProfileId is null) return Result<(Guid, Guid, Guid?, bool)>.Unauthorized("Patient profile is required for self-booking.");
        return Result<(Guid, Guid, Guid?, bool)>.Success((_currentUser.ProfileId.Value, _currentUser.UserId!.Value, _currentUser.UserId.Value, false));
    }

    private async Task<Result<AppointmentSlot>> ValidateSlotAndBookingAsync(Guid slotId, Guid patientProfileId, CancellationToken cancellationToken)
    {
        var slot = await _appointmentSlotRepository.GetByIdWithLockAsync(slotId, cancellationToken);
        if (slot is null) return Result<AppointmentSlot>.NotFound($"Appointment slot '{slotId}' not found.");
        if (slot.Status != ScheduleStatus.Available) return Result<AppointmentSlot>.Failure($"Appointment slot is not available. Current status: {slot.Status}.");
        if (slot.BookedCount >= slot.MaxCapacity) return Result<AppointmentSlot>.Conflict("This appointment slot is fully booked.");

        var hasExisting = await _appointmentRepository.HasExistingAppointmentAsync(patientProfileId, slotId, cancellationToken);
        if (hasExisting) return Result<AppointmentSlot>.Conflict("Patient already has an appointment for this slot.");

        // Validate advance booking time (minimum 30 minutes notice)
        var utcNow = DateTime.UtcNow;
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, VietnamTimeZoneResolver.TimeZone);
        var slotLocalDateTime = slot.Date.ToDateTime(slot.StartTime);
        if (slotLocalDateTime < localNow.AddMinutes(30))
        {
            return Result<AppointmentSlot>.Failure("Appointments must be booked at least 30 minutes in advance. Please select a later time slot.");
        }

        return Result<AppointmentSlot>.Success(slot);
    }

    private async Task<string?> TryCreatePaymentLinkAsync(Payment payment, Order order, Appointment appointment, decimal depositAmount, string description, CancellationToken cancellationToken)
    {
        try
        {
            var returnUrl = _configuration["PayOS:DefaultReturnUrl"] ?? "";
            var cancelUrl = _configuration["PayOS:DefaultCancelUrl"] ?? "";
            var queryParams = $"orderId={order.Id}&appointmentId={appointment.Id}&type=clinic-booking";
            
            returnUrl = $"{returnUrl}{(returnUrl.Contains("?") ? "&" : "?")}{queryParams}";
            cancelUrl = $"{cancelUrl}{(cancelUrl.Contains("?") ? "&" : "?")}{queryParams}&cancel=true";

            var (pUrl, orderCode) = await _payOSService.CreatePaymentLinkAsync(payment.Id, depositAmount, description, returnUrl, cancelUrl);

            payment.SetPaymentLink(pUrl, orderCode);
            await _paymentRepository.UpdateAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return pUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create PayOS payment link for order {OrderId}.", order.Id);
            return null;
        }
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task SendBookingNotificationAsync(
        Appointment appointment,
        Guid? targetUserId,
        AppointmentSlot slot,
        string? visitReason,
        bool isWalkIn,
        CancellationToken cancellationToken)
    {
        if (!targetUserId.HasValue)
        {
            return;
        }

        try
        {
            var title = isWalkIn ? "Đặt lịch khám trực tiếp thành công" : "Đặt lịch khám thành công";
            var body = isWalkIn 
                ? $"Bạn đã được đặt lịch khám trực tiếp vào lúc {slot.StartTime:HH:mm}, ngày {slot.Date:dd/MM/yyyy}."
                : $"Bạn đã đặt lịch thành công vào lúc {slot.StartTime:HH:mm}, ngày {slot.Date:dd/MM/yyyy}. Vui lòng hoàn tất thanh toán đặt cọc.";

            await _notificationService.SendAsync(
                targetUserId.Value,
                title,
                body,
                NotificationType.NewAppointmentBooked,
                new
                {
                    AppointmentId = appointment.Id,
                    AppointmentTime = $"{slot.Date:yyyy-MM-dd}T{slot.StartTime:HH:mm}:00",
                    Reason = visitReason
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send appointment booking notification for appointment {AppointmentId}",
                appointment.Id);
        }
    }

    private async Task SendQueueNotificationAsync(
        Appointment appointment,
        PatientVisit visit,
        CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.SendToRoleAsync(
                roleName: Roles.ClinicStaff,
                title: "New Patient in Queue",
                message: $"Patient {appointment.Patient?.FullName ?? "Unknown"} has been added to the clinic queue.",
                type: NotificationType.SystemAlert,
                payload: new { VisitId = visit.Id, PatientId = visit.PatientId },
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send walk-in queue notification for visit {VisitId}",
                visit.Id);
        }
    }

    private async Task<Result<(decimal Price, Guid? FinalDoctorId, PricingType FinalPricingType)>> ResolvePricingAsync(
        CreateClinicAppointmentCommand request,
        AppointmentSlot slot,
        Guid targetPatientProfileId,
        CancellationToken cancellationToken)
    {
        decimal price = BASE_CLINIC_PRICE;
        Guid? finalDoctorId = request.RequestedDoctorId;
        var finalPricingType = request.PricingType;

        if (slot.OphthalId.HasValue && finalDoctorId == null)
        {
            finalDoctorId = slot.OphthalId;
            finalPricingType = PricingType.DoctorSelected;
        }

        if (finalPricingType == PricingType.DoctorSelected)
        {
            if (finalDoctorId == null)
            {
                return Result<(decimal, Guid?, PricingType)>.Failure("Doctor must be selected for DoctorSelected pricing type.");
            }

            var doctor = await _ophthalmologistRepository.GetByIdAsync(finalDoctorId.Value, cancellationToken);
            if (doctor == null)
            {
                return Result<(decimal, Guid?, PricingType)>.NotFound($"Doctor '{finalDoctorId}' not found.");
            }

            if (slot.OphthalId.HasValue && slot.OphthalId != finalDoctorId)
            {
                return Result<(decimal, Guid?, PricingType)>.Failure("The selected doctor does not match the doctor assigned to this slot.");
            }

            if (!slot.OphthalId.HasValue)
            {
                var isAssigned = await _slotAssignmentRepository.HasAssignmentAsync(
                    request.SlotId,
                    finalDoctorId.Value,
                    SlotAssignmentRole.Doctor,
                    cancellationToken);

                if (!isAssigned)
                {
                    return Result<(decimal, Guid?, PricingType)>.Failure("The selected doctor is not available for this appointment slot.");
                }
            }

            price = doctor.ConsultationFee;
        }

        var discountPatient = await _patientRepository.GetByIdAsync(targetPatientProfileId, cancellationToken);
        if (discountPatient != null)
        {
            var discount = discountPatient.ConsumeDiscount();
            if (discount.HasValue)
            {
                price = Math.Round(price * (1 - discount.Value), 0);
            }
        }

        return Result<(decimal, Guid?, PricingType)>.Success((price, finalDoctorId, finalPricingType));
    }
}
