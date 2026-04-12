using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlotCost;

/// <summary>
/// Command to update the cost on an existing appointment slot.
/// </summary>
public record UpdateAppointmentSlotCostCommand : ICommand
{
    public Guid AppointmentSlotId { get; init; }

    /// <summary>New cost value. Pass null to remove the cost.</summary>
    public decimal? Cost { get; init; }
}
