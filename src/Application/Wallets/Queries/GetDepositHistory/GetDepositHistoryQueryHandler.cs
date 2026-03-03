using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Repositories;

namespace Application.Wallets.Queries.GetDepositHistory;

/// <summary>
/// Handler for GetDepositHistoryQuery.
/// </summary>
public class GetDepositHistoryQueryHandler : IQueryHandler<GetDepositHistoryQuery, PagedResult<DepositRequestDto>>
{
    private readonly IDepositRequestRepository _depositRequestRepository;

    public GetDepositHistoryQueryHandler(IDepositRequestRepository depositRequestRepository)
    {
        _depositRequestRepository = depositRequestRepository;
    }

    public async Task<Result<PagedResult<DepositRequestDto>>> Handle(
        GetDepositHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _depositRequestRepository.GetByUserIdPagedAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(d => new DepositRequestDto
        {
            Id = d.Id,
            UserId = d.UserId,
            WalletId = d.WalletId,
            Amount = d.Amount,
            PaymentMethod = d.PaymentMethod,
            Status = d.Status,
            PaymentOrderCode = d.PaymentOrderCode,
            PaymentUrl = d.PaymentUrl,
            Description = d.Description,
            CreatedAt = d.CreatedAt,
            CompletedAt = d.CompletedAt,
            FailureReason = d.FailureReason
        }).ToList();

        var pagedResult = new PagedResult<DepositRequestDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<DepositRequestDto>>.Success(pagedResult);
    }
}
