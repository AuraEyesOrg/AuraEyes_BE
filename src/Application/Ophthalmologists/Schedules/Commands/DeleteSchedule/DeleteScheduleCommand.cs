using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Schedules.Commands.DeleteSchedule;

/// <summary>
/// Command to soft-delete (cancel) a schedule.
/// Only Available or Booked schedules can be deleted; the entity status transitions to Cancelled.
/// </summary>
public record DeleteScheduleCommand : ICommand
{
    public Guid ScheduleId { get; init; }
}
