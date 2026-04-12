using Domain.Common;
using Domain.Entities.Screening;

namespace Domain.Entities.Users;

/// <summary>
/// Patient profile entity.
/// Two independent creation paths:
///   - Registered (Aura account): <see cref="CreateRegistered"/> — profile resolved from Identity.
///   - Walk-in (no account):      <see cref="CreateWalkIn"/>      — profile stored directly here.
/// </summary>
public class Patient : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Identity user ID. Null for walk-in patients (no Aura account).
    /// </summary>
    public Guid? UserId { get; private set; }

    // ── Walk-in profile fields (only populated when UserId is null) ──

    public string? FullName { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? CitizenId { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public int? GenderId { get; private set; }
    public string? Address { get; private set; }

    // ── EMR fields (shared by both flows) ──

    /// <summary>Body Mass Index — key risk predictor for the AI model.</summary>
    public decimal? BMI { get; private set; }

    /// <summary>Free-text history of prior diseases (e.g. "Type-2 Diabetes, Hypertension").</summary>
    public string? DiseaseHistory { get; private set; }

    /// <summary>Current purchased AI screening credits balance.</summary>
    public int PurchasedAiQuota { get; private set; }

    /// <summary>AI screening credits used today (reset to 0 daily by Hangfire job).</summary>
    public int UsedAiQuota { get; private set; }

    /// <summary>True when the patient has no Identity user (walk-in).</summary>
    public bool IsWalkIn => UserId is null;

    // Navigation properties
    private readonly List<RetinalImage> _retinalImages = new();
    public IReadOnlyCollection<RetinalImage> RetinalImages => _retinalImages.AsReadOnly();

    private readonly List<AiScreening> _aiScreenings = new();
    public IReadOnlyCollection<AiScreening> AiScreenings => _aiScreenings.AsReadOnly();

    private Patient() { } // EF Core

    // ── Factory methods ──

    /// <summary>
    /// Create a registered patient backed by an Identity user.
    /// Profile data (name, phone, etc.) lives in the Identity system.
    /// </summary>
    public static Patient CreateRegistered(Guid userId, decimal? bmi = null, string? diseaseHistory = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        return new Patient
        {
            UserId = userId,
            BMI = bmi,
            DiseaseHistory = diseaseHistory
        };
    }

    /// <summary>
    /// Create a walk-in patient with no Identity account.
    /// All profile data is stored directly on the entity.
    /// </summary>
    public static Patient CreateWalkIn(
        string fullName,
        string? phoneNumber = null,
        string? citizenId = null,
        DateTime? dateOfBirth = null,
        int? genderId = null,
        string? address = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required for walk-in patients.", nameof(fullName));

        return new Patient
        {
            UserId = null,
            FullName = fullName.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            CitizenId = citizenId?.Trim(),
            DateOfBirth = dateOfBirth,
            GenderId = genderId,
            Address = address?.Trim()
        };
    }

    // ── Backward-compatible constructor for existing registration flows ──

    public Patient(Guid userId, decimal? bmi = null, string? diseaseHistory = null)
    {
        UserId = userId;
        BMI = bmi;
        DiseaseHistory = diseaseHistory;
    }

    // ── Mutators ──

    public void UpdateProfile(decimal? bmi, string? diseaseHistory)
    {
        BMI = bmi;
        DiseaseHistory = diseaseHistory;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update walk-in profile fields. Only valid for walk-in patients.
    /// </summary>
    public void UpdateWalkInProfile(
        string fullName,
        string? phoneNumber,
        string? citizenId,
        DateTime? dateOfBirth,
        int? genderId,
        string? address)
    {
        if (!IsWalkIn)
            throw new InvalidOperationException("Cannot update walk-in profile on a registered patient.");

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required.", nameof(fullName));

        FullName = fullName.Trim();
        PhoneNumber = phoneNumber?.Trim();
        CitizenId = citizenId?.Trim();
        DateOfBirth = dateOfBirth;
        GenderId = genderId;
        Address = address?.Trim();
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
