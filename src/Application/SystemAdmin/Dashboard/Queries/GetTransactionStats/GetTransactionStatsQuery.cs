using Application.Common.Interfaces;

namespace Application.SystemAdmin.Dashboard.Queries.GetTransactionStats;

public record GetTransactionStatsQuery : IQuery<IReadOnlyList<TransactionStatsDto>>
{
    public string Period { get; init; } = "daily";
}
