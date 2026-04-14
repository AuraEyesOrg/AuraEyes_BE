namespace Application.SystemAdmin.Cashflow.Queries.GetCashflowTransactions;

public class CashflowTransactionDto
{
    public Guid Id { get; init; }
    public string ActorName { get; init; } = string.Empty;
    public string? ActorEmail { get; init; }
    public string ActorRole { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string TransactionType { get; init; } = string.Empty;
    public string? ReferenceType { get; init; }
    public Guid? ReferenceId { get; init; }
    public string? BookingCode { get; init; }
    public string Status { get; init; } = "Completed";
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }

    // Withdrawal details (when ReferenceType == WithdrawalRequest)
    public string? WithdrawalBankName { get; init; }
    public string? WithdrawalBankAccountNumber { get; init; }
    public string? WithdrawalAccountHolderName { get; init; }
    public string? WithdrawalBankBin { get; init; }
    public string? WithdrawalTransferReference { get; init; }
    public Guid? WithdrawalProcessedByAdminId { get; init; }
    public DateTime? WithdrawalProcessedAt { get; init; }
    public string? WithdrawalExternalPayoutId { get; init; }
    public string? WithdrawalPayOSReferenceId { get; init; }
    public string? WithdrawalPayOSTransactionId { get; init; }
    public string? WithdrawalPayOSApprovalState { get; init; }
    public decimal? WithdrawalFee { get; init; }

    // Deposit details (when ReferenceType == DepositRequest or resolved by OrderCode)
    public string? DepositOrderCode { get; init; }
    public string? DepositPaymentMethod { get; init; }
    public string? DepositPaymentUrl { get; init; }
    public string? DepositProviderTxnRef { get; init; }
    public string? DepositProviderResponse { get; init; }
    public string? DepositReturnUrl { get; init; }
    public string? DepositCancelUrl { get; init; }
    public string? DepositFailureReason { get; init; }
    public DateTime? DepositCompletedAt { get; init; }
}
