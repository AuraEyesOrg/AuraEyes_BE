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
        bool? isVerified = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if ophthalmologist exists for a specific user.
    /// </summary>
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all verified ophthalmologists.
    /// </summary>
    Task<IReadOnlyList<Ophthalmologist>> GetVerifiedAsync(CancellationToken cancellationToken = default);
}
