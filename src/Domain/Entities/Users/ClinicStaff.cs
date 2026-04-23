using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Users;

/// <summary>
/// ClinicStaff profile entity — represents a clinic employee (Receptionist, Coordinator, or Cashier).
/// Linked 1-to-1 with an ApplicationUser (Identity).
/// Sub-role assignment is stored as a bitmask of <see cref="ClinicStaffRole"/> values,
/// allowing one staff member to hold multiple sub-roles.
/// </summary>
public class ClinicStaff : BaseEntity, IAggregateRoot
{
    /// <summary>FK to ApplicationUser (Identity). Unique per staff member.</summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Comma-separated sub-role names (e.g. "Receptionist,Cashier").
    /// Stored as a string for flexibility; parsed via <see cref="GetSubRoles"/>.
    /// </summary>
    public string SubRoles { get; private set; } = string.Empty;

    /// <summary>Internal department / team name (optional).</summary>
    public string? Department { get; private set; }

    /// <summary>Employee ID / staff code assigned by the clinic (optional).</summary>
    public string? EmployeeCode { get; private set; }

    /// <summary>Direct phone number of the staff member.</summary>
    public string? Phone { get; private set; }

    /// <summary>Whether this staff account is currently active.</summary>
    public bool IsActive { get; private set; }

    // Navigation properties
    /// <summary>Navigation back to the Identity user (not EF-owned; joined via UserId).</summary>

    private ClinicStaff() { } // EF Core

    /// <summary>
    /// Creates a new ClinicStaff profile.
    /// </summary>
    /// <param name="userId">The linked Identity user ID.</param>
    /// <param name="subRoles">At least one sub-role must be provided.</param>
    /// <param name="department">Optional department name.</param>
    /// <param name="employeeCode">Optional employee code.</param>
    /// <param name="phone">Optional phone number.</param>
    public ClinicStaff(
        Guid userId,
        IEnumerable<ClinicStaffRole> subRoles,
        string? department = null,
        string? employeeCode = null,
        string? phone = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        var roleList = subRoles?.ToList() ?? [];
        if (roleList.Count == 0)
            throw new ArgumentException("At least one sub-role must be assigned.", nameof(subRoles));

        UserId = userId;
        SubRoles = string.Join(",", roleList.Distinct().Select(r => r.ToString()));
        Department = department?.Trim();
        EmployeeCode = employeeCode?.Trim();
        Phone = phone?.Trim();
        IsActive = true;
    }

    // ── Behaviour ──

    /// <summary>Returns the parsed list of sub-roles assigned to this staff member.</summary>
    public IReadOnlyList<ClinicStaffRole> GetSubRoles()
    {
        if (string.IsNullOrWhiteSpace(SubRoles))
            return [];

        return SubRoles
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(r => Enum.TryParse<ClinicStaffRole>(r, out var role) ? (ClinicStaffRole?)role : null)
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .Distinct()
            .ToList()
            .AsReadOnly();
    }

    /// <summary>Returns true if this staff member holds the specified sub-role.</summary>
    public bool HasSubRole(ClinicStaffRole role) => GetSubRoles().Contains(role);

    /// <summary>Updates the assigned sub-roles. At least one must remain.</summary>
    public void UpdateSubRoles(IEnumerable<ClinicStaffRole> subRoles)
    {
        var roleList = subRoles?.ToList() ?? [];
        if (roleList.Count == 0)
            throw new ArgumentException("At least one sub-role must remain.", nameof(subRoles));

        SubRoles = string.Join(",", roleList.Distinct().Select(r => r.ToString()));
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Updates optional profile details.</summary>
    public void UpdateProfile(
        string? department,
        string? employeeCode,
        string? phone)
    {
        Department = department?.Trim();
        EmployeeCode = employeeCode?.Trim();
        Phone = phone?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Activates the staff account.</summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Deactivates the staff account.</summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
