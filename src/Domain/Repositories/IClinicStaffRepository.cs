using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="ClinicStaff"/> entity.
/// </summary>
public interface IClinicStaffRepository : IRepository<ClinicStaff>
{
    /// <summary>Gets a clinic staff profile by its linked Identity user ID.</summary>
    Task<ClinicStaff?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Returns all active clinic staff members.</summary>
    Task<List<ClinicStaff>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns all staff members who hold a specific sub-role.</summary>
    Task<List<ClinicStaff>> GetBySubRoleAsync(ClinicStaffRole subRole, CancellationToken cancellationToken = default);

    /// <summary>Checks whether a user already has a ClinicStaff profile.</summary>
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a ClinicStaff profile by entity.</summary>
    void Remove(ClinicStaff clinicStaff);
}
