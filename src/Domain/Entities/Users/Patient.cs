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
    public string? MedicalRecordNumber { get; private set; } // Mã YT (Patient-level)

    // ── EMR fields (shared by both flows) ──

    /// <summary>Body Mass Index — key risk predictor for the AI model.</summary>
    public decimal? BMI { get; private set; }

    /// <summary>Free-text history of prior diseases (e.g. "Type-2 Diabetes, Hypertension").</summary>
    public string? DiseaseHistory { get; private set; }

    /// <summary>True when the patient has no Identity user (walk-in).</summary>
    public bool IsWalkIn => UserId is null;

    /// <summary>Discount rate (e.g. 0.20 for 20%) for the next clinic booking. Null if none.</summary>
    public decimal? DiscountForNextBooking { get; private set; }

    /// <summary>UTC expiry of the discount. Must be within 30 days of grant.</summary>
    public DateTime? DiscountExpiryDate { get; private set; }

    // ── Bank information for refunds ──
    public string? BankNumber { get; private set; }
    public string? AccountName { get; private set; }
    public string? BankName { get; private set; }

    // Navigation properties
    private readonly List<RetinalImage> _retinalImages = new();
    public IReadOnlyCollection<RetinalImage> RetinalImages => _retinalImages.AsReadOnly();

    private readonly List<AiScreening> _aiScreenings = new();
    public IReadOnlyCollection<AiScreening> AiScreenings => _aiScreenings.AsReadOnly();

    // EF Core constructor — private, do not use from application code
    private Patient() { }

    // ── Factory methods ──

    /// <summary>
    /// Create a patient backed by an Identity user (registered flow + walk-in-with-account flow).
    /// Profile data (name, phone, etc.) lives in the ApplicationUser / Identity system.
    /// </summary>
    public static Patient CreateRegistered(Guid userId, decimal? bmi = null, string? diseaseHistory = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        return new Patient
        {
            UserId = userId,
            BMI = bmi,
            DiseaseHistory = diseaseHistory,
            IsDeleted = false
        };
    }

    /// <summary>
    /// Create a pure walk-in patient with NO Identity account.
    /// All profile data is stored directly on this entity.
    /// Only use when you intentionally do NOT create an ApplicationUser.
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
            Address = address?.Trim(),
            IsDeleted = false
        };
    }

    // ── Mutators ──

    public void UpdateProfile(decimal? bmi, string? diseaseHistory)
    {
        BMI = bmi;
        DiseaseHistory = diseaseHistory;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetMedicalRecordNumber(string medicalRecordNumber)
    {
        MedicalRecordNumber = medicalRecordNumber;
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

    /// <summary>
    /// Grant a discount for the next clinic booking (e.g. 0.20 for 20%).
    /// </summary>
    public void GrantDiscount(decimal rate, int expiryDays = 30)
    {
        if (rate < 0 || rate > 1)
            throw new ArgumentException("Discount rate must be between 0 and 1.", nameof(rate));

        DiscountForNextBooking = rate;
        DiscountExpiryDate = DateTime.UtcNow.AddDays(expiryDays);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Consume the discount if it is still valid. Returns the discount rate or null.
    /// </summary>
    public decimal? ConsumeDiscount()
    {
        if (DiscountForNextBooking is null)
            return null;

        if (DiscountExpiryDate.HasValue && DateTime.UtcNow > DiscountExpiryDate.Value)
        {
            DiscountForNextBooking = null;
            DiscountExpiryDate = null;
            UpdatedAt = DateTime.UtcNow;
            return null;
        }

        var rate = DiscountForNextBooking.Value;
        DiscountForNextBooking = null;
        DiscountExpiryDate = null;
        UpdatedAt = DateTime.UtcNow;
        return rate;
    }

    /// <summary>
    /// Update bank info for refunds.
    /// </summary>
    public void UpdateBankInfo(string? bankNumber, string? accountName, string? bankName)
    {
        BankNumber = bankNumber?.Trim();
        AccountName = accountName?.Trim();
        BankName = bankName?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

}
