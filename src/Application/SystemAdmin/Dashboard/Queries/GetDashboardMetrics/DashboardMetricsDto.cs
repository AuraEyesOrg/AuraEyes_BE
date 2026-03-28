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
