namespace Application.SystemAdmin.Dashboard.Queries.GetDoctorStatus;

public class DoctorStatusDto
{
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public int PatientsHandledToday { get; set; }
    public int ActiveLoad { get; set; }
}
