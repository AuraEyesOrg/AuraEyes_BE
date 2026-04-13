namespace Application.OrganisationPatients;

/// <summary>
/// Organisation admin view item used by /api/organisations/patients.
/// </summary>
public sealed record OrganisationRecentPatientDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Age { get; init; }
    public string Gender { get; init; } = string.Empty; // "M" | "F"
    public DateTime? DateOfBirth { get; init; }
    public string? CitizenId { get; init; }
    public string? Address { get; init; }
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;

    /// <summary>true = walk-in (org owns full profile), false = Aura account (read-only admin fields)</summary>
    public bool IsWalkIn { get; init; }

    /// <summary>Body Mass Index</summary>
    public decimal? Bmi { get; init; }

    /// <summary>Free-text disease history</summary>
    public string? DiseaseHistory { get; init; }

    public DateTime LastScreening { get; init; }
    public string AiPrediction { get; init; } = string.Empty;
    public decimal Confidence { get; init; } // 0 - 100

    /// <summary>pending-review | reviewed | archived</summary>
    public string Status { get; init; } = "pending-review";

    /// <summary>low | medium | high</summary>
    public string Priority { get; init; } = "low";
}
