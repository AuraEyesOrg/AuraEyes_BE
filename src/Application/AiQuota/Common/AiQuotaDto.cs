namespace Application.AiQuota.Common;

/// <summary>
/// AI Quota information DTO.
/// </summary>
public record AiQuotaDto
{
    public int TotalQuota { get; init; }
    public int UsedQuota { get; init; }
    public int RemainingQuota { get; init; }

    /// <summary>"Free" | "Purchased" | "Contract"</summary>
    public string QuotaSource { get; init; } = string.Empty;

    /// <summary>Price per 1 quota credit in VND.</summary>
    public decimal? UnitPrice { get; init; }

    public int? FreeQuotaLimit { get; init; }
    public int? FreeQuotaUsed { get; init; }
    public int? FreeQuotaRemaining { get; init; }

    public int? MonthlyQuotaLimit { get; init; }
    public int? MonthlyQuotaUsed { get; init; }
    public int? MonthlyQuotaRemaining { get; init; }

    public int? PurchasedQuota { get; init; }
}
