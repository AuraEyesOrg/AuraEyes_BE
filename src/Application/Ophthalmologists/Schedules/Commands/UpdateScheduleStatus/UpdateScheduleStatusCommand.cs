using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Ophthalmologists.Schedules.Commands.UpdateScheduleStatus;

/// <summary>
/// Command to update the status of a schedule.
/// </summary>
public record UpdateScheduleStatusCommand : ICommand<bool>
{
    /// <summary>
    /// Schedule ID to update.
    /// </summary>
    public Guid ScheduleId { get; init; }

    /// <summary>
    /// New status for the schedule.
    /// </summary>
    public ScheduleStatus NewStatus { get; init; }
}
