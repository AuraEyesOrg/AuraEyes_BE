using Application.Common.Models;
using Application.Common.Models.Auth;

namespace Application.Common.Interfaces;

/// <summary>
/// Authentication service interface.
/// Orchestrates identity, token, and email services for auth operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register a new patient account.
    /// </summary>
    Task<Result<RegisterResponse>> RegisterPatientAsync(
        RegisterPatientRequest request,
        string confirmationUrlBase,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Register a new ophthalmologist account.
    /// </summary>
    Task<Result<RegisterResponse>> RegisterOphthalmologistAsync(
        RegisterOphthalmologistRequest request,
        string confirmationUrlBase,
        CancellationToken cancellationToken = default);

    Task<Result<OrganisationRegistrationResponse>> RegisterOrganisationAsync(
        RegisterOrganisationRequest request,
        CancellationToken cancellationToken = default);


    Task<Result<LoginResponse>> GoogleLoginAsync(
        GoogleLoginRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticate user with email and password.
    /// Returns TwoFactorRequired if 2FA is enabled.
    /// </summary>
    Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result<LookupAccountByCitizenIdResponse>> LookupAccountByCitizenIdAsync(
        string citizenId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Complete login after 2FA verification.
    /// </summary>
    Task<Result<AuthResponse>> VerifyTwoFactorLoginAsync(
        VerifyTwoFactorRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh access token using refresh token.
    /// </summary>
    Task<Result<AuthResponse>> RefreshTokenAsync(
        string accessToken,
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout and revoke refresh token.
    /// </summary>
    Task<Result> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout from all devices by revoking all refresh tokens.
    /// </summary>
    Task<Result> LogoutAllAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirm email address.
    /// </summary>
    Task<Result> ConfirmEmailAsync(
        string userId,
        string token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Request password reset email.
    /// </summary>
    Task<Result> ForgotPasswordAsync(
        string email,
        string resetUrlBase,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset password with token.
    /// </summary>
    Task<Result> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get current user information.
    /// </summary>
    Task<Result<UserInfoResponse>> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resend email confirmation.
    /// </summary>
    Task<Result> ResendConfirmationAsync(
        string email,
        string confirmationUrlBase,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Registration response DTO.
/// </summary>
public record RegisterResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Login response that can contain either AuthResponse or TwoFactorRequired.
/// </summary>
public record LoginResponse
{
    public bool RequiresTwoFactor { get; init; }
    public Guid? TwoFactorUserId { get; init; }
    public AuthResponse? AuthResponse { get; init; }

    public static LoginResponse TwoFactorRequired(Guid userId) => new()
    {
        RequiresTwoFactor = true,
        TwoFactorUserId = userId
    };

    public static LoginResponse Success(AuthResponse authResponse) => new()
    {
        RequiresTwoFactor = false,
        AuthResponse = authResponse
    };
}
