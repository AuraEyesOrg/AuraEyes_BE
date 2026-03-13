using Application.Common.Interfaces;

namespace Application.Ophthalmologists.AvailableSlots.Commands.UpdateAvailableSlot;

/// <summary>
/// Command to update an existing available slot.
/// </summary>
public record UpdateAvailableSlotCommand : ICommand
{
    /// <summary>ID of the available slot to update.</summary>
    public Guid AvailableSlotId { get; init; }

    /// <summary>New start time.</summary>
    public DateTime StartTime { get; init; }

    /// <summary>New end time.</summary>
    public DateTime EndTime { get; init; }

    /// <summary>New max capacity.</summary>
    public int MaxCapacity { get; init; }
}
