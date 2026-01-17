using Domain.Common;
using Domain.Events;

namespace Domain.Entities;

/// <summary>
/// Ophthalmologist aggregate root demonstrating DDD patterns
/// Encapsulates business logic for ophthalmologist management
/// </summary>
public class Ophthalmologist : BaseEntity, IAggregateRoot
{
    // Properties with private setters for encapsulation
    public Guid UserId { get; private set; }
    public string Bio { get; private set; } = string.Empty;
    public int YearsOfExperience { get; private set; }
    public bool IsVerified { get; private set; }

    // Parameterless constructor required by EF Core
    private Ophthalmologist() 
    { 
        Bio = string.Empty;
    }

    // Constructor with validation - creates valid aggregate
    public Ophthalmologist(Guid userId, string bio, int yearsOfExperience)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsOfExperience));

        UserId = userId;
        Bio = bio ?? string.Empty;
        YearsOfExperience = yearsOfExperience;
        IsVerified = false; // New ophthalmologists start as unverified

        // Raise domain event for ophthalmologist creation
        AddDomainEvent(new OphthalmologistCreatedEvent(Id, UserId));
    }

    // Business method to update profile details
    public void UpdateProfile(string bio, int yearsOfExperience)
    {
        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsOfExperience));

        Bio = bio ?? string.Empty;
        YearsOfExperience = yearsOfExperience;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OphthalmologistUpdatedEvent(Id, UserId));
    }

    // Business method to verify ophthalmologist
    public void Verify()
    {
        if (IsVerified)
            throw new InvalidOperationException("Ophthalmologist is already verified");

        IsVerified = true;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OphthalmologistVerifiedEvent(Id, UserId));
    }

    // Business method to unverify ophthalmologist
    public void Unverify()
    {
        if (!IsVerified)
            throw new InvalidOperationException("Ophthalmologist is not verified");

        IsVerified = false;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OphthalmologistUnverifiedEvent(Id, UserId));
    }

    // Business method to update years of experience
    public void UpdateYearsOfExperience(int yearsOfExperience)
    {
        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative", nameof(yearsOfExperience));

        YearsOfExperience = yearsOfExperience;
        UpdatedAt = DateTime.UtcNow;
    }

    // Helper method to check if ophthalmologist is experienced
    public bool IsExperienced(int minimumYears = 5)
    {
        return YearsOfExperience >= minimumYears;
    }
}
