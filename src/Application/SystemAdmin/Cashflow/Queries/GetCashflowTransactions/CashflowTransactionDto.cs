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
}
