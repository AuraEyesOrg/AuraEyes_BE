using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetScreeningVolumeTrends;

/// <summary>
/// Handler for GetScreeningVolumeTrendsQuery - Returns mock data
/// </summary>
public class GetScreeningVolumeTrendsQueryHandler : IQueryHandler<GetScreeningVolumeTrendsQuery, ScreeningVolumeTrendsDto>
{
    public Task<Result<ScreeningVolumeTrendsDto>> Handle(GetScreeningVolumeTrendsQuery request, CancellationToken cancellationToken)
    {
        // Generate mock monthly data
        var dataPoints = new List<VolumeTrendDataPoint>();
        var random = new Random(42); // Fixed seed for consistent mock data
        var baseDate = DateTime.UtcNow.AddMonths(-request.Periods);

        for (int i = 0; i < request.Periods; i++)
        {
            var date = baseDate.AddMonths(i);
            dataPoints.Add(new VolumeTrendDataPoint
            {
                Date = new DateTime(date.Year, date.Month, 1),
                Label = date.ToString("MMM yyyy"),
                Count = 1200 + random.Next(-200, 300)
            });
        }

        var dto = new ScreeningVolumeTrendsDto
        {
            TimeRange = request.TimeRange,
            DataPoints = dataPoints,
            TotalScreenings = dataPoints.Sum(x => x.Count),
            AveragePerPeriod = Math.Round((decimal)dataPoints.Sum(x => x.Count) / dataPoints.Count, 1)
        };

        return Task.FromResult(Result<ScreeningVolumeTrendsDto>.Success(dto));
    }
}
