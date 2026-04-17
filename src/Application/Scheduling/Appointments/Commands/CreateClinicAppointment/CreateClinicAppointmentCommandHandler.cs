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

namespace Application.Scheduling.Appointments.Commands.CreateClinicAppointment;

public class CreateClinicAppointmentCommandHandler
    : ICommandHandler<CreateClinicAppointmentCommand, CreateClinicAppointmentResult>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<OrganisationPatientLink> _organisationPatientLinkRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateClinicAppointmentCommandHandler> _logger;

    public CreateClinicAppointmentCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IAppointmentRepository appointmentRepository,
        IRepository<Organisation> organisationRepository,
        IRepository<Patient> patientRepository,
        IRepository<OrganisationPatientLink> organisationPatientLinkRepository,
        IWalletRepository walletRepository,
        ICurrentUserService currentUser,
        IIdentityService identityService,
        IEmailService emailService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<CreateClinicAppointmentCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _appointmentRepository = appointmentRepository;
        _organisationRepository = organisationRepository;
        _patientRepository = patientRepository;
        _organisationPatientLinkRepository = organisationPatientLinkRepository;
        _walletRepository = walletRepository;
        _currentUser = currentUser;
        _identityService = identityService;
        _emailService = emailService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateClinicAppointmentResult>> Handle(
        CreateClinicAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId is null)
        {
            return Result<CreateClinicAppointmentResult>.Unauthorized("Patient profile is required.");
        }

        var organisation = await _organisationRepository.GetByIdAsync(request.OrganisationId, cancellationToken);
        if (organisation is null)
        {
            return Result<CreateClinicAppointmentResult>.NotFound(
                $"Organisation '{request.OrganisationId}' not found.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var slot = await _appointmentSlotRepository.GetByIdWithLockAsync(request.SlotId, cancellationToken);
            if (slot is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CreateClinicAppointmentResult>.NotFound(
                    $"Appointment slot '{request.SlotId}' not found.");
            }

            if (slot.ScheduleTemplate?.OrgId != request.OrganisationId)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CreateClinicAppointmentResult>.Failure("The selected slot does not belong to this organisation.");
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

            // ── Wallet deposit deduction (anti-spam) ──
            var depositFee = slot.Cost ?? 0;

            if (depositFee > 0)
            {
                if (!_currentUser.UserId.HasValue)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.Forbidden(
                        "Unable to resolve user identity for wallet operation.");
                }

                var wallet = await _walletRepository.GetByUserIdAsync(
                    _currentUser.UserId.Value, cancellationToken);

                if (wallet is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.Failure(
                        "Wallet not found. Please top up your wallet first.");
                }

                if (wallet.Balance < depositFee)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateClinicAppointmentResult>.Failure(
                        $"Insufficient wallet balance. Required: {depositFee:N0} VND, Available: {wallet.Balance:N0} VND.");
                }

                wallet.Withdraw(depositFee, $"Clinic booking deposit – Slot {slot.Id}");

                var transaction = new WalletTransaction(
                    wallet.Id,
                    depositFee,
                    TransactionType.Payment,
                    $"Clinic visit deposit – {organisation.Name}",
                    referenceType: "ClinicBooking",
                    referenceId: slot.Id);

                wallet.AddTransaction(transaction);
                await _walletRepository.AddTransactionAsync(transaction, cancellationToken);
            }

            slot.BookWithCapacity();
            if (slot.BookedCount >= slot.MaxCapacity)
            {
                slot.UpdateStatus(ScheduleStatus.Booked);
            }

            var appointment = Appointment.CreateClinicVisit(
                patientId,
                request.SlotId,
                request.OrganisationId,
                request.VisitReason);

            var existingLink = (await _organisationPatientLinkRepository.FindAsync(
                link => link.OrganisationId == request.OrganisationId
                        && link.PatientId == patientId
                        && !link.IsDeleted,
                cancellationToken)).FirstOrDefault();

            if (existingLink is null)
            {
                await _organisationPatientLinkRepository.AddAsync(
                    new OrganisationPatientLink(request.OrganisationId, patientId, "clinic-booking"),
                    cancellationToken);
            }
            else
            {
                existingLink.Touch("clinic-booking");
                await _organisationPatientLinkRepository.UpdateAsync(existingLink, cancellationToken);
            }

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Created clinic appointment {AppointmentId} for patient {PatientId} at slot {SlotId} organisation {OrganisationId}, deposit {DepositFee} VND",
                appointment.Id,
                patientId,
                request.SlotId,
                request.OrganisationId,
                depositFee);

            var appointmentTime = slot.StartTime.ToString("HH:mm");
            var appointmentDate = slot.Date.ToString("dd/MM/yyyy");
            var patientName = "bệnh nhân";
            string? patientEmail = null;

            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
            if (patient is not null)
            {
                if (patient.IsWalkIn)
                {
                    if (!string.IsNullOrWhiteSpace(patient.FullName))
                    {
                        patientName = patient.FullName;
                    }
                }
                else if (patient.UserId.HasValue)
                {
                    var patientUser = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(patientUser?.FullName))
                    {
                        patientName = patientUser.FullName;
                    }

                    if (!string.IsNullOrWhiteSpace(patientUser?.Email))
                    {
                        patientEmail = patientUser.Email;
                    }
                }
            }

            if (_currentUser.UserId.HasValue)
            {
                try
                {
                    await _notificationService.SendAsync(
                        _currentUser.UserId.Value,
                        "Đặt lịch khám thành công",
                        $"Bạn đã đặt lịch thành công vào lúc {appointmentTime}, ngày {appointmentDate}",
                        NotificationType.NewAppointmentBooked,
                        new
                        {
                            AppointmentId = appointment.Id,
                            AppointmentTime = $"{slot.Date:yyyy-MM-dd}T{slot.StartTime.ToString("HH:mm")}:00",
                            Reason = request.VisitReason,
                            OrganisationId = request.OrganisationId
                        },
                        cancellationToken);

                    var organisationAdminUserIds = await _identityService.GetUserIdsByRoleAndOrganizationAsync(
                        Roles.OrgAdmin,
                        request.OrganisationId,
                        cancellationToken);

                    foreach (var providerUserId in organisationAdminUserIds)
                    {
                        await _notificationService.SendAsync(
                            providerUserId,
                            "Lịch hẹn mới từ bệnh nhân",
                            $"Bạn có 1 lịch vào lúc {appointmentTime}, ngày {appointmentDate} từ bệnh nhân {patientName}",
                            NotificationType.NewAppointmentBooked,
                            new
                            {
                                AppointmentId = appointment.Id,
                                AppointmentTime = $"{slot.Date:yyyy-MM-dd}T{slot.StartTime.ToString("HH:mm")}:00",
                                PatientId = patientId,
                                OrganisationId = request.OrganisationId
                            },
                            cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send appointment booking notification for appointment {AppointmentId}",
                        appointment.Id);
                }
            }

            if (!string.IsNullOrWhiteSpace(patientEmail))
            {
                try
                {
                    var qrPayload =
                        $"AURA-CLINIC-APPOINTMENT|{appointment.Id}|{patientId}|{request.OrganisationId}|{slot.Date:yyyy-MM-dd}|{slot.StartTime:HH:mm}|{slot.EndTime:HH:mm}";

                    var checkInCode = appointment.Id.ToString("N")[..10].ToUpperInvariant();

                    await _emailService.SendClinicAppointmentConfirmationAsync(
                        patientEmail,
                        new ClinicAppointmentConfirmationEmailPayload(
                            appointment.Id,
                            patientName,
                            organisation.Name,
                            slot.Date,
                            slot.StartTime,
                            slot.EndTime,
                            request.VisitReason,
                            checkInCode,
                            qrPayload),
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send clinic appointment email for appointment {AppointmentId}",
                        appointment.Id);
                }
            }

            return Result<CreateClinicAppointmentResult>.Success(new CreateClinicAppointmentResult
            {
                AppointmentId = appointment.Id,
                Status = appointment.Status
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
}
