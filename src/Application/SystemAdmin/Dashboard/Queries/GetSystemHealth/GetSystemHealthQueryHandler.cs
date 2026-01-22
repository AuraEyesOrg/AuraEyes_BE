using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetSystemHealth;

/// <summary>
/// Handler for GetSystemHealthQuery - Returns mock data
/// </summary>
public class GetSystemHealthQueryHandler : IQueryHandler<GetSystemHealthQuery, SystemHealthDto>
{
    public Task<Result<SystemHealthDto>> Handle(GetSystemHealthQuery request, CancellationToken cancellationToken)
    {
        // Mock system health data
        var components = new List<ComponentHealthDto>
        {
            new() { ComponentName = "API Server", Status = "Operational", IsHealthy = true, LatencyMs = 45, UptimePercentage = 99.95, LastCheckedAt = DateTime.UtcNow },
            new() { ComponentName = "AI Inference Engine", Status = "Operational", IsHealthy = true, LatencyMs = 150, UptimePercentage = 99.8, LastCheckedAt = DateTime.UtcNow },
            new() { ComponentName = "Database", Status = "Operational", IsHealthy = true, LatencyMs = 12, UptimePercentage = 99.99, LastCheckedAt = DateTime.UtcNow },
            new() { ComponentName = "Storage Service", Status = "Operational", IsHealthy = true, LatencyMs = 25, UptimePercentage = 99.9, LastCheckedAt = DateTime.UtcNow },
            new() { ComponentName = "Notification Service", Status = "Operational", IsHealthy = true, LatencyMs = 35, UptimePercentage = 99.7, LastCheckedAt = DateTime.UtcNow }
        };

        var dto = new SystemHealthDto
        {
            AllSystemsOperational = true,
            Components = components
        };

        return Task.FromResult(Result<SystemHealthDto>.Success(dto));
    }
}
