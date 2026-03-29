using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Repositories;

namespace Application.Wallets.Commands.CreateWithdrawalRequest;

public class CreateWithdrawalRequestCommandHandler : ICommandHandler<CreateWithdrawalRequestCommand, WithdrawalRequestDto>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWithdrawalRequestCommandHandler(
        IWalletRepository walletRepository,
        IWithdrawalRequestRepository withdrawalRequestRepository,
        IContractRepository contractRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _withdrawalRequestRepository = withdrawalRequestRepository;
        _contractRepository = contractRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WithdrawalRequestDto>> Handle(
        CreateWithdrawalRequestCommand request,
        CancellationToken cancellationToken)
    {
        var isOphthalmologist = await _identityService.IsInRoleAsync(request.UserId, Roles.Ophthalmologist);
        if (!isOphthalmologist)
        {
            return Result<WithdrawalRequestDto>.Forbidden("Only ophthalmologists can create withdrawal requests.");
        }

        var hasPending = await _withdrawalRequestRepository.HasPendingRequestAsync(request.UserId, cancellationToken);
        if (hasPending)
        {
            return Result<WithdrawalRequestDto>.Conflict("You already have a pending withdrawal request.");
        }

        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (wallet is null)
        {
            return Result<WithdrawalRequestDto>.NotFound("Wallet not found.");
        }

        if (!string.Equals(wallet.OwnerType, "Ophthalmologist", StringComparison.OrdinalIgnoreCase))
        {
            // Backward compatibility for legacy wallets created before role-based owner type assignment.
            wallet.SetOwnerType("Ophthalmologist");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        if (wallet.Balance < request.AmountVnd)
        {
            return Result<WithdrawalRequestDto>.Failure("Insufficient wallet balance for this withdrawal request.");
        }

        var contract = await _contractRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var contractNumber = request.ContractNumber ?? contract?.ContractNumber;

        var withdrawalRequest = new WithdrawalRequest(
            request.UserId,
            wallet.Id,
            request.AmountVnd,
            request.BankName,
            request.BankAccountNumber,
            request.AccountHolderName,
            contractNumber,
            request.Note);

        await _withdrawalRequestRepository.AddAsync(withdrawalRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new WithdrawalRequestDto
        {
            Id = withdrawalRequest.Id,
            UserId = withdrawalRequest.UserId,
            WalletId = withdrawalRequest.WalletId,
            Amount = withdrawalRequest.Amount,
            Status = withdrawalRequest.Status,
            BankName = withdrawalRequest.BankName,
            BankAccountNumber = withdrawalRequest.BankAccountNumber,
            AccountHolderName = withdrawalRequest.AccountHolderName,
            ContractNumber = withdrawalRequest.ContractNumber,
            Note = withdrawalRequest.Note,
            AdminNote = withdrawalRequest.AdminNote,
            TransferReference = withdrawalRequest.TransferReference,
            ProcessedByAdminId = withdrawalRequest.ProcessedByAdminId,
            ProcessedAt = withdrawalRequest.ProcessedAt,
            CreatedAt = withdrawalRequest.CreatedAt
        };

        return Result<WithdrawalRequestDto>.Success(dto);
    }
}
