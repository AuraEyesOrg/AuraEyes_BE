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

    public string Content { get; private set; } = string.Empty;
    public DateTime? SignedAt { get; private set; }
    public bool IsAgreed { get; private set; }

    private Consent() { } // EF Core

    public Consent(Guid aiScreeningId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Consent content cannot be empty", nameof(content));

        AiScreeningId = aiScreeningId;
        Content = content;
        IsAgreed = false;
    }

    public void Agree()
    {
        SignedAt = DateTime.UtcNow;
        IsAgreed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        IsAgreed = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
