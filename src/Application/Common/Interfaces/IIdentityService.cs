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
    Guid? OrganizationId
);
