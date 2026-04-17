using Application.Common.Interfaces;

namespace Application.SystemAdmin.Dashboard.Queries.GetPartTimeSlotQuotaUsage;

public record GetPartTimeSlotQuotaUsageQuery : IQuery<IReadOnlyList<PartTimeSlotQuotaUsageDto>>
{
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
}