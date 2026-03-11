using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Schedules.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.Schedules.Queries.GetScheduleStats;

/// <summary>
/// Handler for GetScheduleStatsQuery.
/// Aggregates schedule counts by status for dashboard/analytics use.
/// </summary>
public class GetScheduleStatsQueryHandler : IQueryHandler<GetScheduleStatsQuery, ScheduleStatsDto>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public GetScheduleStatsQueryHandler(
        IScheduleRepository scheduleRepository,
        IOphthalmologistRepository ophthalmologistRepository)
    {
        _scheduleRepository = scheduleRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<ScheduleStatsDto>> Handle(
        GetScheduleStatsQuery request,
        CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(
            request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
            return Result<ScheduleStatsDto>.NotFound(
                $"Ophthalmologist '{request.OphthalmologistId}' not found.");

        var counts = await _scheduleRepository.GetStatusCountsAsync(
            request.OphthalmologistId, cancellationToken);

        counts.TryGetValue(ScheduleStatus.Available, out var available);
        counts.TryGetValue(ScheduleStatus.Booked, out var booked);
        counts.TryGetValue(ScheduleStatus.Completed, out var completed);
        counts.TryGetValue(ScheduleStatus.Cancelled, out var cancelled);
        counts.TryGetValue(ScheduleStatus.NoShow, out var noShow);
        var total = counts.Values.Sum();

        var dto = new ScheduleStatsDto
        {
            TotalCount = total,
            AvailableCount = available,
            BookedCount = booked,
            CompletedCount = completed,
            CancelledCount = cancelled,
            NoShowCount = noShow,
        };

        return Result<ScheduleStatsDto>.Success(dto);
    }
}
