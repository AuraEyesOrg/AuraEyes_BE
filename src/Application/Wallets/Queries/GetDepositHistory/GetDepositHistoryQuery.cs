using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;

namespace Application.Wallets.Queries.GetDepositHistory;

/// <summary>
/// Query to get deposit history with pagination.
/// </summary>
public record GetDepositHistoryQuery : IQuery<PagedResult<DepositRequestDto>>
{
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
