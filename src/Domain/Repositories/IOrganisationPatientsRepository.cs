namespace Domain.Repositories;

public sealed record OrganisationRecentPatientReadModel
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Age { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public DateTime LastScreening { get; init; }
    public string AiPrediction { get; init; } = string.Empty;
    public decimal Confidence { get; init; }
    public string Status { get; init; } = "pending-review";
    public string Priority { get; init; } = "low";
}

public sealed record OrganisationScreeningHistoryReadModel
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public int ImagesCount { get; init; }
    public string? LatestRiskLevel { get; init; }
    public decimal? ConfidenceScore { get; init; }
    public string? AiPrimaryLabel { get; init; }
    public string Status { get; init; } = "pending";
}

public interface IOrganisationPatientsRepository
{
    Task<IReadOnlyList<OrganisationRecentPatientReadModel>> GetRecentPatientsForOrganisationAdminAsync(
        Guid orgAdminUserId,
        int take,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrganisationScreeningHistoryReadModel>> GetScreeningHistoryForOrganisationAdminAsync(
        Guid orgAdminUserId,
        int take,
        CancellationToken cancellationToken = default);

    Task<bool> IsPatientManagedByOrganisationAdminAsync(
        Guid orgAdminUserId,
        Guid patientId,
        CancellationToken cancellationToken = default);
}
