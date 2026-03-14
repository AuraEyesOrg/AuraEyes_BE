namespace Application.SystemAdmin.Dashboard.Queries.GetScreeningVolumeTrends;

/// <summary>
/// DTO for screening volume trends
/// </summary>
public class ScreeningVolumeTrendsDto
{
    public string TimeRange { get; set; } = string.Empty;
    public List<VolumeTrendDataPoint> DataPoints { get; set; } = new();
    public int TotalScreenings { get; set; }
    public decimal AveragePerPeriod { get; set; }
}

public class VolumeTrendDataPoint
{
    public DateTime Date { get; set; }
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}
