using Application.Common.Interfaces;

namespace Application.SystemAdmin.Dashboard.Queries.GetScreeningVolumeTrends;

/// <summary>
/// Query to get screening volume trends over time
/// Screen: 3.3.5 View Screening Volume Trends
/// </summary>
public record GetScreeningVolumeTrendsQuery : IQuery<ScreeningVolumeTrendsDto>
{
    public string TimeRange { get; init; } = "monthly";
    public int Periods { get; init; } = 12;
}
