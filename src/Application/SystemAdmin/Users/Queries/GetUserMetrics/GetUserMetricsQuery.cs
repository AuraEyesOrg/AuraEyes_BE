using Application.Common.Interfaces;

namespace Application.SystemAdmin.Users.Queries.GetUserMetrics;

/// <summary>
/// Query to get user management metrics
/// Screen: 3.5.2-3.5.5 View Total Users, Active Doctors, Patients Screened, Pending Approvals
/// </summary>
public record GetUserMetricsQuery : IQuery<UserMetricsDto>;
