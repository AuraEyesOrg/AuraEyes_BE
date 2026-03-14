using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlotStats;

/// <summary>
/// Handler for GetAppointmentSlotStatsQuery.
/// Aggregates appointment slot counts by status for dashboard/analytics use.
/// </summary>
public class GetAppointmentSlotStatsQueryHandler : IQueryHandler<GetAppointmentSlotStatsQuery, AppointmentSlotStatsDto>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public GetAppointmentSlotStatsQueryHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IOphthalmologistRepository ophthalmologistRepository)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<AppointmentSlotStatsDto>> Handle(
        GetAppointmentSlotStatsQuery request,
        CancellationToken cancellationToken)
    {
        // Validate ophthalmologist exists if provided
        if (request.OphthalId.HasValue)
        {
            var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(
                request.OphthalId.Value, cancellationToken);
            if (ophthalmologist is null)
            {
                return Result<AppointmentSlotStatsDto>.NotFound(
                    $"Ophthalmologist '{request.OphthalId}' not found.");
            }
        }

        var counts = await _appointmentSlotRepository.GetStatusCountsAsync(
            request.OphthalId, request.OrgId, cancellationToken);

        counts.TryGetValue(ScheduleStatus.Available, out var available);
        counts.TryGetValue(ScheduleStatus.Booked, out var booked);
        counts.TryGetValue(ScheduleStatus.Completed, out var completed);
        counts.TryGetValue(ScheduleStatus.Cancelled, out var cancelled);
        counts.TryGetValue(ScheduleStatus.NoShow, out var noShow);
        var total = counts.Values.Sum();

        var dto = new AppointmentSlotStatsDto
        {
            TotalCount = total,
            AvailableCount = available,
            BookedCount = booked,
            CompletedCount = completed,
            CancelledCount = cancelled,
            NoShowCount = noShow,
        };

        return Result<AppointmentSlotStatsDto>.Success(dto);
    }
}
