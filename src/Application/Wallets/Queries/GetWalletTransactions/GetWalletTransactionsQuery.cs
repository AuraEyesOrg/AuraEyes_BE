using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;

namespace Application.Wallets.Queries.GetWalletTransactions;

/// <summary>
/// Query to get wallet transactions with pagination.
/// </summary>
public record GetWalletTransactionsQuery : IQuery<PagedResult<WalletTransactionDto>>
{
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
