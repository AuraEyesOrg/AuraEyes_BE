using System.ComponentModel.DataAnnotations;

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
/// </summary>
public record RegisterOphthalmologistRequest
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
    
    public string? Bio { get; init; }
    
    [Range(0, 70)]
    public int YearsOfExperience { get; init; }
    
    public Guid? OrganizationId { get; init; }
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
    public string[] Roles { get; init; } = Array.Empty<string>();
    public bool EmailConfirmed { get; init; }
    public Guid? OrganizationId { get; init; }
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
