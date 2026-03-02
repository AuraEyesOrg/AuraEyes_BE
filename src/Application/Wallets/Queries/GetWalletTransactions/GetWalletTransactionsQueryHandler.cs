using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Repositories;

namespace Application.Wallets.Queries.GetWalletTransactions;

/// <summary>
/// Handler for GetWalletTransactionsQuery.
/// </summary>
public class GetWalletTransactionsQueryHandler : IQueryHandler<GetWalletTransactionsQuery, PagedResult<WalletTransactionDto>>
{
    private readonly IWalletRepository _walletRepository;

    public GetWalletTransactionsQueryHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<Result<PagedResult<WalletTransactionDto>>> Handle(
        GetWalletTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (wallet is null)
        {
            return Result<PagedResult<WalletTransactionDto>>.NotFound("Wallet not found for this user.");
        }

        var (items, totalCount) = await _walletRepository.GetTransactionsPagedAsync(
            wallet.Id,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(t => new WalletTransactionDto
        {
            Id = t.Id,
            WalletId = t.WalletId,
            Amount = t.Amount,
            TransactionType = t.TransactionType,
            Description = t.Description,
            CreatedAt = t.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<WalletTransactionDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<WalletTransactionDto>>.Success(pagedResult);
    }
}
