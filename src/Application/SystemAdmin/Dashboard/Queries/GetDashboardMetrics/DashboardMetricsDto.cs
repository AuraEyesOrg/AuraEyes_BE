namespace Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;

/// <summary>
/// DTO contract for System Admin dashboard metrics.
/// Keep this shape synchronized with AuraEyes_FE system-admin dashboard API mapping.
/// </summary>
public class DashboardMetricsDto
{
    public DashboardUserGrowthMetricDto Doctors { get; set; } = new();
    public DashboardUserGrowthMetricDto Organisations { get; set; } = new();
    public DashboardUserGrowthMetricDto Patients { get; set; } = new();

    public List<DashboardPaymentMethodRevenueDto> PaymentMethodBreakdown { get; set; } = new();
    public List<DashboardRevenuePointDto> MonthlyRevenue { get; set; } = new();
    public List<DashboardRevenuePointDto> DailyRevenue { get; set; } = new();
    public decimal TotalDepositRevenueYear { get; set; }

    public decimal TotalPlatformCommissionYear { get; set; }
    public List<DashboardRevenuePointDto> MonthlyPlatformCommission { get; set; } = new();
    public List<DashboardRevenuePointDto> DailyPlatformCommission { get; set; } = new();

    public List<int> MonthlyNewDoctorCounts { get; set; } = new();
    public List<int> MonthlyNewOrganisationCounts { get; set; } = new();
    public List<int> MonthlyNewPatientCounts { get; set; } = new();

    public DashboardPendingActionsDto PendingActions { get; set; } = new();
    public DashboardSystemStatusDto SystemStatus { get; set; } = new();

    public List<DashboardTopDoctorDto> TopDoctorsByConsultationRevenue { get; set; } = new();
    public List<DashboardTopOrganisationDto> TopOrganisationsByRating { get; set; } = new();
}

public class DashboardUserGrowthMetricDto
{
    public int Total { get; set; }
    public int CurrentMonth { get; set; }
    public int PreviousMonth { get; set; }
    public decimal GrowthPercentage { get; set; }
}

public class DashboardPaymentMethodRevenueDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

public class DashboardRevenuePointDto
{
    public int? Month { get; set; }
    public string? Date { get; set; }
    public string Label { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}

public class DashboardPendingActionsDto
{
    public int PendingOphthalmologistVerifications { get; set; }
    public int PendingWithdrawalRequests { get; set; }
    public int PendingOrganisationOnboarding { get; set; }
}

public class DashboardSystemStatusDto
{
    public int LiveConsultationSessions { get; set; }
    public bool ApiHealthy { get; set; }
    public bool DatabaseHealthy { get; set; }
}

public class DashboardTopDoctorDto
{
    public Guid OphthalmologistId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
}

public class DashboardTopOrganisationDto
{
    public Guid OrganisationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
}
