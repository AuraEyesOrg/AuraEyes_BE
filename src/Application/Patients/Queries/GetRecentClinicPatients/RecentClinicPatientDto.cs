using Application.Common.Models;

namespace Application.Patients.Queries.GetRecentClinicPatients;

public class RecentClinicPatientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = "M"; // M or F
    public DateOnly? DateOfBirth { get; set; }
    public string? CitizenId { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsWalkIn { get; set; }
    public double? Bmi { get; set; }
    public string? DiseaseHistory { get; set; }
    public DateTime? LastScreening { get; set; }
    public string? AiPrediction { get; set; }
    public double Confidence { get; set; }
    public string Status { get; set; } = "reviewed"; // pending-review, reviewed, archived
    public string Priority { get; set; } = "medium"; // low, medium, high
}
