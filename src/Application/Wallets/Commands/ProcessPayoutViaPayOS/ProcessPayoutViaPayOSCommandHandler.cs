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

        // 1. Ước tính phí (Estimate Credit) từ PayOS trước khi chi
        long estimatedFee = 0;
        try
        {
            var payoutItems = new List<PayOSPayoutItem>
            {
                new PayOSPayoutItem
                {
                    ReferenceId = referenceId,
                    Amount = (long)withdrawalRequest.Amount,
                    Description = $"Estimate for {withdrawalRequest.Id}",
                    ToBin = withdrawalRequest.BankBin,
                    ToAccountNumber = withdrawalRequest.BankAccountNumber
                }
            };

            estimatedFee = await _payOSPayoutService.EstimateCreditAsync(
                referenceId: $"est_{referenceId}",
                categories: request.Categories,
                payouts: payoutItems,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<PayoutViaPayOSResponse>.Failure(
                $"PayOS estimate credit failed: {ex.Message}. Vui lòng thử lại sau.");
        }

        // 2. Tính số tiền thực nhận sau khi trừ phí
        var netPayoutAmount = withdrawalRequest.Amount - estimatedFee;
        if (netPayoutAmount <= 0)
        {
            return Result<PayoutViaPayOSResponse>.Failure(
                $"Số tiền yêu cầu ({withdrawalRequest.Amount:N0} VND) không đủ để trả phí PayOS ({estimatedFee:N0} VND).");
        }

        // Mô tả thanh toán (PayOS giới hạn 25 ký tự)
        var description = $"Rut tien AuraEyes {withdrawalRequest.Amount:N0}";

        PayOSPayoutResult payoutResult;
        try
        {
            payoutResult = await _payOSPayoutService.CreatePayoutAsync(
                referenceId: referenceId,
                amountVnd: netPayoutAmount, // Chi số tiền đã trừ phí
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

        // Cập nhật entity kèm phí đã thu
        withdrawalRequest.SetPayOSPayout(
            payOSReferenceId: referenceId,
            externalPayoutId: payoutResult.Id,
            approvalState: payoutResult.ApprovalState,
            transactionId: firstTxn?.Id,
            fee: estimatedFee);

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
