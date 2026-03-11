using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Models.Auth;

/// <summary>
/// Login request DTO.
/// </summary>
public record LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
    
    [Required]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;
    
    /// <summary>
    /// Optional: Device information for refresh token tracking.
    /// </summary>
    public string? DeviceInfo { get; init; }
}

/// <summary>
/// Register patient request DTO.
/// </summary>
public record RegisterPatientRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
    
    [Required]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;
    
    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; init; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string FullName { get; init; } = string.Empty;
    
    public string? Address { get; init; }
    
    public DateTime? DateOfBirth { get; init; }
    
    public int? Gender { get; init; }
}

/// <summary>
/// Register ophthalmologist request DTO.
/// Supports both JSON body and multipart form data (with file uploads).
/// </summary>
public class RegisterOphthalmologistRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;
    
    [Phone]
    public string? Phone { get; set; }
    
    public string? Bio { get; set; }
    
    [Range(0, 70)]
    public int YearsOfExperience { get; set; }
    
    public Guid? OrganizationId { get; set; }
    
    /// <summary>
    /// Medical license image. Accepts PDF, JPG, PNG.
    /// </summary>
    public IFormFile? LicenseImage { get; set; }
    
    /// <summary>
    /// Medical degree image. Accepts PDF, JPG, PNG.
    /// </summary>
    public IFormFile? DegreeImage { get; set; }
}

/// <summary>
/// Authentication response DTO.
/// </summary>
public record AuthResponse
{
    public bool Succeeded { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public UserInfoResponse? User { get; init; }
    public string[] Errors { get; init; } = Array.Empty<string>();
    
    public static AuthResponse Success(
        string accessToken, 
        string refreshToken, 
        DateTime expiresAt,
        UserInfoResponse user)
    {
        return new AuthResponse
        {
            Succeeded = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = user
        };
    }
    
    public static AuthResponse Failure(params string[] errors)
    {
        return new AuthResponse
        {
            Succeeded = false,
            Errors = errors
        };
    }
}

/// <summary>
/// User info in auth response.
/// </summary>
public record UserInfoResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string[] Roles { get; init; } = Array.Empty<string>();
    public bool EmailConfirmed { get; init; }
    public Guid? OrganizationId { get; init; }
    
    /// <summary>
    /// Indicates if 2FA is enabled for this user.
    /// </summary>
    public bool TwoFactorEnabled { get; init; }
    
    /// <summary>
    /// Indicates if the ophthalmologist's credentials have been verified.
    /// Null for non-ophthalmologist roles.
    /// </summary>
    public bool? IsVerified { get; init; }
    
    /// <summary>
    /// Ophthalmologist verification status (PendingVerification, Approved, Rejected).
    /// Null for non-ophthalmologist roles.
    /// </summary>
    public string? VerificationStatus { get; init; }
}

/// <summary>
/// Refresh token request DTO.
/// </summary>
public record RefreshTokenRequest
{
    [Required]
    public string AccessToken { get; init; } = string.Empty;
    
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}

/// <summary>
/// Forgot password request DTO.
/// </summary>
public record ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
}

/// <summary>
/// Reset password request DTO.
/// </summary>
public record ResetPasswordRequest
{
    [Required]
    public string UserId { get; init; } = string.Empty;
    
    [Required]
    public string Token { get; init; } = string.Empty;
    
    [Required]
    [MinLength(8)]
    public string NewPassword { get; init; } = string.Empty;
    
    [Required]
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; init; } = string.Empty;
}

/// <summary>
/// Confirm email request DTO.
/// </summary>
public record ConfirmEmailRequest
{
    [Required]
    public string UserId { get; init; } = string.Empty;
    
    [Required]
    public string Token { get; init; } = string.Empty;
}

#region Two-Factor Authentication DTOs

/// <summary>
/// Response when 2FA setup is initiated.
/// Contains the shared key and QR code URI for authenticator apps.
/// </summary>
public record TwoFactorSetupResponse
{
    public string SharedKey { get; init; } = string.Empty;
    public string AuthenticatorUri { get; init; } = string.Empty;
    public string FormattedKey { get; init; } = string.Empty;
}

public record EnableTwoFactorRequest
{
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string VerificationCode { get; init; } = string.Empty;
}

/// <summary>
/// Response after successfully enabling 2FA.
/// Contains recovery codes that should be saved securely.
/// </summary>
public record EnableTwoFactorResponse
{
    public bool Succeeded { get; init; }
    public string[] RecoveryCodes { get; init; } = Array.Empty<string>();
    public string[] Errors { get; init; } = Array.Empty<string>();
}

public record DisableTwoFactorRequest
{
    [Required]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;
}

public record VerifyTwoFactorRequest
{
    [Required]
    public Guid UserId { get; init; }
    
    [Required]
    [StringLength(10, MinimumLength = 6)]
    public string Code { get; init; } = string.Empty;
    
    /// <summary>
    /// Set to true if the code is a recovery code instead of TOTP.
    /// </summary>
    public bool UseRecoveryCode { get; init; }
    public string? DeviceInfo { get; init; }
}

public record TwoFactorStatusResponse
{
    public bool IsEnabled { get; init; }
    public int RecoveryCodesRemaining { get; init; }
    public bool HasAuthenticator { get; init; }
}

public record GenerateRecoveryCodesRequest
{
    [Required]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;
}

public record RecoveryCodesResponse
{
    public bool Succeeded { get; init; }
    public string[] RecoveryCodes { get; init; } = Array.Empty<string>();
    public string[] Errors { get; init; } = Array.Empty<string>();
}

public record TwoFactorRequiredResponse
{
    public bool RequiresTwoFactor { get; init; } = true;
    public Guid UserId { get; init; }
    public string Message { get; init; } = "Two-factor authentication is required.";
}

#endregion