using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;

namespace Application.SystemAdmin.Cashflow.Queries.GetCashflowTransactions;

public class GetCashflowTransactionsQueryHandler : IQueryHandler<GetCashflowTransactionsQuery, PagedResult<CashflowTransactionDto>>
{
    private static readonly HashSet<string> SupportedActorRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Patient",
        "Ophthalmologist",
        "Organisation",
        "System"
    };

    private readonly IWalletRepository _walletRepository;
    private readonly IDepositRequestRepository _depositRequestRepository;
    private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
    private readonly IIdentityService _identityService;

    public GetCashflowTransactionsQueryHandler(
        IWalletRepository walletRepository,
        IDepositRequestRepository depositRequestRepository,
        IWithdrawalRequestRepository withdrawalRequestRepository,
        IIdentityService identityService)
    {
        _walletRepository = walletRepository;
        _depositRequestRepository = depositRequestRepository;
        _withdrawalRequestRepository = withdrawalRequestRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<CashflowTransactionDto>>> Handle(
        GetCashflowTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 200);

        var statusFilter = request.Status?.Trim();

        var normalizedActorRole = NormalizeActorRole(request.ActorRole);

        var (items, totalCount) = await _walletRepository.GetCashflowTransactionsPagedAsync(
            normalizedActorRole,
            request.SearchTerm,
            request.SortBy,
            request.SortDirection,
            pageNumber,
            pageSize,
            cancellationToken);

        var userCache = new Dictionary<Guid, UserDto?>();
        var withdrawalCache = new Dictionary<Guid, WithdrawalRequest?>();
        var depositByIdCache = new Dictionary<Guid, DepositRequest?>();
        var depositByOrderCodeCache = new Dictionary<string, DepositRequest?>(StringComparer.OrdinalIgnoreCase);
        var rows = new List<CashflowTransactionDto>(items.Count);

        foreach (var (transaction, wallet) in items)
        {
            if (!userCache.TryGetValue(wallet.UserId, out var user))
            {
                user = await _identityService.GetUserByIdAsync(wallet.UserId, cancellationToken);
                userCache[wallet.UserId] = user;
            }

            var actorName = user?.FullName;
            if (string.IsNullOrWhiteSpace(actorName))
            {
                actorName = wallet.OwnerType.Equals("System", StringComparison.OrdinalIgnoreCase)
                    ? "System Wallet"
                    : "Unknown";
            }

            var transactionStatus = "Completed";

            string? withdrawalBankName = null;
            string? withdrawalBankAccountNumber = null;
            string? withdrawalAccountHolderName = null;
            string? withdrawalBankBin = null;
            string? withdrawalTransferReference = null;
            Guid? withdrawalProcessedByAdminId = null;
            DateTime? withdrawalProcessedAt = null;
            string? withdrawalExternalPayoutId = null;
            string? withdrawalPayOSReferenceId = null;
            string? withdrawalPayOSTransactionId = null;
            string? withdrawalPayOSApprovalState = null;
            decimal? withdrawalFee = null;

            string? depositOrderCode = null;
            string? depositPaymentMethod = null;
            string? depositPaymentUrl = null;
            string? depositProviderTxnRef = null;
            string? depositProviderResponse = null;
            string? depositReturnUrl = null;
            string? depositCancelUrl = null;
            string? depositFailureReason = null;
            DateTime? depositCompletedAt = null;

            if (transaction.ReferenceId.HasValue
                && string.Equals(transaction.ReferenceType, "WithdrawalRequest", StringComparison.OrdinalIgnoreCase))
            {
                if (!withdrawalCache.TryGetValue(transaction.ReferenceId.Value, out var withdrawalRequest))
                {
                    withdrawalRequest = await _withdrawalRequestRepository.GetByIdAsync(
                        transaction.ReferenceId.Value,
                        cancellationToken);
                    withdrawalCache[transaction.ReferenceId.Value] = withdrawalRequest;
                }

                if (withdrawalRequest is not null)
                {
                    transactionStatus = withdrawalRequest.Status.ToString();
                    withdrawalBankName = withdrawalRequest.BankName;
                    withdrawalBankAccountNumber = withdrawalRequest.BankAccountNumber;
                    withdrawalAccountHolderName = withdrawalRequest.AccountHolderName;
                    withdrawalBankBin = withdrawalRequest.BankBin;
                    withdrawalTransferReference = withdrawalRequest.TransferReference;
                    withdrawalProcessedByAdminId = withdrawalRequest.ProcessedByAdminId;
                    withdrawalProcessedAt = withdrawalRequest.ProcessedAt;
                    withdrawalExternalPayoutId = withdrawalRequest.ExternalPayoutId;
                    withdrawalPayOSReferenceId = withdrawalRequest.PayOSReferenceId;
                    withdrawalPayOSTransactionId = withdrawalRequest.PayOSTransactionId;
                    withdrawalPayOSApprovalState = withdrawalRequest.PayOSApprovalState;
                    withdrawalFee = withdrawalRequest.Fee;
                }
            }

            if (transaction.ReferenceId.HasValue
                && string.Equals(transaction.ReferenceType, "DepositRequest", StringComparison.OrdinalIgnoreCase))
            {
                if (!depositByIdCache.TryGetValue(transaction.ReferenceId.Value, out var depositRequest))
                {
                    depositRequest = await _depositRequestRepository.GetByIdAsync(
                        transaction.ReferenceId.Value,
                        cancellationToken);
                    depositByIdCache[transaction.ReferenceId.Value] = depositRequest;
                }

                if (depositRequest is not null)
                {
                    transactionStatus = depositRequest.Status.ToString();
                    depositOrderCode = depositRequest.PaymentOrderCode;
                    depositPaymentMethod = depositRequest.PaymentMethod.ToString();
                    depositPaymentUrl = depositRequest.PaymentUrl;
                    depositProviderTxnRef = depositRequest.ProviderTxnRef;
                    depositProviderResponse = depositRequest.ProviderResponse;
                    depositReturnUrl = depositRequest.ReturnUrl;
                    depositCancelUrl = depositRequest.CancelUrl;
                    depositFailureReason = depositRequest.FailureReason;
                    depositCompletedAt = depositRequest.CompletedAt;
                }
            }
            else if (transaction.TransactionType == TransactionType.Deposit
                     && TryExtractOrderCode(transaction.Description, out var orderCode))
            {
                if (!depositByOrderCodeCache.TryGetValue(orderCode, out var depositRequest))
                {
                    depositRequest = await _depositRequestRepository.GetByOrderCodeAsync(orderCode, cancellationToken);
                    depositByOrderCodeCache[orderCode] = depositRequest;
                }

                if (depositRequest is not null)
                {
                    transactionStatus = depositRequest.Status.ToString();
                    depositOrderCode = depositRequest.PaymentOrderCode;
                    depositPaymentMethod = depositRequest.PaymentMethod.ToString();
                    depositPaymentUrl = depositRequest.PaymentUrl;
                    depositProviderTxnRef = depositRequest.ProviderTxnRef;
                    depositProviderResponse = depositRequest.ProviderResponse;
                    depositReturnUrl = depositRequest.ReturnUrl;
                    depositCancelUrl = depositRequest.CancelUrl;
                    depositFailureReason = depositRequest.FailureReason;
                    depositCompletedAt = depositRequest.CompletedAt;
                }
            }

            if (!string.IsNullOrWhiteSpace(statusFilter)
                && !transactionStatus.Equals(statusFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            rows.Add(new CashflowTransactionDto
            {
                Id = transaction.Id,
                ActorName = actorName,
                ActorEmail = user?.Email,
                ActorRole = wallet.OwnerType,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType.ToString(),
                ReferenceType = transaction.ReferenceType,
                ReferenceId = transaction.ReferenceId,
                BookingCode = transaction.ReferenceType?.Equals("Booking", StringComparison.OrdinalIgnoreCase) == true
                    ? transaction.ReferenceId?.ToString()
                    : null,
                Status = transactionStatus,
                Description = transaction.Description,
                CreatedAt = transaction.CreatedAt,
                WithdrawalBankName = withdrawalBankName,
                WithdrawalBankAccountNumber = withdrawalBankAccountNumber,
                WithdrawalAccountHolderName = withdrawalAccountHolderName,
                WithdrawalBankBin = withdrawalBankBin,
                WithdrawalTransferReference = withdrawalTransferReference,
                WithdrawalProcessedByAdminId = withdrawalProcessedByAdminId,
                WithdrawalProcessedAt = withdrawalProcessedAt,
                WithdrawalExternalPayoutId = withdrawalExternalPayoutId,
                WithdrawalPayOSReferenceId = withdrawalPayOSReferenceId,
                WithdrawalPayOSTransactionId = withdrawalPayOSTransactionId,
                WithdrawalPayOSApprovalState = withdrawalPayOSApprovalState,
                WithdrawalFee = withdrawalFee,
                DepositOrderCode = depositOrderCode,
                DepositPaymentMethod = depositPaymentMethod,
                DepositPaymentUrl = depositPaymentUrl,
                DepositProviderTxnRef = depositProviderTxnRef,
                DepositProviderResponse = depositProviderResponse,
                DepositReturnUrl = depositReturnUrl,
                DepositCancelUrl = depositCancelUrl,
                DepositFailureReason = depositFailureReason,
                DepositCompletedAt = depositCompletedAt,
            });
        }

        var pagedResult = new PagedResult<CashflowTransactionDto>(
            rows,
            string.IsNullOrWhiteSpace(statusFilter) ? totalCount : rows.Count,
            pageNumber,
            pageSize);

        return Result<PagedResult<CashflowTransactionDto>>.Success(pagedResult);
    }

    private static string? NormalizeActorRole(string? actorRole)
    {
        if (string.IsNullOrWhiteSpace(actorRole))
        {
            return null;
        }

        var trimmed = actorRole.Trim();
        if (!SupportedActorRoles.Contains(trimmed))
        {
            return null;
        }

        return SupportedActorRoles.First(x => x.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
    }

    private static bool TryExtractOrderCode(string? description, out string orderCode)
    {
        orderCode = string.Empty;
        if (string.IsNullOrWhiteSpace(description))
        {
            return false;
        }

        const string marker = "Order:";
        var markerIndex = description.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
        {
            return false;
        }

        var tail = description[(markerIndex + marker.Length)..].Trim();
        if (string.IsNullOrWhiteSpace(tail))
        {
            return false;
        }

        orderCode = tail.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
        return !string.IsNullOrWhiteSpace(orderCode);
    }
}
