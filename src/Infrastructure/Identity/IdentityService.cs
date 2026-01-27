using System.Text;
using System.Text.Encodings.Web;
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

    // Number of recovery codes to generate
    private const int DefaultRecoveryCodesCount = 10;
    
    // Issuer name for TOTP authenticator apps
    private const string AuthenticatorIssuer = "AuraEyes";

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
            user.OrganizationId,
            user.TwoFactorEnabled
        );
    }

    #region Two-Factor Authentication (2FA)

    public async Task<bool> IsTwoFactorEnabledAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user != null && await _userManager.GetTwoFactorEnabledAsync(user);
    }

    public async Task<string?> GetAuthenticatorKeyAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return null;

        return await _userManager.GetAuthenticatorKeyAsync(user);
    }

    public async Task<string> GetOrCreateAuthenticatorKeyAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new InvalidOperationException("User not found");

        // Reset the authenticator key to generate a new one
        await _userManager.ResetAuthenticatorKeyAsync(user);
        
        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        return key ?? throw new InvalidOperationException("Failed to generate authenticator key");
    }

    public async Task<(bool Succeeded, string[] Errors, string[]? RecoveryCodes)> EnableTwoFactorAsync(
        Guid userId, 
        string verificationCode)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return (false, new[] { "User not found" }, null);

        // Verify the TOTP code
        var isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            verificationCode);

        if (!isCodeValid)
            return (false, new[] { "Invalid verification code" }, null);

        // Enable 2FA
        var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray(), null);

        // Generate recovery codes
        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, DefaultRecoveryCodesCount);
        
        return (true, Array.Empty<string>(), recoveryCodes?.ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> DisableTwoFactorAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return (false, new[] { "User not found" });

        // Disable 2FA
        var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        // Reset authenticator key
        await _userManager.ResetAuthenticatorKeyAsync(user);

        return (true, Array.Empty<string>());
    }

    public async Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        return await _userManager.VerifyTwoFactorTokenAsync(
            user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            code);
    }

    public async Task<(bool Succeeded, string[] Errors)> VerifyRecoveryCodeAsync(Guid userId, string recoveryCode)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return (false, new[] { "User not found" });

        var result = await _userManager.RedeemTwoFactorRecoveryCodeAsync(user, recoveryCode);
        
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<string[]> GenerateNewRecoveryCodesAsync(Guid userId, int count = 10)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new InvalidOperationException("User not found");

        var codes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, count);
        return codes?.ToArray() ?? Array.Empty<string>();
    }

    public async Task<int> GetRecoveryCodesCountAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return 0;

        return await _userManager.CountRecoveryCodesAsync(user);
    }

    /// <summary>
    /// Generate otpauth:// URI for QR code (compatible with Google Authenticator, etc.)
    /// </summary>
    public string GenerateAuthenticatorUri(string email, string sharedKey)
    {
        return $"otpauth://totp/{UrlEncoder.Default.Encode(AuthenticatorIssuer)}:{UrlEncoder.Default.Encode(email)}" +
               $"?secret={sharedKey}" +
               $"&issuer={UrlEncoder.Default.Encode(AuthenticatorIssuer)}" +
               "&digits=6";
    }

    /// <summary>
    /// Format the shared key with spaces for easier manual entry.
    /// </summary>
    public string FormatAuthenticatorKey(string key)
    {
        var result = new StringBuilder();
        var currentPosition = 0;
        
        while (currentPosition + 4 < key.Length)
        {
            result.Append(key.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        
        if (currentPosition < key.Length)
        {
            result.Append(key.AsSpan(currentPosition));
        }

        return result.ToString().ToUpperInvariant();
    }

    #endregion
}
