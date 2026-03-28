namespace Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;

public class OphthalmologistListDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Bio { get; set; }
    public int YearsOfExperience { get; set; }
    public string EmploymentType { get; set; } = string.Empty;
    public int? WorkingHoursPerWeek { get; set; }
    public decimal? ExpectedMonthlySalary { get; set; }
    public decimal? CommissionRate { get; set; }
    public decimal? ActualMonthlySalary { get; set; }
    public string VerificationStatus { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string? LicenseUrl { get; set; }
    public string? DegreeUrl { get; set; }
    public string? RejectionReason { get; set; }
    public string? OrganisationName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
