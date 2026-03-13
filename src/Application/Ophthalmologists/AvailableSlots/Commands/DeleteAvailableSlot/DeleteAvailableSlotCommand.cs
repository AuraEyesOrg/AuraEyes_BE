using Application.Common.Interfaces;

namespace Application.Ophthalmologists.AvailableSlots.Commands.DeleteAvailableSlot;

/// <summary>
/// Command to delete an available slot.
/// </summary>
public record DeleteAvailableSlotCommand : ICommand
{
    /// <summary>ID of the available slot to delete.</summary>
    public Guid AvailableSlotId { get; init; }
}
