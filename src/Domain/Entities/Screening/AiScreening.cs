using Domain.Common;
using Domain.Entities.Users;

namespace Domain.Entities.Screening;

/// <summary>
/// AI Screening entity - contains AI processing results.
/// Each session is owned by a Patient and requires exactly one Consent record.
/// </summary>
public class AiScreening : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
    public Guid? OrganisationId { get; private set; }
    public string ModelVersion { get; private set; } = string.Empty;
    public DateTime? ProcessedAt { get; private set; }

    /// <summary>
    /// Raw JSON output from AI model - stored as JSONB in PostgreSQL
    /// </summary>
    public string? RawJsonOutput { get; private set; }

    public bool IsActive { get; private set; }

    // Navigation properties
    private readonly List<RetinalImage> _retinalImages = new();
    public IReadOnlyCollection<RetinalImage> RetinalImages => _retinalImages.AsReadOnly();

    private readonly List<ScreeningResult> _screeningResults = new();
    public IReadOnlyCollection<ScreeningResult> ScreeningResults => _screeningResults.AsReadOnly();

    /// <summary>1:1 consent that must accompany every AI session.</summary>
    public Consent? Consent { get; private set; }

    private AiScreening() { } // EF Core

    public AiScreening(Guid patientId, string modelVersion, Guid? organisationId = null)
    {
        if (string.IsNullOrWhiteSpace(modelVersion))
            throw new ArgumentException("Model version cannot be empty", nameof(modelVersion));

        PatientId = patientId;
        OrganisationId = organisationId;
        ModelVersion = modelVersion;
        IsActive = true;
    }

    public void Process(string rawJsonOutput)
    {
        RawJsonOutput = rawJsonOutput;
        ProcessedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddRetinalImage(RetinalImage image)
    {
        _retinalImages.Add(image);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddScreeningResult(ScreeningResult result)
    {
        _screeningResults.Add(result);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordConsent(Guid patientId, string consentContent)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("PatientId cannot be empty", nameof(patientId));

        if (patientId != PatientId)
            throw new InvalidOperationException("Consent can only be recorded by the screening owner.");

        if (string.IsNullOrWhiteSpace(consentContent))
            throw new ArgumentException("Consent content cannot be empty", nameof(consentContent));

        var normalizedContent = consentContent.Trim();

        if (Consent is null)
        {
            Consent = new Consent(Id, patientId, normalizedContent);
            Consent.Agree();
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        if (Consent.PatientId != patientId)
            throw new InvalidOperationException("Existing consent owner does not match screening owner.");

        var mergedContent = Consent.Content;
        if (!mergedContent.Contains(normalizedContent, StringComparison.Ordinal))
        {
            mergedContent = string.IsNullOrWhiteSpace(mergedContent)
                ? normalizedContent
                : $"{mergedContent}\n\n{normalizedContent}";
        }

        Consent.Agree(mergedContent);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasAgreedConsent(Guid patientId)
    {
        return Consent is not null
               && Consent.IsAgreed
               && Consent.PatientId == patientId;
    }
}
