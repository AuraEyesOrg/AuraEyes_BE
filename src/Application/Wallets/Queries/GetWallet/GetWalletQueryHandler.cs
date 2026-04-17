using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Constants;
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
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public GetWalletQueryHandler(
        IWalletRepository walletRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WalletDto>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        // If wallet doesn't exist, create one
        if (wallet is null)
        {
            var roles = await _identityService.GetUserRolesAsync(request.UserId);
            var ownerType = roles.Contains(Roles.Ophthalmologist)
                ? "Ophthalmologist"
                : roles.Contains(Roles.OrgAdmin)
                    ? "Organisation"
                    : Roles.Patient;

            wallet = new Wallet(request.UserId, ownerType, 0);
            await _walletRepository.AddAsync(wallet, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var now = DateTime.UtcNow;
        var stats = await _walletRepository.GetMonthlyStatsAsync(wallet.Id, now.Year, now.Month, cancellationToken);

        var dto = new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            Balance = wallet.Balance,
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt,
            TotalDepositsThisMonth = stats.TotalDeposits,
            TotalSpentThisMonth = stats.TotalSpent,
            TransactionsThisMonth = stats.TransactionsCount
        };

        return Result<WalletDto>.Success(dto);
    }
}
