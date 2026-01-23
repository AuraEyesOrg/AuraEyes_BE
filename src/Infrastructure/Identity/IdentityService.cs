using Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

/// <summary>
/// Identity service implementation wrapping ASP.NET Core Identity.
/// Provides a clean interface for the Application layer.
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<(bool Succeeded, string[] Errors)> CreateUserAsync(
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> CreateUserWithRoleAsync(
        string email,
        string password,
        string fullName,
        string role,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(role))
        {
            var roleResult = await _roleManager.CreateAsync(new ApplicationRole(role));
            if (!roleResult.Succeeded)
            {
                return (false, roleResult.Errors.Select(e => e.Description).ToArray());
            }
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, role);

        return (addRoleResult.Succeeded, addRoleResult.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<bool> CheckPasswordAsync(Guid userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);

        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

        return user == null ? null : MapToDto(user);
    }

    public async Task<bool> IsEmailConfirmedAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user != null && user.EmailConfirmed;
    }

    public async Task<bool> IsUserActiveAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user is { IsActive: true, IsDeleted: false };
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new InvalidOperationException("User not found");

        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<(bool Succeeded, string[] Errors)> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return (false, new[] { "User not found" });
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new InvalidOperationException("User not found");

        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(Guid userId, string token, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return (false, new[] { "User not found" });
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<IList<string>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Array.Empty<string>();

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<(bool Succeeded, string[] Errors)> AddToRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return (false, new[] { "User not found" });
        }

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(role))
        {
            var roleResult = await _roleManager.CreateAsync(new ApplicationRole(role));
            if (!roleResult.Succeeded)
            {
                return (false, roleResult.Errors.Select(e => e.Description).ToArray());
            }
        }

        var result = await _userManager.AddToRoleAsync(user, role);

        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task UpdateLastLoginAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return;

        user.UpdateLastLogin();
        await _userManager.UpdateAsync(user);
    }

    public async Task<(bool Succeeded, string[] Errors)> DeactivateUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return (false, new[] { "User not found" });
        }

        user.Deactivate();
        var result = await _userManager.UpdateAsync(user);

        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> SoftDeleteUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return (false, new[] { "User not found" });
        }

        user.SoftDelete();
        var result = await _userManager.UpdateAsync(user);

        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    private static UserDto MapToDto(ApplicationUser user)
    {
        return new UserDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            user.EmailConfirmed,
            user.IsActive,
            user.IsDeleted,
            user.OrganizationId
        );
    }

    // Admin User Management Methods

    public async Task<(List<UserAdminDto> Users, int TotalCount)> GetUsersAsync(
        string? searchTerm = null,
        string? roleFilter = null,
        string? statusFilter = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _userManager.Users.Where(u => !u.IsDeleted);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.Email!.Contains(searchTerm) ||
                u.FullName.Contains(searchTerm) ||
                u.UserName!.Contains(searchTerm));
        }

        // Apply status filter
        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            switch (statusFilter.ToLowerInvariant())
            {
                case "active":
                    query = query.Where(u => u.IsActive);
                    break;
                case "pending":
                    query = query.Where(u => !u.EmailConfirmed);
                    break;
                case "suspended":
                    query = query.Where(u => !u.IsActive);
                    break;
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = new List<UserAdminDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            // Filter by role if specified
            if (!string.IsNullOrWhiteSpace(roleFilter) &&
                !roles.Contains(roleFilter, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            items.Add(new UserAdminDto(
                user.Id,
                user.Email ?? string.Empty,
                user.FullName,
                user.PhoneNumber,
                roles.ToList(),
                GetUserStatus(user),
                user.IsActive,
                user.EmailConfirmed,
                user.CreatedAt,
                user.LastLoginAt
            ));
        }

        return (items, totalCount);
    }

    public async Task<UserMetricsDto> GetUserMetricsAsync(CancellationToken cancellationToken = default)
    {
        // Total users
        var totalUsers = await _userManager.Users.CountAsync(u => !u.IsDeleted, cancellationToken);
        var lastMonthDate = DateTime.UtcNow.AddMonths(-1);
        var lastMonthUsers = await _userManager.Users.CountAsync(u => !u.IsDeleted && u.CreatedAt <= lastMonthDate, cancellationToken);
        var totalUsersChange = lastMonthUsers > 0
            ? ((decimal)(totalUsers - lastMonthUsers) / lastMonthUsers) * 100
            : totalUsers > 0 ? 100 : 0;

        // Active doctors - get users in Ophthalmologist role
        var activeDoctors = await GetUsersInRoleCountAsync("Ophthalmologist", true, cancellationToken);

        // Pending approvals
        var pendingApprovals = await _userManager.Users
            .CountAsync(u => !u.IsDeleted && (!u.EmailConfirmed || !u.IsActive), cancellationToken);

        return new UserMetricsDto(
            totalUsers,
            Math.Round(totalUsersChange, 1),
            activeDoctors,
            0, // Would need historical data for change
            0, // Would need screening data - passed separately
            0, // Would need screening data
            pendingApprovals
        );
    }

    public async Task<int> GetUsersInRoleCountAsync(string role, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(role);
        return activeOnly
            ? usersInRole.Count(u => u.IsActive && !u.IsDeleted)
            : usersInRole.Count(u => !u.IsDeleted);
    }

    public async Task<(bool Succeeded, string[] Errors)> RemoveFromRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return (false, new[] { "User not found" });
        }

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> ActivateUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return (false, new[] { "User not found" });
        }

        user.Activate();
        var result = await _userManager.UpdateAsync(user);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> ApproveUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return (false, new[] { "User not found" });
        }

        user.EmailConfirmed = true;
        user.Activate();
        user.UpdatedAt = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<int> GetPendingApprovalsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _userManager.Users
            .CountAsync(u => !u.IsDeleted && (!u.EmailConfirmed || !u.IsActive), cancellationToken);
    }

    private static string GetUserStatus(ApplicationUser user)
    {
        if (!user.IsActive) return "Suspended";
        if (!user.EmailConfirmed) return "Pending";
        if (user.LastLoginAt.HasValue && user.LastLoginAt.Value > DateTime.UtcNow.AddMinutes(-15)) return "Online";
        return "Active";
    }
}
