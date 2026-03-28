using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;

namespace Application.SystemAdmin.Ophthalmologists.Commands.ConfirmWithdrawalRequest;

public class ConfirmWithdrawalRequestCommandHandler : ICommandHandler<ConfirmWithdrawalRequestCommand>
{
    private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmWithdrawalRequestCommandHandler(
        IWithdrawalRequestRepository withdrawalRequestRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _withdrawalRequestRepository = withdrawalRequestRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ConfirmWithdrawalRequestCommand request,
        CancellationToken cancellationToken)
    {
        var withdrawalRequest = await _withdrawalRequestRepository.GetByIdAsync(request.WithdrawalRequestId, cancellationToken);
        if (withdrawalRequest is null)
        {
            return Result.NotFound("Withdrawal request not found.");
        }

        if (withdrawalRequest.Status != PaymentStatus.Pending && withdrawalRequest.Status != PaymentStatus.Processing)
        {
            return Result.Conflict("Only pending withdrawal requests can be confirmed.");
        }

        var wallet = await _walletRepository.GetByIdAsync(withdrawalRequest.WalletId, cancellationToken);
        if (wallet is null)
        {
            return Result.NotFound("Wallet not found.");
        }

        if (wallet.Balance < withdrawalRequest.Amount)
        {
            return Result.Failure("Insufficient wallet balance at confirmation time.");
        }

        wallet.Withdraw(
            withdrawalRequest.Amount,
            $"Withdrawal payout to {withdrawalRequest.BankAccountNumber} ({withdrawalRequest.BankName})");

        var transaction = new WalletTransaction(
            wallet.Id,
            withdrawalRequest.Amount,
            TransactionType.Withdrawal,
            $"Rút tiền về {withdrawalRequest.BankName} - {withdrawalRequest.BankAccountNumber}",
            "WithdrawalRequest",
            withdrawalRequest.Id);

        wallet.AddTransaction(transaction);
        await _walletRepository.AddTransactionAsync(transaction, cancellationToken);

        withdrawalRequest.MarkCompleted(request.AdminUserId, request.TransferReference, request.Note);

        await _walletRepository.UpdateAsync(wallet, cancellationToken);
        await _withdrawalRequestRepository.UpdateAsync(withdrawalRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
