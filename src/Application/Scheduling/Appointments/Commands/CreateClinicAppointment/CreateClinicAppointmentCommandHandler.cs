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
            // ── 1. Validate slot ──────────────────────────────────────────────
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

            // ── 2. Duplicate booking check ────────────────────────────────────
            var patientId = _currentUser.ProfileId.Value;
            var hasExistingAppointment = await _appointmentRepository.HasExistingAppointmentAsync(
                patientId,
                request.SlotId,
                cancellationToken);

            if (hasExistingAppointment)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CreateClinicAppointmentResult>.Conflict("You already have an appointment for this slot.");
            }

            // ── 3. Resolve pricing ────────────────────────────────────────────
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

            // ── 4. Calculate 30% deposit ──────────────────────────────────────
            var depositAmount = Math.Round(price * DepositRatio, 0);
            if (depositAmount < 1) depositAmount = 1; // PayOS minimum 1 VND

            // ── 5. Create Appointment ─────────────────────────────────────────
            slot.BookWithCapacity();

            var appointment = new Appointment(
                patientId,
                request.SlotId,
                price,
                request.PricingType,
                request.RequestedDoctorId,
                request.VisitReason);

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);

            // ── 6. Create Order + Payment for deposit ─────────────────────────
            // We append the AppointmentId to the description so the webhook can extract it
            // and send the confirmation email upon successful payment.
            var orderDescription = $"Đặt cọc khám {slot.Date:dd/MM} {slot.StartTime:HH:mm} [Appt:{appointment.Id}]";
            var order = new Order(
                _currentUser.UserId.Value,
                depositAmount,
                orderDescription);

            await _orderRepository.AddAsync(order, cancellationToken);

            var payment = new Payment(order.Id, depositAmount, PaymentMethod.PayOS, orderDescription);
            await _paymentRepository.AddAsync(payment, cancellationToken);

            // ── 7. Generate PayOS checkout link ───────────────────────────────
            // Extract the base URL from the PayOS settings so it's fully configurable via appsettings.json
            var defaultReturnUrl = _configuration["PayOS:DefaultReturnUrl"] ?? "http://localhost:3000";
            var uri = new Uri(defaultReturnUrl);
            var baseUrl = $"{uri.Scheme}://{uri.Authority}";
            
            var returnUrl = $"{baseUrl}/patient/wallet/payment-callback" +
                $"?type=clinic-booking&orderId={order.Id}&appointmentId={appointment.Id}";
            var cancelUrl = $"{baseUrl}/patient/wallet/payment-callback" +
                $"?type=clinic-booking&orderId={order.Id}&appointmentId={appointment.Id}&cancel=true";

            var (paymentUrl, orderCode) = await _payOSService.CreatePaymentLinkAsync(
                payment.Id,
                depositAmount,
                orderDescription,
                returnUrl,
                cancelUrl);

            payment.SetPaymentLink(paymentUrl, orderCode);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Created clinic appointment {AppointmentId} for patient {PatientId} at slot {SlotId}. " +
                "Deposit {DepositAmount} VND (30% of {FullPrice} VND). Order {OrderId}, PayOS URL generated.",
                appointment.Id,
                patientId,
                request.SlotId,
                depositAmount,
                price,
                order.Id);

            // ── 8. Post-commit: notifications (best-effort) ───────────
            // We only send an app notification for booking creation. The email will be sent
            // later when the payment is completed via the webhook.
            await SendBookingNotificationAsync(appointment, slot, request.VisitReason, cancellationToken);

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
        AppointmentSlot slot,
        string? visitReason,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue) return;

        try
        {
            await _notificationService.SendAsync(
                _currentUser.UserId.Value,
                "Đặt lịch khám thành công",
                $"Bạn đã đặt lịch thành công vào lúc {slot.StartTime:HH:mm}, ngày {slot.Date:dd/MM/yyyy}. Vui lòng hoàn tất thanh toán đặt cọc.",
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
