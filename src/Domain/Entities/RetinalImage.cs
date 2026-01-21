using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Retinal Image entity - captured eye images for screening
/// </summary>
public class RetinalImage : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Guid? AiScreeningId { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string? DeviceName { get; private set; }
    public EyeSide EyeSide { get; private set; }
    public decimal? QualityScore { get; private set; }
    public DateTime CapturedAt { get; private set; }

    private RetinalImage() { } // EF Core

    public RetinalImage(Guid patientId, string imageUrl, EyeSide eyeSide, DateTime capturedAt, string? deviceName = null, decimal? qualityScore = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL cannot be empty", nameof(imageUrl));

        PatientId = patientId;
        ImageUrl = imageUrl;
        EyeSide = eyeSide;
        CapturedAt = capturedAt;
        DeviceName = deviceName;
        QualityScore = qualityScore;
    }

    public void AssignToScreening(Guid aiScreeningId)
    {
        AiScreeningId = aiScreeningId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateQualityScore(decimal qualityScore)
    {
        if (qualityScore < 0 || qualityScore > 100)
            throw new ArgumentException("Quality score must be between 0 and 100", nameof(qualityScore));

        QualityScore = qualityScore;
        UpdatedAt = DateTime.UtcNow;
    }
}
