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
    private const string ProviderAvatarClaimType = "provider_avatar_url";

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

    public async Task<(bool Succeeded, Guid? UserId, string[] Errors)> CreateUserWalkInPatientAsync(
        string email,
        string password,
        string fullName,
        string role,
        Guid? organizationId = null,
        UserProfileWalkInDto? userProfile = null,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            OrganizationId = organizationId
        };

        if (userProfile != null)
        {
            user.PhoneNumber = userProfile.PhoneNumber;
            user.DateOfBirth = userProfile.DateOfBirth;
            if (userProfile.Gender.HasValue)
            {
                user.Gender = (Domain.Enums.Gender)userProfile.Gender.Value;
            }
            user.Address = userProfile.Address;
            user.AvatarUrl = userProfile.AvatarUrl;
            user.CitizenId = userProfile.CitizenId;
            if (!string.IsNullOrEmpty(userProfile.FullName))
            {
                user.FullName = userProfile.FullName;
            }
        }

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return (false, null, result.Errors.Select(e => e.Description).ToArray());
        }

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(role))
        {
            var roleResult = await _roleManager.CreateAsync(new ApplicationRole(role));
            if (!roleResult.Succeeded)
            {
                return (false, null, roleResult.Errors.Select(e => e.Description).ToArray());
            }
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, role);

        return (addRoleResult.Succeeded, user.Id, addRoleResult.Errors.Select(e => e.Description).ToArray());
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

        return user == null ? null : await MapToDtoAsync(user);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);

        return user == null ? null : await MapToDtoAsync(user);
    }

    public async Task<bool> IsPhoneNumberInUseByOrganizationAsync(
        Guid organizationId,
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var normalizedPhoneNumber = NormalizePhone(phoneNumber);
        if (string.IsNullOrWhiteSpace(normalizedPhoneNumber))
        {
            return false;
        }

        var candidateSuffix = GetCandidatePhoneSuffix(normalizedPhoneNumber);

        var existingPhoneNumbers = await _userManager.Users
            .Where(u =>
                u.OrganizationId == organizationId &&
                !u.IsDeleted &&
                u.PhoneNumber != null &&
                u.PhoneNumber != string.Empty)
            .Where(u =>
                u.PhoneNumber!
                    .Replace(" ", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace(".", string.Empty)
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty)
                    .Replace("+", string.Empty)
                    .EndsWith(candidateSuffix))
            .Select(u => u.PhoneNumber!)
            .ToListAsync(cancellationToken);

        return existingPhoneNumbers
            .Select(NormalizePhone)
            .Any(p => p == normalizedPhoneNumber);
    }

    public async Task<bool> IsCitizenIdInUseByOrganizationAsync(
        Guid organizationId,
        string citizenId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(citizenId))
        {
            return false;
        }

        return await _userManager.Users
            .AnyAsync(u => u.OrganizationId == organizationId && u.CitizenId == citizenId && !u.IsDeleted, cancellationToken);
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

    public async Task<IReadOnlyList<Guid>> GetUserIdsByRoleAndOrganizationAsync(
        string role,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(role);

        return usersInRole
            .Where(u =>
                u.OrganizationId == organizationId &&
                u.IsActive &&
                !u.IsDeleted)
            .Select(u => u.Id)
            .ToList();
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

    private async Task<UserDto> MapToDtoAsync(ApplicationUser user)
    {
        var avatarUrl = await ResolveEffectiveAvatarUrlAsync(user);

        return new UserDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            user.EmailConfirmed,
            user.IsActive,
            user.IsDeleted,
            user.OrganizationId,
            user.TwoFactorEnabled,
            avatarUrl
        );
    }

    private async Task<string?> ResolveEffectiveAvatarUrlAsync(ApplicationUser user)
    {
        if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
            return user.AvatarUrl;

        var claims = await _userManager.GetClaimsAsync(user);
        var providerAvatar = claims.FirstOrDefault(c =>
            string.Equals(c.Type, ProviderAvatarClaimType, StringComparison.Ordinal));

        return string.IsNullOrWhiteSpace(providerAvatar?.Value)
            ? null
            : providerAvatar.Value;
    }

    private static string NormalizePhone(string phoneNumber)
    {
        return new string(phoneNumber
            .Where(char.IsDigit)
            .ToArray());
    }

    private static string GetCandidatePhoneSuffix(string normalizedPhoneNumber)
    {
        const int CandidateSuffixLength = 8;
        if (normalizedPhoneNumber.Length <= CandidateSuffixLength)
        {
            return normalizedPhoneNumber;
        }

        return normalizedPhoneNumber[^CandidateSuffixLength..];
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

    // ============ PROFILE MANAGEMENT ============

    public async Task<UserDetailsDto?> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
            return null;

        var avatarUrl = await ResolveEffectiveAvatarUrlAsync(user);

        return new UserDetailsDto
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            Address = user.Address,
            AvatarUrl = avatarUrl,
            CitizenId = user.CitizenId,
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };
    }

    public async Task<(bool Succeeded, string[] Errors)> UpdateUserProfileAsync(
        Guid userId,
        string fullName,
        string? phone,
        DateTime? dateOfBirth,
        int? gender,
        string? address,
        string? citizenId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
            return (false, new[] { "User not found" });

        user.FullName = fullName;
        user.PhoneNumber = phone;
        user.DateOfBirth = dateOfBirth;
        user.Gender = gender.HasValue ? (Domain.Enums.Gender)gender.Value : null;
        user.Address = address;
        user.CitizenId = citizenId;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> UpdateUserEmailAsync(
        Guid userId,
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
            return (false, new[] { "User not found" });
            
        if(!user.OrganizationId.HasValue)
            return (false, new[] { "Only organization users can have their email updated" });

        if (string.IsNullOrWhiteSpace(email))
            return (false, new[] { "Email is required" });

        var normalizedEmail = email.Trim();

        user.Email = normalizedEmail;
        user.UserName = normalizedEmail;
        user.EmailConfirmed = false;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> UpdateAvatarUrlAsync(
        Guid userId,
        string avatarUrl,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
            return (false, new[] { "User not found" });

        user.AvatarUrl = avatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> UpdateUserOrganizationAsync(
        Guid userId,
        Guid? organizationId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
            return (false, new[] { "User not found" });

        user.OrganizationId = organizationId;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
            return (false, new[] { "User not found" });

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
        {
            user.UpdatedAt = DateTime.UtcNow;
            user.MustChangePassword = false;
            await _userManager.UpdateAsync(user);
        }

        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> UpdateUserProfileAsync(
        Guid userId,
        string fullName,
        string? phone,
        DateTime? dateOfBirth,
        int? gender,
        string? address,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
            return (false, new[] { "User not found" });

        user.FullName = fullName;
        user.PhoneNumber = phone;
        user.DateOfBirth = dateOfBirth;
        user.Gender = gender.HasValue ? (Domain.Enums.Gender)gender.Value : null;
        user.Address = address;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }
}
