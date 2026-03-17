using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Constants;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.ReleaseReservation;

/// <summary>
/// Handler for ReleaseReservationCommand.
/// Releases a slot reservation and makes it available again.
/// </summary>
public class ReleaseReservationCommandHandler : ICommandHandler<ReleaseReservationCommand>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<ReleaseReservationCommandHandler> _logger;

    public ReleaseReservationCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ILogger<ReleaseReservationCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result> Handle(ReleaseReservationCommand request, CancellationToken cancellationToken)
    {
        var slot = await _appointmentSlotRepository.GetByIdAsync(
            request.AppointmentSlotId, cancellationToken);

        if (slot is null)
        {
            return Result.NotFound($"Appointment slot '{request.AppointmentSlotId}' not found.");
        }

        if (slot.Status != ScheduleStatus.Reserved)
        {
            return Result.Failure($"Slot is not in reserved state. Current status: {slot.Status}");
        }

        // Verify authorization
        if (!request.IsSystemRelease)
        {
            Guid? effectivePatientProfileId = request.PatientId;

            if (_currentUser.IsInRole(Roles.Patient))
            {
                if (!_currentUser.ProfileId.HasValue)
                {
                    return Result.Forbidden("Unable to resolve patient profile from current token.");
                }

                effectivePatientProfileId = _currentUser.ProfileId.Value;
            }

            if (!effectivePatientProfileId.HasValue)
            {
                return Result.Forbidden("Patient ID is required to release a reservation.");
            }

            var reservedByMatchesProfile = slot.ReservedBy == effectivePatientProfileId.Value;
            var reservedByMatchesUser = _currentUser.UserId.HasValue && slot.ReservedBy == _currentUser.UserId.Value;

            if (!reservedByMatchesProfile && !reservedByMatchesUser)
            {
                return Result.Forbidden("You are not authorized to release this reservation.");
            }
        }

        try
        {
            slot.ReleaseReservation();
            await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Reservation released for slot {SlotId}. Released by: {ReleasedBy}",
                request.AppointmentSlotId,
                request.IsSystemRelease ? "System" : request.PatientId.ToString());

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
