using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Patient profile entity - linked to ApplicationUser
/// Contains medical history as JSON
/// </summary>
public class Patient : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// Medical history stored as JSONB in PostgreSQL
    /// </summary>
    public string? MedicalHistorySummary { get; private set; }

    // Navigation properties
    private readonly List<RetinalImage> _retinalImages = new();
    public IReadOnlyCollection<RetinalImage> RetinalImages => _retinalImages.AsReadOnly();

    private readonly List<Consent> _consents = new();
    public IReadOnlyCollection<Consent> Consents => _consents.AsReadOnly();

    private Patient() { } // EF Core

    public Patient(Guid userId, string? medicalHistorySummary = null)
    {
        UserId = userId;
        MedicalHistorySummary = medicalHistorySummary;
    }

    public void UpdateMedicalHistory(string? medicalHistorySummary)
    {
        MedicalHistorySummary = medicalHistorySummary;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddConsent(Consent consent)
    {
        _consents.Add(consent);
        UpdatedAt = DateTime.UtcNow;
    }
}
