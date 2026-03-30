namespace Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;

/// <summary>
/// DTO for system admin growth KPIs and chart datasets.
/// </summary>
public class DashboardMetricsDto
{
    public UserGrowthMetricDto Doctors { get; set; } = new();
    public UserGrowthMetricDto Organisations { get; set; } = new();
    public UserGrowthMetricDto Patients { get; set; } = new();
    public List<PaymentMethodRevenueDto> PaymentMethodBreakdown { get; set; } = new();
    public List<MonthlyRevenuePointDto> MonthlyRevenue { get; set; } = new();
    public List<DailyRevenuePointDto> DailyRevenue { get; set; } = new();

    /// <summary>Sum of completed patient top-ups (deposit requests) in the current calendar year.</summary>
    public decimal TotalDepositRevenueYear { get; set; }

    /// <summary>Platform share from consultations credited to the System wallet in the current calendar year.</summary>
    public decimal TotalPlatformCommissionYear { get; set; }

    public List<MonthlyRevenuePointDto> MonthlyPlatformCommission { get; set; } = new();
    public List<DailyRevenuePointDto> DailyPlatformCommission { get; set; } = new();

    /// <summary>Registration counts per calendar month (Jan = index 0) for sparklines.</summary>
    public List<int> MonthlyNewDoctorCounts { get; set; } = new();

    public List<int> MonthlyNewOrganisationCounts { get; set; } = new();
    public List<int> MonthlyNewPatientCounts { get; set; } = new();

    public DashboardPendingActionsDto PendingActions { get; set; } = new();
    public DashboardSystemStatusDto SystemStatus { get; set; } = new();
    public List<TopPerformerDoctorDto> TopDoctorsByConsultationRevenue { get; set; } = new();
    public List<TopPerformerOrganisationDto> TopOrganisationsByRating { get; set; } = new();
}

public class DashboardPendingActionsDto
{
    public int PendingOphthalmologistVerifications { get; set; }
    public int PendingWithdrawalRequests { get; set; }
    public int PendingOrganisationOnboarding { get; set; }
}

public class DashboardSystemStatusDto
{
    /// <summary>Sessions with open chat (active consultation window).</summary>
    public int LiveConsultationSessions { get; set; }

    public bool ApiHealthy { get; set; } = true;
    public bool DatabaseHealthy { get; set; }
}

public class TopPerformerDoctorDto
{
    public Guid OphthalmologistId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Revenue { get; set; }

    /// <summary>Aggregate rating from patient feedback (1–5).</summary>
    public decimal RatingAverage { get; set; }

    public int RatingCount { get; set; }
}

public class TopPerformerOrganisationDto
{
    public Guid OrganisationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
}

public class UserGrowthMetricDto
{
    public int Total { get; set; }
    public int CurrentMonth { get; set; }
    public int PreviousMonth { get; set; }
    public decimal GrowthPercentage { get; set; }
}

public class PaymentMethodRevenueDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

public class MonthlyRevenuePointDto
{
    public int Month { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}

public class DailyRevenuePointDto
{
    public DateTime Date { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}
