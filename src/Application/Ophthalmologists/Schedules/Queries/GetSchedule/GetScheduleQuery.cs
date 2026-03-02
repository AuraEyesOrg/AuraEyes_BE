using Application.Common.Interfaces;
using Application.Ophthalmologists.Schedules.Common;

namespace Application.Ophthalmologists.Schedules.Queries.GetSchedule;

/// <summary>
/// Query to get a single schedule by ID.
/// </summary>
/// <param name="ScheduleId">The schedule ID.</param>
public record GetScheduleQuery(Guid ScheduleId) : IQuery<ScheduleDto>;
