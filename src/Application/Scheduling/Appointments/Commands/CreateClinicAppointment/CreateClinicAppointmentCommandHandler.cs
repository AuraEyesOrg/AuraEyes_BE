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
    private readonly IPatientVisitRepository _patientVisitRepository;
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
        IPatientVisitRepository patientVisitRepository,
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
        _patientVisitRepository = patientVisitRepository;
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
        if (!_currentUser.UserId.HasValue)
        {
            return Result<CreateClinicAppointmentResult>.Forbidden(
                "Unable to resolve user identity for payment operation.");
        }

        // ── 0. Pre-transaction checks & Role resolution ──────────────────
        var isStaffBooking = await _identityService.IsInRoleAsync(_currentUser.UserId.Value, Roles.ClinicStaff) ||
                             await _identityService.IsInRoleAsync(_currentUser.UserId.Value, Roles.SystemAdmin);

        if (request.PatientId.HasValue && !isStaffBooking)
        {
            return Result<CreateClinicAppointmentResult>.Forbidden("Only clinic staff can book for another patient.");
        }

        Guid targetPatientProfileId;
        Guid orderUserId;
        Guid? notificationUserId;
        bool isStaffCreatedWalkIn = false;

        if (request.PatientId.HasValue)
        {
            // Staff booking for another patient
            var targetPatient = await _patientRepository.GetByIdAsync(request.PatientId.Value, cancellationToken);
            if (targetPatient == null)
            {
                return Result<CreateClinicAppointmentResult>.NotFound($"Patient profile '{request.PatientId}' not found.");
            }
            targetPatientProfileId = targetPatient.Id;
            orderUserId = targetPatient.UserId ?? _currentUser.UserId.Value;
            notificationUserId = targetPatient.UserId;
            isStaffCreatedWalkIn = true;
        }
        else
        {
            // Self-booking
            if (_currentUser.ProfileId is null)
            {
                return Result<CreateClinicAppointmentResult>.Unauthorized("Patient profile is required for self-booking.");
            }
            targetPatientProfileId = _currentUser.ProfileId.Value;
            orderUserId = _currentUser.UserId.Value;
            notificationUserId = _currentUser.UserId.Value;
        }

        // ── 1. Start Transaction ──────────────────────────────────────────
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {

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
            Guid? finalDoctorId = request.RequestedDoctorId;
            var finalPricingType = request.PricingType;

            // Auto-detect doctor if the slot is owned by one (Doctor-centric model)
            if (slot.OphthalId.HasValue && finalDoctorId == null)
            {
                finalDoctorId = slot.OphthalId;
                finalPricingType = PricingType.DoctorSelected;
            }

            if (finalPricingType == PricingType.DoctorSelected)
            {
                if (finalDoctorId == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.Failure("Doctor must be selected for DoctorSelected pricing type.");
                }

                var doctor = await _ophthalmologistRepository.GetByIdAsync(finalDoctorId.Value, cancellationToken);
                if (doctor == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.NotFound($"Doctor '{finalDoctorId}' not found.");
                }

                // If the slot has a specific OphthalId, verify it matches
                if (slot.OphthalId.HasValue && slot.OphthalId != finalDoctorId)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.Failure("The selected doctor does not match the doctor assigned to this slot.");
                }

                // If slot doesn't have OphthalId but we have assignments (shared slot model), verify assignment
                if (!slot.OphthalId.HasValue)
                {
                    var isAssigned = await _slotAssignmentRepository.HasAssignmentAsync(
                        request.SlotId,
                        finalDoctorId.Value,
                        SlotAssignmentRole.Doctor,
                        cancellationToken);

                    if (!isAssigned)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return Result<CreateClinicAppointmentResult>.Failure("The selected doctor is not available for this appointment slot.");
                    }
                }

                price = doctor.ConsultationFee;
            }

            // ── Apply discount if patient has one ──────────────────────────────
            var discountPatient = await _patientRepository.GetByIdAsync(targetPatientProfileId, cancellationToken);
            if (discountPatient != null)
            {
                var discount = discountPatient.ConsumeDiscount();
                if (discount.HasValue)
                {
                    price = Math.Round(price * (1 - discount.Value), 0);
                }
            }

            // ── 5. Calculate deposit ──────────────────────────────────────────
            // If it's a walk-in (staff booking), there is NO deposit (they pay 100% full amount).
            // For online bookings, we take 30% deposit.
            decimal? depositAmount = isStaffCreatedWalkIn ? null : Math.Round(price * DepositRatio, 0);
            if (depositAmount.HasValue && depositAmount.Value < 1) depositAmount = 1;

            // ── 6. Create Appointment ─────────────────────────────────────────
            slot.BookWithCapacity();

            var appointment = new Appointment(
                targetPatientProfileId,
                request.SlotId,
                price,
                finalPricingType,
                finalDoctorId,
                request.VisitReason);

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);

            // ── 7. Create Order + Payment ─────────────────────────────────────
            var typeLabel = isStaffCreatedWalkIn ? "Thanh toán đủ" : "Đặt cọc";
            var orderDescription = $"{typeLabel} khám {slot.Date:dd/MM} {slot.StartTime:HH:mm}";
            
            var order = new Order(
                orderUserId,
                price,
                depositAmount,
                orderDescription,
                appointment.Id);

            await _orderRepository.AddAsync(order, cancellationToken);

            Payment? payment = null;
            if (!isStaffCreatedWalkIn)
            {
                // Create skeleton payment, will call PayOS AFTER commit
                payment = new Payment(order.Id, depositAmount!.Value, PaymentMethod.PayOS, orderDescription);
                await _paymentRepository.AddAsync(payment, cancellationToken);
            }
            else
            {
                // For walk-ins, create a "Pending" Cash payment
                payment = new Payment(order.Id, price, PaymentMethod.Cash, orderDescription);
                await _paymentRepository.AddAsync(payment, cancellationToken);
            }

            PatientVisit? visit = null;
            if (isStaffCreatedWalkIn)
            {
                visit = PatientVisit.CreateFromAppointment(appointment);
                await _patientVisitRepository.AddAsync(visit, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // ── 8. External API calls (AFTER commit to avoid holding locks) ──
            string? paymentUrl = null;
            if (!isStaffCreatedWalkIn && payment != null)
            {
                try 
                {
                    var returnUrl = _configuration["PayOS:DefaultReturnUrl"] ?? "";
                    var cancelUrl = _configuration["PayOS:DefaultCancelUrl"] ?? "";
                    var separator = returnUrl.Contains("?") ? "&" : "?";
                    var queryParams = $"orderId={order.Id}&appointmentId={appointment.Id}&type=clinic-booking";
                    returnUrl = $"{returnUrl}{separator}{queryParams}";
                    var cancelSeparator = cancelUrl.Contains("?") ? "&" : "?";
                    cancelUrl = $"{cancelUrl}{cancelSeparator}{queryParams}&cancel=true";

                    var (pUrl, orderCode) = await _payOSService.CreatePaymentLinkAsync(
                        payment.Id,
                        depositAmount!.Value,
                        orderDescription,
                        returnUrl,
                        cancelUrl);

                    // Update payment with the link in a separate small transaction or just save
                    payment.SetPaymentLink(pUrl, orderCode);
                    await _paymentRepository.UpdateAsync(payment, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    paymentUrl = pUrl;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create PayOS payment link for order {OrderId}. User will need to retry from history.", order.Id);
                    // We don't fail the whole booking because the slot is already booked.
                }
            }

            _logger.LogInformation(
                "Created {Type} clinic appointment {AppointmentId} for patient {PatientId} at slot {SlotId}. " +
                "FullPrice: {FullPrice} VND, Deposit: {DepositAmount} VND. Order {OrderId}",
                isStaffCreatedWalkIn ? "Walk-in" : "Online",
                appointment.Id,
                targetPatientProfileId,
                request.SlotId,
                price,
                depositAmount ?? price,
                order.Id);

            // ── 8. Post-commit: notifications ──────────────────────────
            await SendBookingNotificationAsync(appointment, notificationUserId, slot, request.VisitReason, isStaffCreatedWalkIn, cancellationToken);

            if (visit is not null)
            {
                await SendQueueNotificationAsync(appointment, visit, cancellationToken);
            }

            return Result<CreateClinicAppointmentResult>.Success(new CreateClinicAppointmentResult
            {
                AppointmentId = appointment.Id,
                VisitId = visit?.Id,
                Status = visit?.Status.ToString() ?? appointment.Status.ToString(),
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
}
