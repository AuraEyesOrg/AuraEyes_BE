using Application.Common.Interfaces;

namespace Application.SystemAdmin.Dashboard.Queries.GetSystemHealth;

/// <summary>
/// Query to get system health status
/// Screen: 3.3.7 View System Health Status
/// </summary>
public record GetSystemHealthQuery : IQuery<SystemHealthDto>;
