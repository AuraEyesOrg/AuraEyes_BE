using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Entities.Financial;
using Domain.Repositories;
using Domain.Common;

namespace Application.Wallets.Queries.GetWallet;

/// <summary>
/// Handler for GetWalletQuery.
/// </summary>
public class GetWalletQueryHandler : IQueryHandler<GetWalletQuery, WalletDto>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetWalletQueryHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WalletDto>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        // If wallet doesn't exist, create one
        if (wallet is null)
        {
            wallet = new Wallet(request.UserId, 0);
            await _walletRepository.AddAsync(wallet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var dto = new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            Balance = wallet.Balance,
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt
        };

        return Result<WalletDto>.Success(dto);
    }
}
