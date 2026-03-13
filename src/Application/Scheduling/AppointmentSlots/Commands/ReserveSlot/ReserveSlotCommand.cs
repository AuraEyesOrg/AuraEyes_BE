using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.ReserveSlot;

/// <summary>
/// Command to reserve an appointment slot for a patient.
/// Creates a temporary reservation with an expiration time.
/// </summary>
public record ReserveSlotCommand : ICommand<ReserveSlotResult>
{
    /// <summary>The appointment slot to reserve.</summary>
    public Guid AppointmentSlotId { get; init; }

    /// <summary>The patient making the reservation.</summary>
    public Guid PatientId { get; init; }

    /// <summary>Reservation duration in minutes (default: 5 minutes).</summary>
    public int ReservationMinutes { get; init; } = 5;
}

/// <summary>
/// Result of a slot reservation.
/// </summary>
public record ReserveSlotResult
{
    /// <summary>The reserved slot ID.</summary>
    public Guid SlotId { get; init; }

    /// <summary>When the reservation expires.</summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>Remaining seconds until expiration.</summary>
    public int RemainingSeconds { get; init; }
}
