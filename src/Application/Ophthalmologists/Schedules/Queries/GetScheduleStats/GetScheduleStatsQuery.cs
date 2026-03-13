using Application.Common.Interfaces;
using Application.Ophthalmologists.Schedules.Common;

namespace Application.Ophthalmologists.Schedules.Queries.GetScheduleStats;

/// <summary>
/// Query to get schedule statistics for an ophthalmologist.
/// </summary>
/// <param name="OphthalmologistId">The ophthalmologist to retrieve stats for.</param>
public record GetScheduleStatsQuery(Guid OphthalmologistId) : IQuery<ScheduleStatsDto>;
