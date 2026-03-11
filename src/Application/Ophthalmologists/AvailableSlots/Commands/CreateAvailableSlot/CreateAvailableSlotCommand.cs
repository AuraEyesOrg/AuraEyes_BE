using Application.Common.Interfaces;

namespace Application.Ophthalmologists.AvailableSlots.Commands.CreateAvailableSlot;

/// <summary>
/// Command to create a new available slot for booking.
/// </summary>
public record CreateAvailableSlotCommand : ICommand<Guid>
{
    /// <summary>Organisation ID (optional - can be a solo doctor slot).</summary>
    public Guid? OrganisationId { get; init; }

    /// <summary>Ophthalmologist ID (optional - can be a clinic-only slot).</summary>
    public Guid? OphthalmologistId { get; init; }

    /// <summary>Start time of the available slot.</summary>
    public DateTime StartTime { get; init; }

    /// <summary>End time of the available slot.</summary>
    public DateTime EndTime { get; init; }

    /// <summary>Maximum concurrent patients in this time window.</summary>
    public int MaxCapacity { get; init; }
}
