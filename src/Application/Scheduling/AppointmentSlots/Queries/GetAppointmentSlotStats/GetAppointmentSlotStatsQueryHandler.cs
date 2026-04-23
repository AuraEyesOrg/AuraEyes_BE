using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlotStats;

public class GetAppointmentSlotStatsQueryHandler : IQueryHandler<GetAppointmentSlotStatsQuery, AppointmentSlotStatsDto>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;

    public GetAppointmentSlotStatsQueryHandler(IAppointmentSlotRepository appointmentSlotRepository)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
    }

    public async Task<Result<AppointmentSlotStatsDto>> Handle(
        GetAppointmentSlotStatsQuery request,
        CancellationToken cancellationToken)
    {
        var counts = await _appointmentSlotRepository.GetStatusCountsAsync(cancellationToken);

        counts.TryGetValue(ScheduleStatus.Available, out var available);
        counts.TryGetValue(ScheduleStatus.Blocked, out var blocked);
        var total = counts.Values.Sum();

        var dto = new AppointmentSlotStatsDto
        {
            TotalCount = total,
            AvailableCount = available,
            BlockedCount = blocked,
        };

        return Result<AppointmentSlotStatsDto>.Success(dto);
    }
}
