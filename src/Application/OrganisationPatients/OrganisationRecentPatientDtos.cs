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
    public string PhoneNumber { get; init; } = string.Empty;
    public DateTime LastScreening { get; init; }

    public string AiPrediction { get; init; } = string.Empty;
    public decimal Confidence { get; init; } // 0 - 100

    /// <summary>pending-review | reviewed | archived</summary>
    public string Status { get; init; } = "pending-review";

    /// <summary>low | medium | high</summary>
    public string Priority { get; init; } = "low";
}

