using Domain.Common;

namespace Domain.Entities.Screening;

/// <summary>
/// AI Screening entity - contains AI processing results.
/// Each session is owned by a Patient.
/// </summary>
public class AiScreening : BaseEntity, IAggregateRoot
{
    public Guid PatientId { get; private set; }
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


    private AiScreening() { } // EF Core

    public AiScreening(Guid patientId, string modelVersion)
    {
        if (string.IsNullOrWhiteSpace(modelVersion))
            throw new ArgumentException("Model version cannot be empty", nameof(modelVersion));

        PatientId = patientId;
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

}
