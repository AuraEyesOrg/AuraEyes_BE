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

public interface IOrganisationPatientsRepository
{
    Task<IReadOnlyList<OrganisationRecentPatientReadModel>> GetRecentPatientsForOrganisationAdminAsync(
        Guid orgAdminUserId,
        int take,
        CancellationToken cancellationToken = default);
}
