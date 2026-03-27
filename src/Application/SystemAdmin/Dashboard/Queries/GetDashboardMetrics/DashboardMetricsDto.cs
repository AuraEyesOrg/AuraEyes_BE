namespace Application.SystemAdmin.Dashboard.Queries.GetDashboardMetrics;

/// <summary>
/// DTO for dashboard metrics summary
/// </summary>
public class DashboardMetricsDto
{
    public decimal TotalInflow { get; set; }
    public decimal TotalOutflow { get; set; }
    public decimal RefundOutflow { get; set; }
    public decimal NetCashflow { get; set; }
    public decimal EstimatedCommission { get; set; }
    public List<PaymentMethodCashflowDto> PaymentMethodBreakdown { get; set; } = new();
}

public class PaymentMethodCashflowDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}
