using Application.Common.Interfaces;
using Application.Scheduling.AppointmentSlots.Common;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlotStats;

public record GetAppointmentSlotStatsQuery : IQuery<AppointmentSlotStatsDto>
{
}
