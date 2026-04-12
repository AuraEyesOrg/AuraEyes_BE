using Domain.Common;

namespace Domain.Entities.Users;

/// <summary>
/// Link entity between Organisation and Patient.
/// A patient can be managed by multiple organisations.
/// </summary>
public class OrganisationPatientLink : BaseEntity, IAggregateRoot
{
    public Guid OrganisationId { get; private set; }
    public Guid PatientId { get; private set; }
    public string Source { get; private set; } = "system";
    public DateTime FirstLinkedAt { get; private set; }
    public DateTime LastSeenAt { get; private set; }

    public Organisation? Organisation { get; private set; }
    public Patient? Patient { get; private set; }

    private OrganisationPatientLink() { }

    public OrganisationPatientLink(Guid organisationId, Guid patientId, string source = "system")
    {
        if (organisationId == Guid.Empty)
            throw new ArgumentException("OrganisationId cannot be empty", nameof(organisationId));

        if (patientId == Guid.Empty)
            throw new ArgumentException("PatientId cannot be empty", nameof(patientId));

        OrganisationId = organisationId;
        PatientId = patientId;
        Source = string.IsNullOrWhiteSpace(source) ? "system" : source.Trim();
        FirstLinkedAt = DateTime.UtcNow;
        LastSeenAt = FirstLinkedAt;
    }

    public void Touch(string? source = null)
    {
        LastSeenAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(source))
        {
            Source = source.Trim();
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
