using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
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
    private readonly IRepository<Patient> _patientRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
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
        IRepository<Patient> patientRepository,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        ICurrentUserService currentUser,
        IIdentityService identityService,
        IEmailService emailService,
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
        _patientRepository = patientRepository;
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _currentUser = currentUser;
        _identityService = identityService;
        _emailService = emailService;
        _notificationService = notificationService;
        _slotAssignmentRepository = slotAssignmentRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _payOSService = payOSService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<Result<CreateClinicAppointmentResult>> Handle(
        CreateClinicAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId is null)
        {
            return Result<CreateClinicAppointmentResult>.Unauthorized("Patient profile is required.");
        }

        if (!_currentUser.UserId.HasValue)
        {
            return Result<CreateClinicAppointmentResult>.Forbidden(
                "Unable to resolve user identity for payment operation.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // ── 1. Resolve Patient ──────────────────────────────────────────
            Guid targetPatientProfileId;
            Guid targetUserId;
            bool isWalkIn = false;

            if (request.PatientId.HasValue)
            {
                // Staff booking for another patient
                var patient = await _patientRepository.GetByIdAsync(request.PatientId.Value, cancellationToken);
                if (patient == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.NotFound($"Patient profile '{request.PatientId}' not found.");
                }
                targetPatientProfileId = patient.Id;
                targetUserId = patient.UserId ?? Guid.Empty; // Should have a UserId from CreateUserWalkInPatientAsync
                
                // Check if current user is staff/admin
                isWalkIn = await _identityService.IsInRoleAsync(_currentUser.UserId.Value, Roles.ClinicStaff) ||
                           await _identityService.IsInRoleAsync(_currentUser.UserId.Value, Roles.SystemAdmin);
            }
            else
            {
                // Self-booking
                if (_currentUser.ProfileId is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.Unauthorized("Patient profile is required for self-booking.");
                }
                targetPatientProfileId = _currentUser.ProfileId.Value;
                targetUserId = _currentUser.UserId.Value;
            }

            // ── 2. Validate slot ──────────────────────────────────────────────
            var slot = await _appointmentSlotRepository.GetByIdWithLockAsync(request.SlotId, cancellationToken);
            if (slot is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CreateClinicAppointmentResult>.NotFound(
                    $"Appointment slot '{request.SlotId}' not found.");
            }

            if (slot.Status != ScheduleStatus.Available)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CreateClinicAppointmentResult>.Failure(
                    $"Appointment slot is not available. Current status: {slot.Status}.");
            }

            if (slot.BookedCount >= slot.MaxCapacity)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CreateClinicAppointmentResult>.Conflict("This appointment slot is fully booked.");
            }

            // ── 3. Duplicate booking check ────────────────────────────────────
            var hasExistingAppointment = await _appointmentRepository.HasExistingAppointmentAsync(
                targetPatientProfileId,
                request.SlotId,
                cancellationToken);

            if (hasExistingAppointment)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CreateClinicAppointmentResult>.Conflict("Patient already has an appointment for this slot.");
            }

            // ── 4. Resolve pricing ────────────────────────────────────────────
            decimal price = BASE_CLINIC_PRICE;

            if (request.PricingType == PricingType.DoctorSelected)
            {
                if (request.RequestedDoctorId == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.Failure("Doctor must be selected for DoctorSelected pricing type.");
                }

                var doctor = await _ophthalmologistRepository.GetByIdAsync(request.RequestedDoctorId.Value, cancellationToken);
                if (doctor == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.NotFound($"Doctor '{request.RequestedDoctorId}' not found.");
                }

                var isAssigned = await _slotAssignmentRepository.HasAssignmentAsync(
                    request.SlotId,
                    request.RequestedDoctorId.Value,
                    SlotAssignmentRole.Doctor,
                    cancellationToken);

                if (!isAssigned)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.Failure("The selected doctor is not available for this appointment slot.");
                }

                price = doctor.ConsultationFee;
            }

            // ── 5. Calculate deposit ──────────────────────────────────────────
            // If it's a walk-in (staff booking), there is NO deposit (they pay 100% full amount).
            // For online bookings, we take 30% deposit.
            decimal? depositAmount = isWalkIn ? null : Math.Round(price * DepositRatio, 0);
            if (depositAmount.HasValue && depositAmount.Value < 1) depositAmount = 1;

            // ── 6. Create Appointment ─────────────────────────────────────────
            slot.BookWithCapacity();

            var appointment = new Appointment(
                targetPatientProfileId,
                request.SlotId,
                price,
                request.PricingType,
                request.RequestedDoctorId,
                request.VisitReason);

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);

            // ── 7. Create Order + Payment ─────────────────────────────────────
            var typeLabel = isWalkIn ? "Thanh toán đủ" : "Đặt cọc";
            var orderDescription = $"{typeLabel} khám {slot.Date:dd/MM} {slot.StartTime:HH:mm}";
            
            var order = new Order(
                targetUserId,
                price,
                depositAmount,
                orderDescription,
                appointment.Id);

            await _orderRepository.AddAsync(order, cancellationToken);

            string? paymentUrl = null;
            if (!isWalkIn)
            {
                // Create PayOS payment link for online deposit
                var payment = new Payment(order.Id, depositAmount!.Value, PaymentMethod.PayOS, orderDescription);
                await _paymentRepository.AddAsync(payment, cancellationToken);

                var defaultReturnUrl = _configuration["PayOS:DefaultReturnUrl"] ?? "http://localhost:3000";
                var uri = new Uri(defaultReturnUrl);
                var baseUrl = $"{uri.Scheme}://{uri.Authority}";
                
                var returnUrl = $"{baseUrl}/patient/wallet/payment-callback" +
                    $"?type=clinic-booking&orderId={order.Id}&appointmentId={appointment.Id}";
                var cancelUrl = $"{baseUrl}/patient/wallet/payment-callback" +
                    $"?type=clinic-booking&orderId={order.Id}&appointmentId={appointment.Id}&cancel=true";

                var (pUrl, orderCode) = await _payOSService.CreatePaymentLinkAsync(
                    payment.Id,
                    depositAmount.Value,
                    orderDescription,
                    returnUrl,
                    cancelUrl);

                payment.SetPaymentLink(pUrl, orderCode);
                paymentUrl = pUrl;
            }
            else
            {
                // For walk-ins, we can create a "Pending" Cash payment or just leave it for staff to collect
                // Let's create a pending Cash payment for the full amount
                var payment = new Payment(order.Id, price, PaymentMethod.Cash, orderDescription);
                await _paymentRepository.AddAsync(payment, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Created {Type} clinic appointment {AppointmentId} for patient {PatientId} at slot {SlotId}. " +
                "FullPrice: {FullPrice} VND, Deposit: {DepositAmount} VND. Order {OrderId}",
                isWalkIn ? "Walk-in" : "Online",
                appointment.Id,
                targetPatientProfileId,
                request.SlotId,
                price,
                depositAmount ?? price,
                order.Id);

            // ── 8. Post-commit: notifications ──────────────────────────
            await SendBookingNotificationAsync(appointment, targetUserId, slot, request.VisitReason, isWalkIn, cancellationToken);

            return Result<CreateClinicAppointmentResult>.Success(new CreateClinicAppointmentResult
            {
                AppointmentId = appointment.Id,
                Status = appointment.Status,
                PaymentUrl = paymentUrl,
                OrderId = order.Id,
                DepositAmount = depositAmount,
            });
        }
        catch (Domain.Common.ConcurrencyException)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<CreateClinicAppointmentResult>.Conflict(
                "Slot was updated by another request. Please retry.");
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<CreateClinicAppointmentResult>.Failure(ex.Message);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task SendBookingNotificationAsync(
        Appointment appointment,
        Guid targetUserId,
        AppointmentSlot slot,
        string? visitReason,
        bool isWalkIn,
        CancellationToken cancellationToken)
    {
        try
        {
            var title = isWalkIn ? "Đặt lịch khám trực tiếp thành công" : "Đặt lịch khám thành công";
            var body = isWalkIn 
                ? $"Bạn đã được đặt lịch khám trực tiếp vào lúc {slot.StartTime:HH:mm}, ngày {slot.Date:dd/MM/yyyy}."
                : $"Bạn đã đặt lịch thành công vào lúc {slot.StartTime:HH:mm}, ngày {slot.Date:dd/MM/yyyy}. Vui lòng hoàn tất thanh toán đặt cọc.";

            await _notificationService.SendAsync(
                targetUserId,
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
}
