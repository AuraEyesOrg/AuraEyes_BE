namespace Application.SystemAdmin.Dashboard.Queries.GetSystemHealth;

/// <summary>
/// DTO for system health status
/// </summary>
public class SystemHealthDto
{
    public bool AllSystemsOperational { get; set; }
    public List<ComponentHealthDto> Components { get; set; } = new();
}

public class ComponentHealthDto
{
    public string ComponentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public double? LatencyMs { get; set; }
    public double? UptimePercentage { get; set; }
    public DateTime LastCheckedAt { get; set; }
}
