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

    /// <summary>Price per bundle in VND (for Patient purchase flow).</summary>
    public decimal? BundlePrice { get; init; }

    /// <summary>Credits per bundle (for Patient purchase flow).</summary>
    public int? BundleSize { get; init; }
}
