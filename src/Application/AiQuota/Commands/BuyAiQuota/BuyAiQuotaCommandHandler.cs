using Application.AiQuota.Interfaces;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.AiQuota.Commands.BuyAiQuota;

public class BuyAiQuotaCommandHandler : ICommandHandler<BuyAiQuotaCommand, BuyAiQuotaResponse>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IAiQuotaService _quotaService;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BuyAiQuotaCommandHandler> _logger;

    public BuyAiQuotaCommandHandler(
        IWalletRepository walletRepository,
        IAiQuotaService quotaService,
        ICurrentUserService currentUser,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<BuyAiQuotaCommandHandler> logger)
    {
        _walletRepository = walletRepository;
        _quotaService = quotaService;
        _currentUser = currentUser;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BuyAiQuotaResponse>> Handle(
        BuyAiQuotaCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result<BuyAiQuotaResponse>.Unauthorized("User is not authenticated.");

        var userId = _currentUser.UserId.Value;
        var role = _currentUser.Roles.FirstOrDefault() ?? "Patient";

        // Get current quota info (includes BundlePrice and BundleSize from SystemSettings)
        var currentQuota = await _quotaService.GetQuotaAsync(userId, role, cancellationToken);

        if (currentQuota.BundlePrice is null || currentQuota.BundleSize is null)
            return Result<BuyAiQuotaResponse>.Failure("AI quota bundle is not configured for this role.");

        var totalCost = currentQuota.BundlePrice.Value * request.NumberOfBundles;

        // Get wallet
        var wallet = await _walletRepository.GetByUserIdAsync(userId, cancellationToken);
        if (wallet is null)
            return Result<BuyAiQuotaResponse>.Failure("Wallet not found. Please create a wallet first.");

        // Check sufficient balance
        if (wallet.Balance < totalCost)
            return Result<BuyAiQuotaResponse>.PaymentRequired(
                $"Insufficient wallet balance. Required: {totalCost:N0} VND, Available: {wallet.Balance:N0} VND.");

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Withdraw from wallet
            var description = $"Mua {request.NumberOfBundles * currentQuota.BundleSize.Value} lượt AI screening ({request.NumberOfBundles} gói x {currentQuota.BundlePrice.Value:N0} VND)";
            wallet.Withdraw(totalCost, description);
            await _walletRepository.UpdateAsync(wallet, cancellationToken);

            // Create wallet transactions (one per bundle for audit trail)
            Guid? lastTransactionId = null;
            for (var i = 0; i < request.NumberOfBundles; i++)
            {
                var transaction = new WalletTransaction(
                    wallet.Id,
                    currentQuota.BundlePrice.Value,
                    TransactionType.Payment,
                    description,
                    "AiQuota");

                await _walletRepository.AddTransactionAsync(transaction, cancellationToken);
                lastTransactionId = transaction.Id;
            }

            // Add purchased quota credits to the entity (Patient or Organisation)
            var totalCredits = request.NumberOfBundles * currentQuota.BundleSize.Value;
            await _quotaService.AddPurchasedQuotaAsync(userId, role, totalCredits, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "User {UserId} purchased {Bundles} AI quota bundle(s) for {Amount} VND",
                userId, request.NumberOfBundles, totalCost);

            // Get updated quota
            var updatedQuota = await _quotaService.GetQuotaAsync(userId, role, cancellationToken);

            // Send real-time notification for successful payment [FR-49]
            try
            {
                await _notificationService.SendAsync(
                    userId,
                    "Thanh toán thành công",
                    $"Bạn đã mua {totalCredits} lượt AI screening với giá {totalCost:N0} VND. Số dư còn lại: {wallet.Balance:N0} VND",
                    NotificationType.WalletPaymentProcessed,
                    new { TransactionId = lastTransactionId, Amount = totalCost, Action = "AI Quota Purchase" },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send AI quota purchase notification for user {UserId}",
                    userId);
            }

            return Result<BuyAiQuotaResponse>.Success(new BuyAiQuotaResponse
            {
                TotalAiQuota = updatedQuota.TotalQuota,
                UsedAiQuota = updatedQuota.UsedQuota,
                RemainingQuota = updatedQuota.RemainingQuota,
                WalletBalance = wallet.Balance,
                AmountDeducted = totalCost
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Failed to purchase AI quota for user {UserId}", userId);
            return Result<BuyAiQuotaResponse>.Failure($"Failed to purchase quota: {ex.Message}");
        }
    }
}
