using Domain.Common;
using Domain.Entities.Users;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for Ophthalmologist aggregate root.
/// Contains domain-specific query methods beyond basic CRUD.
/// </summary>
public interface IOphthalmologistRepository : IRepository<Ophthalmologist>
{
    /// <summary>
    /// Get ophthalmologist by UserId (ApplicationUser reference).
    /// </summary>
    Task<Ophthalmologist?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get ophthalmologist with certificates included.
    /// </summary>
    Task<Ophthalmologist?> GetByIdWithCertificatesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated list of ophthalmologists with optional filters.
    /// </summary>
    Task<(IReadOnlyList<Ophthalmologist> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if ophthalmologist exists for a specific user.
    /// </summary>
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get display names keyed by ophthalmologist ID.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, string>> GetDisplayNamesByIdsAsync(
        IReadOnlyCollection<Guid> ophthalmologistIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get enhanced doctor details (name, avatar, bio, ratings, certificates) keyed by ophthalmologist ID.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, EnhancedDoctorDetail>> GetEnhancedDoctorDetailsByIdsAsync(
        IReadOnlyCollection<Guid> ophthalmologistIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get doctor basic details (FullName, AvatarUrl) keyed by ophthalmologist ID.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, (string FullName, string? AvatarUrl)>> GetDoctorDetailsByIdsAsync(
        IReadOnlyCollection<Guid> ophthalmologistIds,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Get doctors available for real-time consilium based on time window.
    /// Derived from overlapping appointment slots and leave requests.
    /// </summary>
    Task<List<ConsiliumDoctorDetail>> GetAvailableDoctorsForConsiliumAsync(
        DateTime windowStart,
        DateTime windowEnd,
        CancellationToken cancellationToken = default);
}

public record EnhancedDoctorDetail(
    string FullName,
    string? AvatarUrl,
    string? ProviderAvatarUrl,
    string? Bio,
    decimal RatingAverage,
    int RatingCount,
    int CompletedPatientsCount,
    List<Certificate> Certificates,
    bool IsActive);

public record ConsiliumDoctorDetail(
    Guid Id,
    string Name,
    string? Avatar,
    string? DegreeLevel);
