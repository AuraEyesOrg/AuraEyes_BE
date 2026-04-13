using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlotCost;

/// <summary>
/// Command to update the cost on an existing appointment slot.
/// </summary>
public record UpdateAppointmentSlotCostCommand : ICommand
{
    public Guid AppointmentSlotId { get; init; }

    /// <summary>New cost value.</summary>
    public decimal? Cost { get; init; }
}
