using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Authentication endpoints for user registration, login, and token management.
/// Uses IAuthService to delegate all business logic.
/// </summary>
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuthController> _logger;
    private readonly string _frontendUrl;

    public AuthController(
        IAuthService authService,
        ICurrentUserService currentUserService,
        ILogger<AuthController> logger,
        IConfiguration configuration)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _logger = logger;
        _frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:3000";
    }

    /// <summary>
    /// Register a new patient account.
    /// </summary>
    [HttpPost("register/patient")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterPatient(
        [FromBody] RegisterPatientRequest request,
        CancellationToken cancellationToken)
    {
        var confirmationUrlBase = $"{_frontendUrl}/confirm-email";
        var result = await _authService.RegisterPatientAsync(request, confirmationUrlBase!, cancellationToken);

        return HandleResult(result, result.Data?.Message ?? "Registration successful");
    }

    /// <summary>
    /// Register a new ophthalmologist account.
    /// Accepts multipart form data with optional credential file uploads.
    /// </summary>
    [HttpPost("register/ophthalmologist")]
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterOphthalmologist(
        [FromForm] RegisterOphthalmologistRequest request,
        CancellationToken cancellationToken)
    {
        var confirmationUrlBase = $"{_frontendUrl}/confirm-email";
        var result = await _authService.RegisterOphthalmologistAsync(request, confirmationUrlBase!, cancellationToken);

        return HandleResult(result, result.Data?.Message ?? "Registration successful");
    }

    /// <summary>
    /// Submit a new organisation onboarding request.
    /// </summary>
    [HttpPost("register/organisation")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<OrganisationRegistrationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterOrganisation(
        [FromBody] RegisterOrganisationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterOrganisationAsync(request, cancellationToken);
        return HandleResult(result, result.Data?.Message ?? "Organisation registration submitted successfully");
    }

    [HttpPost("google-login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TwoFactorRequiredResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GoogleLogin(
        [FromBody] GoogleLoginRequest request,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.GoogleLoginAsync(request, ipAddress, cancellationToken);

        if (result.IsUnauthorized)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized(result.ErrorMessage));
        }

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponseFactory.Error("Google login failed", result.Errors));
        }

        var loginResponse = result.Data!;

        // Handle 2FA required
        if (loginResponse.RequiresTwoFactor)
        {
            var twoFactorResponse = new TwoFactorRequiredResponse
            {
                RequiresTwoFactor = true,
                UserId = loginResponse.TwoFactorUserId!.Value,
                Message = "Two-factor authentication is required. Please enter your verification code from your authenticator app."
            };
            return OkResponse(twoFactorResponse, "Two-factor authentication required");
        }

        return OkResponse(loginResponse.AuthResponse, "Google login successful");
    }

    /// <summary>
    /// Authenticate user with email and password.
    /// Returns TwoFactorRequiredResponse if 2FA is enabled.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TwoFactorRequiredResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LoginAsync(request, ipAddress, cancellationToken);

        if (result.IsUnauthorized)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized(result.ErrorMessage));
        }

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponseFactory.Error("Login failed", result.Errors));
        }

        var loginResponse = result.Data!;

        // Handle 2FA required
        if (loginResponse.RequiresTwoFactor)
        {
            var twoFactorResponse = new TwoFactorRequiredResponse
            {
                RequiresTwoFactor = true,
                UserId = loginResponse.TwoFactorUserId!.Value,
                Message = "Two-factor authentication is required. Please enter your verification code from your authenticator app."
            };
            return OkResponse(twoFactorResponse, "Two-factor authentication required");
        }

        return OkResponse(loginResponse.AuthResponse, "Login successful");
    }

    /// <summary>
    /// Complete login after 2FA verification.
    /// </summary>
    [HttpPost("login/verify-2fa")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyTwoFactorLogin(
        [FromBody] VerifyTwoFactorRequest request,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.VerifyTwoFactorLoginAsync(request, ipAddress, cancellationToken);

        return HandleResult(result, "Login successful");
    }

    /// <summary>
    /// Refresh access token using refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.RefreshTokenAsync(
            request.AccessToken,
            request.RefreshToken,
            ipAddress,
            cancellationToken);

        return HandleResult(result, "Token refreshed successfully");
    }

    /// <summary>
    /// Logout and revoke refresh token.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LogoutAsync(request.RefreshToken, cancellationToken);

        return result.IsSuccess
            ? OkResponse("Logged out successfully")
            : BadRequest(ApiResponseFactory.Error("Logout failed", result.Errors));
    }

    /// <summary>
    /// Logout from all devices by revoking all refresh tokens.
    /// </summary>
    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("Invalid token"));
        }

        var result = await _authService.LogoutAllAsync(_currentUserService.UserId.Value, cancellationToken);

        return result.IsSuccess
            ? OkResponse("Logged out from all devices")
            : BadRequest(ApiResponseFactory.Error("Logout failed", result.Errors));
    }

    /// <summary>
    /// Confirm email address.
    /// </summary>
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] string userId,
        [FromQuery] string token,
        CancellationToken cancellationToken)
    {
        var result = await _authService.ConfirmEmailAsync(userId, token, cancellationToken);

        if (result.IsNotFound)
        {
            return NotFound(ApiResponseFactory.NotFound(result.ErrorMessage));
        }

        return result.IsSuccess
            ? OkResponse("Email confirmed successfully. You can now login.")
            : BadRequest(ApiResponseFactory.Error("Email confirmation failed", result.Errors));
    }

    /// <summary>
    /// Request password reset email.
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var resetUrlBase = $"{Request.Scheme}://{Request.Host}/reset-password";
        await _authService.ForgotPasswordAsync(request.Email, resetUrlBase, cancellationToken);

        // Always return success to prevent email enumeration
        return OkResponse("If your email exists in our system, you will receive a password reset link.");
    }

    /// <summary>
    /// Reset password with token.
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.ResetPasswordAsync(request, cancellationToken);

        if (result.IsNotFound)
        {
            return NotFound(ApiResponseFactory.NotFound(result.ErrorMessage));
        }

        return result.IsSuccess
            ? OkResponse("Password reset successfully. Please login with your new password.")
            : BadRequest(ApiResponseFactory.Error("Password reset failed", result.Errors));
    }

    /// <summary>
    /// Get current user information.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("Invalid token"));
        }

        var result = await _authService.GetCurrentUserAsync(_currentUserService.UserId.Value, cancellationToken);

        return HandleResult(result, "User information retrieved successfully");
    }

    /// <summary>
    /// Resend email confirmation.
    /// </summary>
    [HttpPost("resend-confirmation")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendConfirmation(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var confirmationUrlBase = $"{_frontendUrl}/confirm-email";
        await _authService.ResendConfirmationAsync(request.Email, confirmationUrlBase!, cancellationToken);

        return OkResponse("If your email exists and is not confirmed, you will receive a confirmation link.");
    }
}
