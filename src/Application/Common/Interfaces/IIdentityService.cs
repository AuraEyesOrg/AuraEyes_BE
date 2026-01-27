namespace Application.Common.Interfaces;

/// <summary>
/// Identity service interface for authentication operations.
/// Abstraction over ASP.NET Core Identity for use in Application layer.
/// </summary>
public interface IIdentityService
{
    // User Management
    Task<(bool Succeeded, string[] Errors)> CreateUserAsync(
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, string[] Errors)> CreateUserWithRoleAsync(
        string email,
        string password,
        string fullName,
        string role,
        CancellationToken cancellationToken = default);

    Task<bool> CheckPasswordAsync(Guid userId, string password);
    
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<bool> IsEmailConfirmedAsync(Guid userId);
    
    Task<bool> IsUserActiveAsync(Guid userId);

    // Email Confirmation
    Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
    
    Task<(bool Succeeded, string[] Errors)> ConfirmEmailAsync(Guid userId, string token);

    // Password Reset
    Task<string> GeneratePasswordResetTokenAsync(Guid userId);
    
    Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(Guid userId, string token, string newPassword);

    // Role Management
    Task<IList<string>> GetUserRolesAsync(Guid userId);
    
    Task<(bool Succeeded, string[] Errors)> AddToRoleAsync(Guid userId, string role);
    
    Task<bool> IsInRoleAsync(Guid userId, string role);

    // Account Management
    Task UpdateLastLoginAsync(Guid userId);
    
    Task<(bool Succeeded, string[] Errors)> DeactivateUserAsync(Guid userId);
    
    Task<(bool Succeeded, string[] Errors)> SoftDeleteUserAsync(Guid userId);

    // Two-Factor Authentication (2FA) - TOTP Authenticator
    
    Task<bool> IsTwoFactorEnabledAsync(Guid userId);
    
    /// <summary>
    /// Get the current authenticator key (without regenerating).
    /// </summary>
    Task<string?> GetAuthenticatorKeyAsync(Guid userId);
    
    Task<string> GetOrCreateAuthenticatorKeyAsync(Guid userId);
    
    string GenerateAuthenticatorUri(string email, string sharedKey);
    
    string FormatAuthenticatorKey(string key);
    
    Task<(bool Succeeded, string[] Errors, string[]? RecoveryCodes)> EnableTwoFactorAsync(Guid userId, string verificationCode);
    Task<(bool Succeeded, string[] Errors)> DisableTwoFactorAsync(Guid userId);
    Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code);
    Task<(bool Succeeded, string[] Errors)> VerifyRecoveryCodeAsync(Guid userId, string recoveryCode);
    Task<string[]> GenerateNewRecoveryCodesAsync(Guid userId, int count = 10);
    Task<int> GetRecoveryCodesCountAsync(Guid userId);
    // Admin User Management
    /// <summary>
    /// Get paginated list of users with optional filters.
    /// </summary>
    Task<(List<UserAdminDto> Users, int TotalCount)> GetUsersAsync(
        string? searchTerm = null,
        string? roleFilter = null,
        string? statusFilter = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user metrics for admin dashboard.
    /// </summary>
    Task<UserMetricsDto> GetUserMetricsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get count of users in a specific role.
    /// </summary>
    Task<int> GetUsersInRoleCountAsync(string role, bool activeOnly = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove user from role.
    /// </summary>
    Task<(bool Succeeded, string[] Errors)> RemoveFromRoleAsync(Guid userId, string role);

    /// <summary>
    /// Activate a user.
    /// </summary>
    Task<(bool Succeeded, string[] Errors)> ActivateUserAsync(Guid userId);

    /// <summary>
    /// Approve a pending user (confirms email and activates).
    /// </summary>
    Task<(bool Succeeded, string[] Errors)> ApproveUserAsync(Guid userId);

    /// <summary>
    /// Get pending approvals count.
    /// </summary>
    Task<int> GetPendingApprovalsCountAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// User DTO for cross-layer communication.
/// </summary>
public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    bool EmailConfirmed,
    bool IsActive,
    bool IsDeleted,
    Guid? OrganizationId,
    bool TwoFactorEnabled = false
);

/// <summary>
/// Extended User DTO for admin operations.
/// </summary>
public record UserAdminDto(
    Guid Id,
    string Email,
    string FullName,
    string? PhoneNumber,
    List<string> Roles,
    string Status,
    bool IsActive,
    bool EmailConfirmed,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);

/// <summary>
/// User metrics DTO for dashboard.
/// </summary>
public record UserMetricsDto(
    int TotalUsers,
    decimal TotalUsersMonthlyChange,
    int ActiveDoctors,
    decimal ActiveDoctorsChange,
    int PatientsScreened,
    decimal PatientsScreenedChange,
    int PendingApprovals
);
