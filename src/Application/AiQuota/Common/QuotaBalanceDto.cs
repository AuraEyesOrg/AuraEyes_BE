namespace Application.AiQuota.Common;

public record QuotaBalanceDto
{
    public int TotalAiQuota { get; init; }
    public int UsedAiQuota { get; init; }
    public int RemainingQuota { get; init; }
    public string QuotaSource { get; init; } = string.Empty;
    public decimal? BundlePrice { get; init; }
    public int? BundleSize { get; init; }
}
