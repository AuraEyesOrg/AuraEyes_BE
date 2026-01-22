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
}
