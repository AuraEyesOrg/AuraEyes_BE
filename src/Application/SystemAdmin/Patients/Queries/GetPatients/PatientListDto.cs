namespace Application.SystemAdmin.Patients.Queries.GetPatients;

public class PatientListDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public decimal? BMI { get; set; }
    public string? DiseaseHistory { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool IsWalkIn { get; set; }
    public string PatientType { get; set; } = string.Empty;
    public string? LinkedOrganisationName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
