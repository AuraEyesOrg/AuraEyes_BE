namespace Application.OrganisationScreenings;

/// <summary>List row for /organisations/screenings history table.</summary>
public sealed record OrgScreeningHistoryItemDto
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public int ImagesCount { get; init; }
    public string? LatestRiskLevel { get; init; }
    public decimal? ConfidenceScore { get; init; }
    public string? AiPrimaryLabel { get; init; }
    /// <summary>pending | completed | saved</summary>
    public string Status { get; init; } = "pending";
}

/// <summary>Billing summary for a month.</summary>
public sealed record OrgBillingSummaryDto
{
    public int TotalScreeningsThisMonth { get; init; }
    public int TotalScreeningsAllTime { get; init; }
    public decimal WalletBalance { get; init; }
    public int RemainingQuota { get; init; }
    public int MonthlyQuotaLimit { get; init; }
    public int MonthlyQuotaUsed { get; init; }
    public int MonthlyQuotaRemaining { get; init; }
    public int UsedQuotaToday { get; init; }
    public int PurchasedQuota { get; init; }
    public decimal PatientUnitPrice { get; init; }
    public decimal OrganisationUnitPrice { get; init; }
}

/// <summary>Individual quota transaction record.</summary>
public sealed record OrgBillingTransactionDto
{
    public Guid ScreeningId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public int CreditsUsed { get; init; } = 1;
}

/// <summary>Aggregate screening reports for organisation.</summary>
public sealed record OrgScreeningReportDto
{
    public int TotalScreenings { get; init; }
    public int HighRiskCount { get; init; }
    public int ModerateRiskCount { get; init; }
    public int LowRiskCount { get; init; }
    public decimal AverageConfidence { get; init; }
    public List<OrgMonthlyScreeningCount> MonthlyBreakdown { get; init; } = new();
}

public sealed record OrgMonthlyScreeningCount
{
    public string Month { get; init; } = string.Empty; // "2026-01"
    public int Count { get; init; }
    public int HighRisk { get; init; }
    public int ModerateRisk { get; init; }
    public int LowRisk { get; init; }
}
