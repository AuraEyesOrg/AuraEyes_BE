using Application.AiQuota.Interfaces;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemSettings.Interfaces;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.AiQuota.Commands.BuyAiQuota;

public class BuyAiQuotaCommandHandler : ICommandHandler<BuyAiQuotaCommand, BuyAiQuotaResponse>
{
    private const decimal DefaultUnitPrice = 10000m;
    private const decimal OrganisationUnitPriceRatio = 0.60m;

    private readonly IWalletRepository _walletRepository;
    private readonly IAiQuotaService _quotaService;
    private readonly ISystemSettingService _settingService;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BuyAiQuotaCommandHandler> _logger;

    public BuyAiQuotaCommandHandler(
        IWalletRepository walletRepository,
        IAiQuotaService quotaService,
        ISystemSettingService settingService,
        ICurrentUserService currentUser,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<BuyAiQuotaCommandHandler> logger)
    {
        _walletRepository = walletRepository;
        _quotaService = quotaService;
        _settingService = settingService;
        _currentUser = currentUser;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<BuyAiQuotaResponse>> Handle(
        BuyAiQuotaCommand request, CancellationToken cancellationToken)
    {
        if (request.QuotaAmount <= 0)
            return Result<BuyAiQuotaResponse>.Failure("Quota amount must be greater than 0.");

        if (_currentUser.UserId is null)
            return Result<BuyAiQuotaResponse>.Unauthorized("User is not authenticated.");

        var userId = _currentUser.UserId.Value;
        var role = _currentUser.Roles.FirstOrDefault() ?? Roles.Patient;

        var configuredUnitPrice = await _settingService.GetSettingAsync("AI_QUOTA_UNIT_PRICE", cancellationToken);
        var unitPrice = ResolveUnitPrice(configuredUnitPrice);
        if (IsOrganisationBillingRole(role))
        {
            unitPrice = Math.Round(unitPrice * OrganisationUnitPriceRatio, 0, MidpointRounding.AwayFromZero);
        }
        var totalCredits = request.QuotaAmount;
        var totalCost = Math.Round(unitPrice * totalCredits, 0, MidpointRounding.AwayFromZero);

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
            var description = $"Mua {totalCredits} lượt AI screening ({unitPrice:N0} VND/lượt)";
            wallet.Withdraw(totalCost, description);
            await _walletRepository.UpdateAsync(wallet, cancellationToken);

            // Create wallet transaction for this quota top-up purchase
            var transaction = new WalletTransaction(
                wallet.Id,
                totalCost,
                TransactionType.Payment,
                description,
                "AiQuota");

            await _walletRepository.AddTransactionAsync(transaction, cancellationToken);
            var lastTransactionId = transaction.Id;

            // Add purchased quota credits to the entity (Patient or Organisation)
            await _quotaService.AddPurchasedQuotaAsync(userId, role, totalCredits, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "User {UserId} purchased {QuotaAmount} AI quota credit(s) for {Amount} VND",
                userId, totalCredits, totalCost);

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
                    new
                    {
                        TransactionId = lastTransactionId,
                        Amount = totalCost,
                        QuotaAmount = totalCredits,
                        UnitPrice = unitPrice,
                        Action = "AI Quota Purchase"
                    },
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

    private static decimal ResolveUnitPrice(string? configuredValue)
    {
        if (decimal.TryParse(
                configuredValue,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var parsed)
            && parsed > 0m)
        {
            return parsed;
        }

        return DefaultUnitPrice;
    }

    private static bool IsOrganisationBillingRole(string role)
    {
        return string.Equals(role, Roles.OrgAdmin, StringComparison.OrdinalIgnoreCase);
    }
}
