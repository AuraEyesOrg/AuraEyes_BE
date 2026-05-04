namespace Application.SystemAdmin.Dashboard.Queries.GetTransactionStats;

public class TransactionStatsDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Count { get; set; }
}
