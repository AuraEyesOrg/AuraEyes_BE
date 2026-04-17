using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Constants;
using Application.Common.Helpers;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Financial;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.ConfirmReservation;

/// <summary>
/// Confirms a slot reservation: deducts wallet, creates Google Meet, and
/// creates a Confirmed ConsultationSession in a single transaction.
/// </summary>
public class ConfirmReservationCommandHandler : ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IConsultationSessionRepository _consultationSessionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IRepository<AiScreening> _aiScreeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;
    private readonly IGoogleMeetService _googleMeetService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<ConfirmReservationCommandHandler> _logger;

    public ConfirmReservationCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IWalletRepository walletRepository,
        IRepository<AiScreening> aiScreeningRepository,
        IRepository<Patient> patientRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService,
        INotificationService notificationService,
        IGoogleMeetService googleMeetService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ILogger<ConfirmReservationCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _walletRepository = walletRepository;
        _aiScreeningRepository = aiScreeningRepository;
        _patientRepository = patientRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
        _notificationService = notificationService;
        _googleMeetService = googleMeetService;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<ConfirmReservationResult>> Handle(
        ConfirmReservationCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // ── 1. Resolve patient profile ──
            Guid effectivePatientProfileId = request.PatientId;

            if (_currentUser.IsInRole(Roles.Patient))
            {
                if (!_currentUser.ProfileId.HasValue)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ConfirmReservationResult>.Forbidden(
                        "Unable to resolve patient profile from current token.");
                }
                effectivePatientProfileId = _currentUser.ProfileId.Value;
            }

            // ── 2. Lock and validate the slot ──
            var slot = await _appointmentSlotRepository.GetByIdWithLockAsync(
                request.AppointmentSlotId, cancellationToken);

            if (slot is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ConfirmReservationResult>.NotFound(
                    $"Appointment slot '{request.AppointmentSlotId}' not found.");
            }

            if (slot.Status != ScheduleStatus.Reserved)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ConfirmReservationResult>.Failure(
                    $"Slot is not in reserved state. Current status: {slot.Status}");
            }

            var reservedByMatchesProfile = slot.ReservedBy == effectivePatientProfileId;
            var reservedByMatchesUser = _currentUser.UserId.HasValue && slot.ReservedBy == _currentUser.UserId.Value;
            if (!reservedByMatchesProfile && !reservedByMatchesUser)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ConfirmReservationResult>.Forbidden(
                    "This slot was reserved by a different patient.");
            }

            if (slot.IsReservationExpired())
            {
                slot.ReleaseReservation();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return Result<ConfirmReservationResult>.Failure(
                    "Your reservation has expired. Please try booking again.");
            }

            // ── 3. Wallet balance check & deduction ──
            Guid? linkedAiScreeningId = null;
            if (request.AiScreeningId.HasValue)
            {
                var screening = await _aiScreeningRepository.GetByIdAsync(
                    request.AiScreeningId.Value, cancellationToken);

                if (screening is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ConfirmReservationResult>.NotFound(
                        $"AI screening '{request.AiScreeningId.Value}' not found.");
                }

                if (screening.PatientId != effectivePatientProfileId)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ConfirmReservationResult>.Forbidden(
                        "You can only attach your own screening result to this consultation.");
                }

                linkedAiScreeningId = screening.Id;
            }

            // ── 4. Wallet balance check & deduction ──
            var consultationFee = slot.Cost ?? 0;

            if (consultationFee > 0)
            {
                if (!_currentUser.UserId.HasValue)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ConfirmReservationResult>.Forbidden(
                        "Unable to resolve user identity for wallet operation.");
                }

                var wallet = await _walletRepository.GetByUserIdAsync(
                    _currentUser.UserId.Value, cancellationToken);

                if (wallet is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ConfirmReservationResult>.Failure(
                        "Wallet not found. Please top up your wallet first.");
                }

                if (wallet.Balance < consultationFee)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ConfirmReservationResult>.Failure(
                        $"Insufficient wallet balance. Required: {consultationFee:N0} VND, Available: {wallet.Balance:N0} VND.");
                }

                wallet.Withdraw(consultationFee, $"Consultation booking – Slot {slot.Id}");

                var transaction = new WalletTransaction(
                    wallet.Id,
                    consultationFee,
                    TransactionType.Payment,
                    $"Video-call consultation booking",
                    referenceType: "Booking",
                    referenceId: slot.Id);

                wallet.AddTransaction(transaction);
                await _walletRepository.AddTransactionAsync(transaction, cancellationToken);
            }

            // ── 5. Resolve attendee emails & create Google Meet ──
            var ophthalmologistId = slot.ScheduleTemplate?.OphthalId;
            var appointmentTime = CalculateAppointmentTimeUtc(slot);

            var attendeeEmails = await ResolveAttendeeEmailsAsync(
                effectivePatientProfileId, ophthalmologistId, cancellationToken);

            MeetingInfo? meetingInfo = null;
            try
            {
                meetingInfo = await _googleMeetService.CreateMeetingAsync(
                    "AURA Consultation",
                    appointmentTime,
                    attendeeEmails,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to create Google Meet for slot {SlotId}. Session will be created without a link.",
                    slot.Id);
            }

            // ── 6. Confirm slot & create session (Status = Confirmed) ──
            slot.ConfirmReservation(effectivePatientProfileId);

            var session = ConsultationSession.CreateVideoCall(
                patientId: effectivePatientProfileId,
                price: consultationFee,
                appointmentTime: appointmentTime,
                ophthalmologistId: ophthalmologistId,
                appointmentSlotId: slot.Id,
                aiScreeningId: linkedAiScreeningId,
                shareRetinalImages: request.ShareRetinalImages,
                shareAiResults: request.ShareAiResults,
                meetingLink: meetingInfo?.MeetingLink,
                calendarEventId: meetingInfo?.CalendarEventId);

            await _consultationSessionRepository.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var appointmentTimeText = slot.StartTime.ToString("HH:mm");
            var appointmentDateText = slot.Date.ToString("dd/MM/yyyy");
            var appointmentTimestamp = $"{slot.Date:yyyy-MM-dd}T{slot.StartTime:HH:mm}:00";
            var sharedMedicalData = linkedAiScreeningId.HasValue &&
                                    (request.ShareRetinalImages || request.ShareAiResults);

            var patient = await _patientRepository.GetByIdAsync(effectivePatientProfileId, cancellationToken);
            string patientName;
            if (patient is not null && patient.IsWalkIn)
            {
                patientName = !string.IsNullOrWhiteSpace(patient.FullName)
                    ? patient.FullName
                    : "bệnh nhân";
            }
            else if (patient is not null && patient.UserId.HasValue)
            {
                var patientUser = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
                patientName = !string.IsNullOrWhiteSpace(patientUser?.FullName)
                    ? patientUser.FullName
                    : "bệnh nhân";
            }
            else
            {
                patientName = "bệnh nhân";
            }

            if (patient?.UserId is Guid patientUserId)
            {
                try
                {
                    await _notificationService.SendAsync(
                        patientUserId,
                        "Đặt lịch thành công",
                        $"Bạn đã đặt lịch thành công vào lúc {appointmentTimeText}, ngày {appointmentDateText}",
                        NotificationType.NewAppointmentBooked,
                        new
                        {
                            ConsultationSessionId = session.Id,
                            AppointmentSlotId = slot.Id,
                            AppointmentTime = appointmentTimestamp,
                            AiScreeningId = linkedAiScreeningId,
                            SharedMedicalData = sharedMedicalData
                        },
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to send booking notification to patient for slot {SlotId}",
                        slot.Id);
                }
            }

            try
            {
                if (ophthalmologistId.HasValue)
                {
                    var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(
                        ophthalmologistId.Value,
                        cancellationToken);

                    if (ophthalmologist is not null)
                    {
                        var doctorBody = sharedMedicalData && linkedAiScreeningId.HasValue
                            ? $"Bạn có lịch {appointmentTimeText} ngày {appointmentDateText} từ {patientName}. Bệnh nhân đã đính kèm hồ sơ sàng lọc — mở thông báo để xem chi tiết AI trên trang tư vấn."
                            : $"Bạn có 1 lịch vào lúc {appointmentTimeText}, ngày {appointmentDateText} từ bệnh nhân {patientName}";

                        await _notificationService.SendAsync(
                            ophthalmologist.UserId,
                            "Lịch hẹn mới từ bệnh nhân",
                            doctorBody,
                            NotificationType.NewAppointmentBooked,
                            new
                            {
                                ConsultationSessionId = session.Id,
                                AppointmentSlotId = slot.Id,
                                AppointmentTime = appointmentTimestamp,
                                PatientId = effectivePatientProfileId,
                                AiScreeningId = linkedAiScreeningId,
                                SharedMedicalData = sharedMedicalData
                            },
                            cancellationToken);
                    }
                }
                else if (slot.ScheduleTemplate?.OrgId is Guid orgId)
                {
                    var organisationAdminUserIds = await _identityService.GetUserIdsByRoleAndOrganizationAsync(
                        Roles.OrgAdmin,
                        orgId,
                        cancellationToken);

                    foreach (var providerUserId in organisationAdminUserIds)
                    {
                        await _notificationService.SendAsync(
                            providerUserId,
                            "Lịch hẹn mới từ bệnh nhân",
                            sharedMedicalData && linkedAiScreeningId.HasValue
                                ? $"Bạn có lịch {appointmentTimeText} ngày {appointmentDateText} từ {patientName}. Hồ sơ sàng lọc đã được chia sẻ — xem trong hệ thống."
                                : $"Bạn có 1 lịch vào lúc {appointmentTimeText}, ngày {appointmentDateText} từ bệnh nhân {patientName}",
                            NotificationType.NewAppointmentBooked,
                            new
                            {
                                ConsultationSessionId = session.Id,
                                AppointmentSlotId = slot.Id,
                                AppointmentTime = appointmentTimestamp,
                                PatientId = effectivePatientProfileId,
                                OrganisationId = orgId,
                                AiScreeningId = linkedAiScreeningId,
                                SharedMedicalData = sharedMedicalData
                            },
                            cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send provider booking notification for slot {SlotId}",
                    slot.Id);
            }

            _logger.LogInformation(
                "Reservation confirmed: slot {SlotId} → session {SessionId} (Confirmed), fee {Fee} VND deducted",
                slot.Id, session.Id, consultationFee);

            return Result<ConfirmReservationResult>.Success(new ConfirmReservationResult
            {
                ConsultationSessionId = session.Id,
                AppointmentSlotId = slot.Id,
                AppointmentTime = appointmentTime
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error confirming reservation for slot {SlotId}", request.AppointmentSlotId);
            throw;
        }
    }

    private DateTime CalculateAppointmentTimeUtc(Domain.Entities.Scheduling.AppointmentSlot slot)
    {
        var localAppointmentTime = slot.Date.ToDateTime(slot.StartTime, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(localAppointmentTime, VietnamTimeZoneResolver.TimeZone);
    }

    private async Task<List<string>> ResolveAttendeeEmailsAsync(
        Guid patientProfileId,
        Guid? ophthalmologistId,
        CancellationToken cancellationToken)
    {
        var emails = new List<string>();

        var patient = await _patientRepository.GetByIdAsync(patientProfileId, cancellationToken);
        if (patient is not null && !patient.IsWalkIn && patient.UserId.HasValue)
        {
            var patientUser = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
            if (patientUser is not null && !string.IsNullOrWhiteSpace(patientUser.Email))
                emails.Add(patientUser.Email);
        }

        if (ophthalmologistId.HasValue)
        {
            var doctor = await _ophthalmologistRepository.GetByIdAsync(ophthalmologistId.Value, cancellationToken);
            if (doctor is not null)
            {
                var doctorUser = await _identityService.GetUserByIdAsync(doctor.UserId, cancellationToken);
                if (doctorUser is not null && !string.IsNullOrWhiteSpace(doctorUser.Email))
                    emails.Add(doctorUser.Email);
            }
        }

        return emails;
    }
}
