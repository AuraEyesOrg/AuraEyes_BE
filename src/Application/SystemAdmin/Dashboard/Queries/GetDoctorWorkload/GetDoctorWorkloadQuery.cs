using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkload;

public record GetDoctorWorkloadQuery : IQuery<DoctorWorkloadDto>
{
    public Guid DoctorId { get; init; }
    public WorkloadPeriodType PeriodType { get; init; } = WorkloadPeriodType.Week;
    public DateOnly Date { get; init; }
}
