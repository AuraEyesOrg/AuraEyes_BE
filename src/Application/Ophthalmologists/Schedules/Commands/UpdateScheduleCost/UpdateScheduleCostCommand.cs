using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Schedules.Commands.UpdateScheduleCost;

/// <summary>
/// Command to update the cost on an existing schedule.
/// </summary>
public record UpdateScheduleCostCommand : ICommand
{
    public Guid ScheduleId { get; init; }

    /// <summary>New cost value. Pass null to remove the cost.</summary>
    public decimal? Cost { get; init; }
}
