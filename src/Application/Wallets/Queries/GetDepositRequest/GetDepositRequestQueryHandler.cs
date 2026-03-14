using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Repositories;

namespace Application.Wallets.Queries.GetDepositRequest;

/// <summary>
/// Handler for GetDepositRequestQuery.
/// </summary>
public class GetDepositRequestQueryHandler : IQueryHandler<GetDepositRequestQuery, DepositRequestDto>
{
    private readonly IDepositRequestRepository _depositRequestRepository;

    public GetDepositRequestQueryHandler(IDepositRequestRepository depositRequestRepository)
    {
        _depositRequestRepository = depositRequestRepository;
    }

    public async Task<Result<DepositRequestDto>> Handle(
        GetDepositRequestQuery request,
        CancellationToken cancellationToken)
    {
        var depositRequest = await _depositRequestRepository.GetByIdAsync(request.Id, cancellationToken);

        if (depositRequest is null)
        {
            return Result<DepositRequestDto>.NotFound($"Deposit request with ID '{request.Id}' was not found.");
        }

        var dto = new DepositRequestDto
        {
            Id = depositRequest.Id,
            UserId = depositRequest.UserId,
            WalletId = depositRequest.WalletId,
            Amount = depositRequest.Amount,
            PaymentMethod = depositRequest.PaymentMethod,
            Status = depositRequest.Status,
            PaymentOrderCode = depositRequest.PaymentOrderCode,
            PaymentUrl = depositRequest.PaymentUrl,
            Description = depositRequest.Description,
            CreatedAt = depositRequest.CreatedAt,
            CompletedAt = depositRequest.CompletedAt,
            FailureReason = depositRequest.FailureReason
        };

        return Result<DepositRequestDto>.Success(dto);
    }
}
