using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Enums;

namespace Application.SystemAdmin.Ophthalmologists.Queries.GetWithdrawalRequests;

public record GetWithdrawalRequestsQuery : IQuery<PagedResult<AdminWithdrawalRequestDto>>
{
    public PaymentStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
