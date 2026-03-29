using Domain.Common;
using Domain.Entities.Screening;

namespace Domain.Entities.Users;

/// <summary>
/// Consent - 1:1 legal consent record that accompanies every AiScreening session.
/// Binds patient agreement to the exact screening event for liability protection.
/// </summary>
public class Consent : BaseEntity
{
    /// <summary>FK to AiScreening (1:1).</summary>
    public Guid AiScreeningId { get; private set; }

    /// <summary>FK to Patient who owns and signs this consent.</summary>
    public Guid PatientId { get; private set; }

    public string Content { get; private set; } = string.Empty;
    public DateTime? SignedAt { get; private set; }
    public bool IsAgreed { get; private set; }

    private Consent() { } // EF Core

    public Consent(Guid aiScreeningId, Guid patientId, string content)
    {
        if (aiScreeningId == Guid.Empty)
            throw new ArgumentException("AiScreeningId cannot be empty", nameof(aiScreeningId));

        if (patientId == Guid.Empty)
            throw new ArgumentException("PatientId cannot be empty", nameof(patientId));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Consent content cannot be empty", nameof(content));

        AiScreeningId = aiScreeningId;
        PatientId = patientId;
        Content = content.Trim();
        IsAgreed = false;
    }

    public void Agree(string? content = null)
    {
        if (!string.IsNullOrWhiteSpace(content))
        {
            Content = content.Trim();
        }

        SignedAt = DateTime.UtcNow;
        IsAgreed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Consent content cannot be empty", nameof(content));

        Content = content.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        IsAgreed = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
