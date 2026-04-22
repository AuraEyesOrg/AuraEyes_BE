using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Cashflow.Queries.GetCashflowTransactions;

public record GetCashflowTransactionsQuery : IQuery<PagedResult<CashflowTransactionDto>>
{
    public string? ActorRole { get; init; }
    public string? Status { get; init; }
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
