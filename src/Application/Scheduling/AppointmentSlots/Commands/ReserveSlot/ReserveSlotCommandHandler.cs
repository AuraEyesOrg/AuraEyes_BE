using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Constants;
using Application.Common.Helpers;
using Application.SystemSettings.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.ReserveSlot;

/// <summary>
/// Handler for ReserveSlotCommand.
/// Uses database transaction with row locking to prevent race conditions.
/// </summary>
public class ReserveSlotCommandHandler : ICommandHandler<ReserveSlotCommand, ReserveSlotResult>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<ReserveSlotCommandHandler> _logger;
    private readonly ISystemSettingService _settingService;

    public ReserveSlotCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ILogger<ReserveSlotCommandHandler> logger,
        ISystemSettingService settingService)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
        _settingService = settingService;
    }

    public async Task<Result<ReserveSlotResult>> Handle(ReserveSlotCommand request, CancellationToken cancellationToken)
    {
        // Start a transaction to ensure atomicity
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            Guid effectivePatientProfileId = request.PatientId;

            if (_currentUser.IsInRole(Roles.Patient))
            {
                if (!_currentUser.ProfileId.HasValue)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReserveSlotResult>.Forbidden(
                        "Unable to resolve patient profile from current token.");
                }

                effectivePatientProfileId = _currentUser.ProfileId.Value;
            }

            // Get the slot with a row lock to prevent concurrent modifications
            var slot = await _appointmentSlotRepository.GetByIdWithLockAsync(
                request.AppointmentSlotId, cancellationToken);

            if (slot is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<ReserveSlotResult>.NotFound($"Appointment slot '{request.AppointmentSlotId}' not found.");
            }

            // Handle reserved slots first to avoid false conflicts for the same patient.
            if (slot.Status == ScheduleStatus.Reserved)
            {
                if (slot.IsReservationExpired())
                {
                    slot.ReleaseReservation();
                }
                else
                {
                    var reservedByMatchesProfile = slot.ReservedBy == effectivePatientProfileId;
                    var reservedByMatchesUser = _currentUser.UserId.HasValue && slot.ReservedBy == _currentUser.UserId.Value;

                    if (reservedByMatchesProfile || reservedByMatchesUser)
                    {
                        var expiresAt = slot.ReservationExpireAt ?? DateTime.UtcNow.AddMinutes(request.ReservationMinutes);
                        var remainingSeconds = Math.Max(
                            0,
                            (int)Math.Floor((expiresAt - DateTime.UtcNow).TotalSeconds));

                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                        return Result<ReserveSlotResult>.Success(new ReserveSlotResult
                        {
                            SlotId = slot.Id,
                            ExpiresAt = expiresAt,
                            RemainingSeconds = remainingSeconds
                        });
                    }

                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<ReserveSlotResult>.Conflict(
                        "This slot is already reserved by another patient. Please try a different slot.");
                }
            }

            // Check if slot is available for reservation
            if (slot.Status != ScheduleStatus.Available)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                // Provide specific error messages based on status
                return slot.Status switch
                {
                    ScheduleStatus.Reserved => Result<ReserveSlotResult>.Conflict(
                        "This slot is already reserved by another patient. Please try a different slot."),
                    ScheduleStatus.Booked => Result<ReserveSlotResult>.Conflict(
                        "This slot is already booked. Please try a different slot."),
                    ScheduleStatus.Blocked => Result<ReserveSlotResult>.Conflict(
                        "This slot is not available. Please try a different slot."),
                    _ => Result<ReserveSlotResult>.Conflict(
                        $"This slot is not available. Current status: {slot.Status}")
                };
            }

            // Check advance booking constraint
            var advanceBookingStr = await _settingService.GetSettingAsync("MIN_ADVANCE_BOOKING_HOURS", cancellationToken);
            double advanceBookingHours = 0;
            if (!string.IsNullOrEmpty(advanceBookingStr) && double.TryParse(advanceBookingStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double parsed))
            {
                advanceBookingHours = parsed;
            }
            if (advanceBookingHours < 0.5)
            {
                advanceBookingHours = 0.5; // Enforce minimum 30 minutes
            }

            var localAppointmentTime = slot.Date.ToDateTime(slot.StartTime, DateTimeKind.Unspecified);
            var appointmentTimeUtc = TimeZoneInfo.ConvertTimeToUtc(localAppointmentTime, VietnamTimeZoneResolver.TimeZone);
            
            if (appointmentTimeUtc <= DateTime.UtcNow.AddHours(advanceBookingHours))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                var formattedTime = advanceBookingHours < 1 ? $"{Math.Round(advanceBookingHours * 60)} minute(s)" : $"{advanceBookingHours} hour(s)";
                return Result<ReserveSlotResult>.Conflict($"Slots must be booked at least {formattedTime} in advance.");
            }

            // Calculate expiration time
            var expirationTime = DateTime.UtcNow.AddMinutes(request.ReservationMinutes);

            // Reserve the slot
            slot.Reserve(effectivePatientProfileId, expirationTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Slot {SlotId} reserved by patient {PatientId}, expires at {ExpiresAt}",
                request.AppointmentSlotId, effectivePatientProfileId, expirationTime);

            return Result<ReserveSlotResult>.Success(new ReserveSlotResult
            {
                SlotId = slot.Id,
                ExpiresAt = expirationTime,
                RemainingSeconds = request.ReservationMinutes * 60
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error reserving slot {SlotId}", request.AppointmentSlotId);
            throw;
        }
    }
}
