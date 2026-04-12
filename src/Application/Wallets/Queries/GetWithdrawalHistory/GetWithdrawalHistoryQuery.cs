using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;

namespace Application.Wallets.Queries.GetWithdrawalHistory;

public record GetWithdrawalHistoryQuery : IQuery<PagedResult<WithdrawalRequestDto>>
{
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
