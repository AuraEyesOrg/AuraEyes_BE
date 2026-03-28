using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Repositories;

namespace Application.Wallets.Queries.GetWithdrawalHistory;

public class GetWithdrawalHistoryQueryHandler : IQueryHandler<GetWithdrawalHistoryQuery, PagedResult<WithdrawalRequestDto>>
{
    private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;

    public GetWithdrawalHistoryQueryHandler(IWithdrawalRequestRepository withdrawalRequestRepository)
    {
        _withdrawalRequestRepository = withdrawalRequestRepository;
    }

    public async Task<Result<PagedResult<WithdrawalRequestDto>>> Handle(
        GetWithdrawalHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _withdrawalRequestRepository.GetByUserIdPagedAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(x => new WithdrawalRequestDto
        {
            Id = x.Id,
            UserId = x.UserId,
            WalletId = x.WalletId,
            Amount = x.Amount,
            Status = x.Status,
            BankName = x.BankName,
            BankAccountNumber = x.BankAccountNumber,
            AccountHolderName = x.AccountHolderName,
            ContractNumber = x.ContractNumber,
            Note = x.Note,
            AdminNote = x.AdminNote,
            TransferReference = x.TransferReference,
            ProcessedByAdminId = x.ProcessedByAdminId,
            ProcessedAt = x.ProcessedAt,
            CreatedAt = x.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<WithdrawalRequestDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<WithdrawalRequestDto>>.Success(pagedResult);
    }
}
