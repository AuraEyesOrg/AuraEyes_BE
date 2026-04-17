namespace Application.AiQuota.Common;

public record QuotaBalanceDto
{
    public int TotalAiQuota { get; init; }
    public int UsedAiQuota { get; init; }
    public int RemainingQuota { get; init; }
    public string QuotaSource { get; init; } = string.Empty;
    public decimal? UnitPrice { get; init; }

    public int? FreeQuotaLimit { get; init; }
    public int? FreeQuotaUsed { get; init; }
    public int? FreeQuotaRemaining { get; init; }

    public int? MonthlyQuotaLimit { get; init; }
    public int? MonthlyQuotaUsed { get; init; }
    public int? MonthlyQuotaRemaining { get; init; }

    public int? PurchasedQuota { get; init; }
}
