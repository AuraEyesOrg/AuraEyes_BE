using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.ReleaseReservation;

/// <summary>
/// Command to release a slot reservation manually.
/// Can be used by patient to cancel their reservation or by system for cleanup.
/// </summary>
public record ReleaseReservationCommand : ICommand
{
    /// <summary>The reserved appointment slot.</summary>
    public Guid AppointmentSlotId { get; init; }

    /// <summary>The patient releasing the reservation (must match ReservedBy).</summary>
    public Guid? PatientId { get; init; }

    /// <summary>If true, allows system/admin to release any reservation.</summary>
    public bool IsSystemRelease { get; init; }
}
