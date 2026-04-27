using Application.Common.Interfaces;
using Application.Scheduling.AppointmentSlots.Common;

namespace Application.Scheduling.AppointmentSlots.Queries.GetClinicSchedule;

public record GetClinicScheduleQuery : IQuery<ClinicScheduleDto>
{
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
}
