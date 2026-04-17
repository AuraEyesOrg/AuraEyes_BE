using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Wallets.Commands.ProcessPayoutViaPayOS;

/// <summary>
/// Xử lý lệnh chi qua PayOS Payout API.
/// Chỉ SystemAdmin mới có thể trigger; ophthalmologist chỉ tạo WithdrawalRequest.
/// Flow:
///   1. Validate request tồn tại và ở trạng thái Pending
///   2. Gọi PayOS Payout API
///   3. Lưu thông tin PayOS vào WithdrawalRequest (SetPayOSPayout)
///   4. Nếu PayOS trả SUCCEEDED ngay → mark Completed + deduct wallet
///   5. Nếu PROCESSING → giữ trạng thái Processing, chờ webhook/poll
/// </summary>
public class ProcessPayoutViaPayOSCommandHandler
    : ICommandHandler<ProcessPayoutViaPayOSCommand, PayoutViaPayOSResponse>
{
    private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IPayOSPayoutService _payOSPayoutService;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPayoutViaPayOSCommandHandler(
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

    public async Task<Result<PayoutViaPayOSResponse>> Handle(
        ProcessPayoutViaPayOSCommand request,
        CancellationToken cancellationToken)
    {
        // Chỉ admin mới được trigger lệnh chi
        var isAdmin = await _identityService.IsInRoleAsync(request.RequestedByUserId, Roles.SystemAdmin);
        if (!isAdmin)
            return Result<PayoutViaPayOSResponse>.Forbidden("Only system admins can process payouts via PayOS.");

        // Tìm WithdrawalRequest
        var withdrawalRequest = await _withdrawalRequestRepository.GetByIdAsync(
            request.WithdrawalRequestId, cancellationToken);

        if (withdrawalRequest is null)
            return Result<PayoutViaPayOSResponse>.NotFound("Withdrawal request not found.");

        if (withdrawalRequest.Status != PaymentStatus.Pending)
            return Result<PayoutViaPayOSResponse>.Failure(
                $"Withdrawal request is not in Pending status (current: {withdrawalRequest.Status}).");

        if (string.IsNullOrWhiteSpace(withdrawalRequest.BankBin))
            return Result<PayoutViaPayOSResponse>.Failure(
                "BankBin is required for PayOS payout. Please update the withdrawal request with a valid BankBin.");

        // Sinh referenceId duy nhất: payout_{withdrawalRequestId}
        var referenceId = $"payout_{withdrawalRequest.Id:N}";

        // Mô tả thanh toán
        var description = $"Thanh toan bac si AuraEyes";

        PayOSPayoutResult payoutResult;
        try
        {
            payoutResult = await _payOSPayoutService.CreatePayoutAsync(
                referenceId: referenceId,
                amountVnd: withdrawalRequest.Amount,
                description: description,
                toBin: withdrawalRequest.BankBin,
                toAccountNumber: withdrawalRequest.BankAccountNumber,
                categories: request.Categories,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<PayoutViaPayOSResponse>.Failure(
                $"PayOS payout creation failed: {ex.Message}");
        }

        // Lấy transaction đầu tiên nếu có
        var firstTxn = payoutResult.Transactions.FirstOrDefault();

        // Cập nhật entity
        withdrawalRequest.SetPayOSPayout(
            payOSReferenceId: referenceId,
            externalPayoutId: payoutResult.Id,
            approvalState: payoutResult.ApprovalState,
            transactionId: firstTxn?.Id,
            fee: null);

        // PayOS trả approvalState ở cấp batch: "COMPLETED" | "PROCESSING" | "FAILED"
        // Từng transaction bên trong có state: "SUCCEEDED" | "PROCESSING" | "FAILED"
        // Coi cả "COMPLETED" (batch done) lẫn "SUCCEEDED" (compat) là hoàn thành ngay
        var isCompletedImmediately =
            payoutResult.ApprovalState.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)
            || payoutResult.ApprovalState.Equals("SUCCEEDED", StringComparison.OrdinalIgnoreCase)
            || (firstTxn?.State.Equals("SUCCEEDED", StringComparison.OrdinalIgnoreCase) ?? false);

        if (isCompletedImmediately)
        {
            var wallet = await _walletRepository.GetByIdAsync(withdrawalRequest.WalletId, cancellationToken);
            if (wallet != null)
            {
                wallet.Withdraw(withdrawalRequest.Amount, $"Payout via PayOS: {referenceId}");

                var transaction = new WalletTransaction(
                    wallet.Id,
                    withdrawalRequest.Amount,
                    TransactionType.Withdrawal,
                    $"Rút tiền về {withdrawalRequest.BankName} - {withdrawalRequest.BankAccountNumber}",
                    "WithdrawalRequest",
                    withdrawalRequest.Id);

                await _walletRepository.AddTransactionAsync(transaction, cancellationToken);
            }
            withdrawalRequest.UpdatePayOSApprovalState("COMPLETED", firstTxn?.Id);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new PayoutViaPayOSResponse
        {
            WithdrawalRequestId = withdrawalRequest.Id,
            ExternalPayoutId = payoutResult.Id,
            PayOSReferenceId = referenceId,
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

        return Result<PayoutViaPayOSResponse>.Success(response);
    }
}
