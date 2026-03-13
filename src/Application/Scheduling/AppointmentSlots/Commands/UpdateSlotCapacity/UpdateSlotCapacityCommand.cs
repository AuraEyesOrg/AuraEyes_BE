using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateSlotCapacity;

/// <summary>
/// Command to update the maximum capacity of an appointment slot.
/// For organisation clinic management.
/// </summary>
public record UpdateSlotCapacityCommand : ICommand<bool>
{
    /// <summary>Slot to update.</summary>
    public Guid SlotId { get; init; }

    /// <summary>New maximum capacity.</summary>
    public int NewCapacity { get; init; }
}
