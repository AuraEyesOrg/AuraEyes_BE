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

    /// <summary>Current purchased AI screening credits balance.</summary>
    public int PurchasedAiQuota { get; private set; }

    /// <summary>AI screening credits used today (reset to 0 daily by Hangfire job).</summary>
    public int UsedAiQuota { get; private set; }

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

    public void AddPurchasedQuota(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        PurchasedAiQuota += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasAvailableQuota(int freeQuota)
    {
        return UsedAiQuota < freeQuota || PurchasedAiQuota > 0;
    }

    public void ConsumeQuota(int freeQuota)
    {
        if (freeQuota < 0)
            throw new ArgumentOutOfRangeException(nameof(freeQuota));

        if (UsedAiQuota < freeQuota)
        {
            UsedAiQuota++;
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        if (PurchasedAiQuota <= 0)
            throw new InvalidOperationException("No AI quota available.");

        PurchasedAiQuota--;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ResetDailyQuota()
    {
        UsedAiQuota = 0;
        UpdatedAt = DateTime.UtcNow;
    }
}
