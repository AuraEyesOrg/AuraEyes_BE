using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Consent entity - patient consent for data processing
/// </summary>
public class Consent : BaseEntity
{
    public Guid PatientId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime? SignedAt { get; private set; }
    public bool IsValid { get; private set; }

    private Consent() { } // EF Core

    public Consent(Guid patientId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Consent content cannot be empty", nameof(content));

        PatientId = patientId;
        Content = content;
        IsValid = false;
    }

    public void Sign()
    {
        SignedAt = DateTime.UtcNow;
        IsValid = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        IsValid = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
