namespace Application.SystemAdmin.Users.Queries.GetUserMetrics;

/// <summary>
/// DTO for user management metrics
/// </summary>
public class UserMetricsDto
{
    public int TotalUsers { get; set; }
    public decimal TotalUsersMonthlyChange { get; set; }
    public int ActiveDoctors { get; set; }
    public decimal ActiveDoctorsChange { get; set; }
    public int PatientsScreened { get; set; }
    public decimal PatientsScreenedChange { get; set; }
    public int PendingApprovals { get; set; }
    public int ClinicStaffCount { get; set; }
    public int OphthalmologistCount { get; set; }
}
