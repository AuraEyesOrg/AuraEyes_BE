using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for Organisation aggregate root.
/// Contains domain-specific query methods beyond basic CRUD.
/// </summary>
public interface IOrganisationRepository : IRepository<Organisation>
{
    /// <summary>
    /// Get organisation by OwnerId (ApplicationUser reference).
    /// </summary>
    Task<Organisation?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated list of organisations with optional filters.
    /// </summary>
    Task<(IReadOnlyList<Organisation> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        OrgType? orgType = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if organisation exists for a specific owner.
    /// </summary>
    Task<bool> ExistsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if organisation name already exists.
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if license number already exists.
    /// </summary>
    Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get organisations by type.
    /// </summary>
    Task<IReadOnlyList<Organisation>> GetByOrgTypeAsync(OrgType orgType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total count of organisations by type.
    /// </summary>
    Task<Dictionary<OrgType, int>> GetCountByOrgTypeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total active organisations count.
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);
}
