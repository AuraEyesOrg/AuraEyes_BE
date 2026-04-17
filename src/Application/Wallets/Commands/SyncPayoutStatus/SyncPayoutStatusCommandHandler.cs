using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Wallets.Commands.SyncPayoutStatus;

/// <summary>
/// Handler đồng bộ trạng thái lệnh chi từ PayOS về hệ thống.
/// Flow:
///   1. Validate admin + WithdrawalRequest tồn tại và có ExternalPayoutId
///   2. Gọi PayOS GET /v1/payouts/{payoutId}
///   3. Cập nhật ApprovalState vào entity
///   4. Nếu SUCCEEDED và chưa Completed → trừ ví + mark Completed
///   5. Nếu FAILED → mark Failed
/// </summary>
public class SyncPayoutStatusCommandHandler
    : ICommandHandler<SyncPayoutStatusCommand, PayoutStatusResponse>
{
    private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IPayOSPayoutService _payOSPayoutService;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public SyncPayoutStatusCommandHandler(
        IWithdrawalRequestRepository withdrawalRequestRepository,
        IWalletRepository walletRepository,
        IPayOSPayoutService payOSPayoutService,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _withdrawalRequestRepository = withdrawalRequestRepository;
        _walletRepository = walletRepository;
        _payOSPayoutService = payOSPayoutService;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PayoutStatusResponse>> Handle(
        SyncPayoutStatusCommand request,
        CancellationToken cancellationToken)
    {
        var isAdmin = await _identityService.IsInRoleAsync(request.RequestedByUserId, Roles.SystemAdmin);
        if (!isAdmin)
            return Result<PayoutStatusResponse>.Forbidden("Only system admins can sync payout status.");

        var withdrawalRequest = await _withdrawalRequestRepository.GetByIdAsync(
            request.WithdrawalRequestId, cancellationToken);

        if (withdrawalRequest is null)
            return Result<PayoutStatusResponse>.NotFound("Withdrawal request not found.");

        if (string.IsNullOrWhiteSpace(withdrawalRequest.ExternalPayoutId))
            return Result<PayoutStatusResponse>.Failure(
                "This withdrawal request has not been submitted to PayOS yet. No ExternalPayoutId found.");

        // Chỉ cần sync nếu đang ở trạng thái Processing
        if (withdrawalRequest.Status != PaymentStatus.Processing)
            return Result<PayoutStatusResponse>.Failure(
                $"Payout sync is only applicable for Processing requests (current: {withdrawalRequest.Status}).");

        PayOSPayoutResult payoutResult;
        try
        {
            payoutResult = await _payOSPayoutService.GetPayoutAsync(
                withdrawalRequest.ExternalPayoutId, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<PayoutStatusResponse>.Failure(
                $"Failed to query payout status from PayOS: {ex.Message}");
        }

        var firstTxn = payoutResult.Transactions.FirstOrDefault();

        var isCompleted =
            payoutResult.ApprovalState.Equals("SUCCEEDED", StringComparison.OrdinalIgnoreCase)
            || payoutResult.ApprovalState.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase);

        // Nếu payout đã hoàn tất và wallet chưa bị trừ
        if (isCompleted && withdrawalRequest.Status != PaymentStatus.Completed)
        {
            var wallet = await _walletRepository.GetByIdAsync(withdrawalRequest.WalletId, cancellationToken);
            if (wallet != null)
            {
                wallet.Withdraw(
                    withdrawalRequest.Amount,
                    $"Payout via PayOS: {withdrawalRequest.PayOSReferenceId}");

                var transaction = new WalletTransaction(
                    wallet.Id,
                    withdrawalRequest.Amount,
                    TransactionType.Withdrawal,
                    $"Rút tiền về {withdrawalRequest.BankName} - {withdrawalRequest.BankAccountNumber}",
                    "WithdrawalRequest",
                    withdrawalRequest.Id);

                await _walletRepository.AddTransactionAsync(transaction, cancellationToken);
            }
        }

        withdrawalRequest.UpdatePayOSApprovalState(
            payoutResult.ApprovalState,
            firstTxn?.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new PayoutStatusResponse
        {
            WithdrawalRequestId = withdrawalRequest.Id,
            ExternalPayoutId = withdrawalRequest.ExternalPayoutId,
            PayOSReferenceId = withdrawalRequest.PayOSReferenceId ?? string.Empty,
            ApprovalState = payoutResult.ApprovalState,
            WithdrawalStatus = withdrawalRequest.Status.ToString(),
            Transactions = payoutResult.Transactions.Select(t => new PayOSPayoutTransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Description = t.Description,
                ToBin = t.ToBin,
                ToAccountNumber = t.ToAccountNumber,
                ToAccountName = t.ToAccountName,
                State = t.State
            }).ToList()
        };

        return Result<PayoutStatusResponse>.Success(response);
    }
}
