using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;

namespace Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkloads;

public record GetDoctorWorkloadsQuery : IQuery<PagedResult<DoctorWorkloadListItemDto>>
{
    public WorkloadPeriodType PeriodType { get; init; } = WorkloadPeriodType.Week;
    public DateOnly Date { get; init; }
    public string? SearchTerm { get; init; }
    public OphthalmologistEmploymentType? EmploymentType { get; init; }
    public string? Status { get; init; }
    public bool WarningOnly { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
