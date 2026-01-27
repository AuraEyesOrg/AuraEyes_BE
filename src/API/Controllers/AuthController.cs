using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Authentication endpoints for user registration, login, and token management.
/// </summary>
public class AuthController : BaseApiController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IEmailService _emailService;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IEmailService emailService,
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        ApplicationDbContext context,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _emailService = emailService;
        _identityService = identityService;
        _currentUserService = currentUserService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Register a new patient account.
    /// </summary>
    [HttpPost("register/patient")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterPatient(
        [FromBody] RegisterPatientRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return ErrorResponse("A user with this email already exists");
            }

            // Create user
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender.HasValue ? (Gender)request.Gender.Value : null
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(ApiResponseFactory.Error("Registration failed", errors));
            }

            // Add to Patient role
            await _userManager.AddToRoleAsync(user, Roles.Patient);

            // Create Patient profile
            var patient = new Patient(user.Id, null);
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken);

            // Generate email confirmation token
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(
                nameof(ConfirmEmail),
                "Auth",
                new { userId = user.Id, token = confirmationToken },
                Request.Scheme);

            await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink ?? "", cancellationToken);

            _logger.LogInformation("Patient registered: {Email}", request.Email);

            return OkResponse(new { userId = user.Id }, "Registration successful. Please check your email to confirm your account.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering patient: {Email}", request.Email);
            return InternalError("An error occurred during registration");
        }
    }

    /// <summary>
    /// Register a new ophthalmologist account.
    /// </summary>
    [HttpPost("register/ophthalmologist")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterOphthalmologist(
        [FromBody] RegisterOphthalmologistRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return ErrorResponse("A user with this email already exists");
            }

            // Create user
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                OrganizationId = request.OrganizationId
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(ApiResponseFactory.Error("Registration failed", errors));
            }

            // Add to Ophthalmologist role
            await _userManager.AddToRoleAsync(user, Roles.Ophthalmologist);

            // Create Ophthalmologist profile
            var ophthalmologist = new Ophthalmologist(user.Id, request.Bio, request.YearsOfExperience);
            _context.Ophthalmologists.Add(ophthalmologist);
            await _context.SaveChangesAsync(cancellationToken);

            // Generate email confirmation token
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(
                nameof(ConfirmEmail),
                "Auth",
                new { userId = user.Id, token = confirmationToken },
                Request.Scheme);

            await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink ?? "", cancellationToken);

            _logger.LogInformation("Ophthalmologist registered: {Email}", request.Email);

            return OkResponse(new { userId = user.Id }, "Registration successful. Please check your email to confirm your account.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering ophthalmologist: {Email}", request.Email);
            return InternalError("An error occurred during registration");
        }
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
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || user.IsDeleted)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid email or password"));
            }

            if (!user.IsActive)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Account is deactivated. Please contact support."));
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid email or password"));
            }

            // Check email confirmation (optional - can be disabled for development)
            if (!user.EmailConfirmed)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Please confirm your email before logging in."));
            }

            // Check if 2FA is enabled
            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                _logger.LogInformation("2FA required for user: {Email}", request.Email);
                
                var twoFactorResponse = new TwoFactorRequiredResponse
                {
                    RequiresTwoFactor = true,
                    UserId = user.Id,
                    Message = "Two-factor authentication is required. Please enter your verification code from your authenticator app."
                };
                
                return OkResponse(twoFactorResponse, "Two-factor authentication required");
            }

            // Complete login (no 2FA)
            return await CompleteLoginAsync(user, request.DeviceInfo, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login: {Email}", request.Email);
            return InternalError("An error occurred during login");
        }
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
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted || !user.IsActive)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("User not found or inactive"));
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return BadRequest(ApiResponseFactory.Error("Two-factor authentication is not enabled for this account"));
            }

            bool isValidCode;

            if (request.UseRecoveryCode)
            {
                // Verify recovery code
                var result = await _identityService.VerifyRecoveryCodeAsync(user.Id, request.Code);
                isValidCode = result.Succeeded;
                
                if (isValidCode)
                {
                    _logger.LogWarning("Recovery code used for user: {UserId}", user.Id);
                }
            }
            else
            {
                // Verify TOTP code from authenticator app
                isValidCode = await _identityService.VerifyTwoFactorCodeAsync(user.Id, request.Code);
            }

            if (!isValidCode)
            {
                _logger.LogWarning("Invalid 2FA code for user: {UserId}", user.Id);
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid verification code"));
            }

            return await CompleteLoginAsync(user, request.DeviceInfo, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during 2FA verification for user: {UserId}", request.UserId);
            return InternalError("An error occurred during verification");
        }
    }

    /// <summary>
    /// Helper method to complete login and generate tokens.
    /// </summary>
    private async Task<IActionResult> CompleteLoginAsync(
        ApplicationUser user, 
        string? deviceInfo, 
        CancellationToken cancellationToken)
    {
        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate tokens
        var tokenResult = await _tokenService.GenerateAccessTokenAsync(
            user.Id,
            user.Email!,
            user.FullName,
            roles);

        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenHash = TokenService.HashToken(refreshToken);

        // Store refresh token
        await _refreshTokenService.CreateRefreshTokenAsync(
            user.Id,
            refreshTokenHash,
            tokenResult.Jti,
            7, // 7 days
            deviceInfo,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken);

        // Update last login
        user.UpdateLastLogin();
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User logged in: {Email}", user.Email);

        var response = new AuthResponse
        {
            Succeeded = true,
            AccessToken = tokenResult.AccessToken,
            RefreshToken = refreshToken,
            ExpiresAt = tokenResult.ExpiresAt,
            User = new UserInfoResponse
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToArray(),
                EmailConfirmed = user.EmailConfirmed,
                OrganizationId = user.OrganizationId,
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user)
            }
        };

        return OkResponse(response, "Login successful");
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
        try
        {
            // Validate access token (without lifetime validation)
            var userId = _tokenService.GetUserIdFromToken(request.AccessToken);
            var jti = _tokenService.GetJtiFromToken(request.AccessToken);

            if (userId == null || jti == null)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid access token"));
            }

            // Validate refresh token
            var refreshTokenHash = TokenService.HashToken(request.RefreshToken);
            var storedToken = await _refreshTokenService.GetByTokenHashAsync(refreshTokenHash, cancellationToken);

            if (storedToken == null)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid refresh token"));
            }

            if (!storedToken.IsActive)
            {
                // Token reuse detected - revoke all tokens for security
                _logger.LogWarning("Refresh token reuse detected for user {UserId}", storedToken.UserId);
                await _refreshTokenService.RevokeTokenFamilyAsync(storedToken.Id, "token_reuse_detected", cancellationToken);
                return Unauthorized(ApiResponseFactory.Unauthorized("Token has been revoked. Please login again."));
            }

            if (storedToken.JwtId != jti || storedToken.UserId != userId)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Token mismatch"));
            }

            // Get user
            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null || !user.IsActive || user.IsDeleted)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("User not found or inactive"));
            }

            // Generate new tokens
            var roles = await _userManager.GetRolesAsync(user);
            var tokenResult = await _tokenService.GenerateAccessTokenAsync(
                user.Id,
                user.Email!,
                user.FullName,
                roles);

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenHash = TokenService.HashToken(newRefreshToken);

            // Rotate refresh token
            await _refreshTokenService.RotateRefreshTokenAsync(
                storedToken.Id,
                newRefreshTokenHash,
                tokenResult.Jti,
                7,
                null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                cancellationToken);

            _logger.LogInformation("Token refreshed for user: {UserId}", user.Id);

            var response = new AuthResponse
            {
                Succeeded = true,
                AccessToken = tokenResult.AccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = tokenResult.ExpiresAt,
                User = new UserInfoResponse
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Roles = roles.ToArray(),
                    EmailConfirmed = user.EmailConfirmed,
                    OrganizationId = user.OrganizationId,
                    TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user)
                }
            };

            return OkResponse(response, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return InternalError("An error occurred while refreshing token");
        }
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
        try
        {
            var refreshTokenHash = TokenService.HashToken(request.RefreshToken);
            var storedToken = await _refreshTokenService.GetByTokenHashAsync(refreshTokenHash, cancellationToken);

            if (storedToken != null)
            {
                await _refreshTokenService.RevokeTokenAsync(storedToken.Id, "logout", cancellationToken);
            }

            _logger.LogInformation("User logged out");
            return OkResponse("Logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return InternalError("An error occurred during logout");
        }
    }

    /// <summary>
    /// Logout from all devices by revoking all refresh tokens.
    /// </summary>
    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (userId.HasValue)
            {
                await _refreshTokenService.RevokeAllUserTokensAsync(userId.Value, "logout_all", cancellationToken);
                _logger.LogInformation("All tokens revoked for user: {UserId}", userId);
            }

            return OkResponse("Logged out from all devices");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout all");
            return InternalError("An error occurred during logout");
        }
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
        [FromQuery] string token)
    {
        try
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return ErrorResponse("Invalid user ID");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponseFactory.NotFound("User not found"));
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(ApiResponseFactory.Error("Email confirmation failed", errors));
            }

            _logger.LogInformation("Email confirmed for user: {UserId}", userId);
            return OkResponse("Email confirmed successfully. You can now login.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming email for user: {UserId}", userId);
            return InternalError("An error occurred during email confirmation");
        }
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
        try
        {
            // Always return success to prevent email enumeration
            var user = await _userManager.FindByEmailAsync(request.Email);
            
            if (user != null && !user.IsDeleted)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?userId={user.Id}&token={Uri.EscapeDataString(resetToken)}";

                await _emailService.SendPasswordResetAsync(user.Email!, resetLink, cancellationToken);
                _logger.LogInformation("Password reset requested for: {Email}", request.Email);
            }
            else
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
            }

            return OkResponse("If your email exists in our system, you will receive a password reset link.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password: {Email}", request.Email);
            return InternalError("An error occurred while processing your request");
        }
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
        try
        {
            if (!Guid.TryParse(request.UserId, out var userGuid))
            {
                return ErrorResponse("Invalid user ID");
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound(ApiResponseFactory.NotFound("User not found"));
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(ApiResponseFactory.Error("Password reset failed", errors));
            }

            // Revoke all refresh tokens for security
            await _refreshTokenService.RevokeAllUserTokensAsync(userGuid, "password_reset", cancellationToken);

            _logger.LogInformation("Password reset for user: {UserId}", request.UserId);
            return OkResponse("Password reset successfully. Please login with your new password.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user: {UserId}", request.UserId);
            return InternalError("An error occurred while resetting password");
        }
    }

    /// <summary>
    /// Get current user information.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid token"));
            }

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null || user.IsDeleted)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("User not found"));
            }

            var roles = await _userManager.GetRolesAsync(user);

            var response = new UserInfoResponse
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToArray(),
                EmailConfirmed = user.EmailConfirmed,
                OrganizationId = user.OrganizationId,
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user)
            };

            return OkResponse(response, "User information retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return InternalError("An error occurred while retrieving user information");
        }
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
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            
            if (user != null && !user.EmailConfirmed && !user.IsDeleted)
            {
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(
                    nameof(ConfirmEmail),
                    "Auth",
                    new { userId = user.Id, token = confirmationToken },
                    Request.Scheme);

                await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink ?? "", cancellationToken);
                _logger.LogInformation("Confirmation email resent to: {Email}", request.Email);
            }

            return OkResponse("If your email exists and is not confirmed, you will receive a confirmation link.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resending confirmation email: {Email}", request.Email);
            return InternalError("An error occurred while processing your request");
        }
    }
}
