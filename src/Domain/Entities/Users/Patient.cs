using Domain.Common;
using Domain.Entities.Screening;

namespace Domain.Entities.Users;

/// <summary>
/// Patient profile entity - linked to ApplicationUser.
/// Contains lean EMR fields for AI risk analysis (diabetes/hypertension effect on retina).
/// </summary>
public class Patient : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }

    /// <summary>Body Mass Index — key risk predictor for the AI model.</summary>
    public decimal? BMI { get; private set; }

    /// <summary>Free-text history of prior diseases (e.g. "Type-2 Diabetes, Hypertension").</summary>
    public string? DiseaseHistory { get; private set; }

    // Navigation properties
    private readonly List<RetinalImage> _retinalImages = new();
    public IReadOnlyCollection<RetinalImage> RetinalImages => _retinalImages.AsReadOnly();

    private readonly List<AiScreening> _aiScreenings = new();
    public IReadOnlyCollection<AiScreening> AiScreenings => _aiScreenings.AsReadOnly();

    private Patient() { } // EF Core

    public Patient(Guid userId, decimal? bmi = null, string? diseaseHistory = null)
    {
        UserId = userId;
        BMI = bmi;
        DiseaseHistory = diseaseHistory;
    }

    public void UpdateProfile(decimal? bmi, string? diseaseHistory)
    {
        BMI = bmi;
        DiseaseHistory = diseaseHistory;
        UpdatedAt = DateTime.UtcNow;
    }
}
