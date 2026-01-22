using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models.Auth;
using Domain.Entities;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Authentication endpoints for user registration, login, and token management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IEmailService _emailService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IIdentityService identityService,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IEmailService emailService,
        ApplicationDbContext context,
        ILogger<AuthController> logger)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _emailService = emailService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Register a new patient account.
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registration result with confirmation instructions</returns>
    [HttpPost("register/patient")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> RegisterPatient(
        [FromBody] RegisterPatientRequest request,
        CancellationToken cancellationToken)
    {
        var (succeeded, errors) = await _identityService.CreateUserWithRoleAsync(
            request.Email,
            request.Password,
            request.FullName,
            Roles.Patient,
            cancellationToken);

        if (!succeeded)
        {
            return BadRequest(AuthResponse.Failure(errors));
        }

        var user = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return BadRequest(AuthResponse.Failure("Failed to retrieve created user"));
        }

        // Create Patient profile
        var patient = new Patient(user.Id, null);
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate email confirmation token
        var confirmationToken = await _identityService.GenerateEmailConfirmationTokenAsync(user.Id);
        var confirmationLink = Url.Action(
            nameof(ConfirmEmail),
            "Auth",
            new { userId = user.Id, token = confirmationToken },
            Request.Scheme);

        await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink ?? "", cancellationToken);

        _logger.LogInformation("Patient registered: {Email}", request.Email);

        return Ok(new AuthResponse
        {
            Succeeded = true,
            Errors = new[] { "Registration successful. Please check your email to confirm your account." }
        });
    }

    /// <summary>
    /// Register a new ophthalmologist account.
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registration result with confirmation instructions</returns>
    [HttpPost("register/ophthalmologist")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> RegisterOphthalmologist(
        [FromBody] RegisterOphthalmologistRequest request,
        CancellationToken cancellationToken)
    {
        var (succeeded, errors) = await _identityService.CreateUserWithRoleAsync(
            request.Email,
            request.Password,
            request.FullName,
            Roles.Ophthalmologist,
            cancellationToken);

        if (!succeeded)
        {
            return BadRequest(AuthResponse.Failure(errors));
        }

        var user = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return BadRequest(AuthResponse.Failure("Failed to retrieve created user"));
        }

        // Create Ophthalmologist profile
        var ophthalmologist = new Ophthalmologist(user.Id, request.Bio, request.YearsOfExperience);
        _context.Ophthalmologists.Add(ophthalmologist);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate email confirmation token
        var confirmationToken = await _identityService.GenerateEmailConfirmationTokenAsync(user.Id);
        var confirmationLink = Url.Action(
            nameof(ConfirmEmail),
            "Auth",
            new { userId = user.Id, token = confirmationToken },
            Request.Scheme);

        await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink ?? "", cancellationToken);

        _logger.LogInformation("Ophthalmologist registered: {Email}", request.Email);

        return Ok(new AuthResponse
        {
            Succeeded = true,
            Errors = new[] { "Registration successful. Please check your email to confirm your account." }
        });
    }

    /// <summary>
    /// Authenticate user with email and password.
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT access token and refresh token</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return Unauthorized(AuthResponse.Failure("Invalid email or password"));
        }

        if (!user.IsActive)
        {
            return Unauthorized(AuthResponse.Failure("Account is deactivated. Please contact support."));
        }

        if (!await _identityService.IsEmailConfirmedAsync(user.Id))
        {
            return Unauthorized(AuthResponse.Failure("Please confirm your email before logging in."));
        }

        if (!await _identityService.CheckPasswordAsync(user.Id, request.Password))
        {
            return Unauthorized(AuthResponse.Failure("Invalid email or password"));
        }

        // Get user roles
        var roles = await _identityService.GetUserRolesAsync(user.Id);

        // Generate tokens
        var tokenResult = await _tokenService.GenerateAccessTokenAsync(
            user.Id,
            user.Email,
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
            request.DeviceInfo,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken);

        // Update last login
        await _identityService.UpdateLastLoginAsync(user.Id);

        _logger.LogInformation("User logged in: {Email}", request.Email);

        return Ok(AuthResponse.Success(
            tokenResult.AccessToken,
            refreshToken,
            tokenResult.ExpiresAt,
            new UserInfoResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles.ToArray(),
                EmailConfirmed = user.EmailConfirmed,
                OrganizationId = user.OrganizationId
            }));
    }

    /// <summary>
    /// Refresh access token using refresh token.
    /// Implements token rotation for security.
    /// </summary>
    /// <param name="request">Current access token and refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access token and rotated refresh token</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        // Validate access token (without lifetime validation)
        var userId = _tokenService.GetUserIdFromToken(request.AccessToken);
        var jti = _tokenService.GetJtiFromToken(request.AccessToken);

        if (userId == null || jti == null)
        {
            return Unauthorized(AuthResponse.Failure("Invalid access token"));
        }

        // Validate refresh token
        var refreshTokenHash = TokenService.HashToken(request.RefreshToken);
        var storedToken = await _refreshTokenService.GetByTokenHashAsync(refreshTokenHash, cancellationToken);

        if (storedToken == null)
        {
            return Unauthorized(AuthResponse.Failure("Invalid refresh token"));
        }

        if (!storedToken.IsActive)
        {
            // Token reuse detected - revoke all tokens for security
            _logger.LogWarning("Refresh token reuse detected for user {UserId}", storedToken.UserId);
            await _refreshTokenService.RevokeTokenFamilyAsync(storedToken.Id, "token_reuse_detected", cancellationToken);
            return Unauthorized(AuthResponse.Failure("Token has been revoked. Please login again."));
        }

        if (storedToken.JwtId != jti)
        {
            return Unauthorized(AuthResponse.Failure("Token mismatch"));
        }

        if (storedToken.UserId != userId)
        {
            return Unauthorized(AuthResponse.Failure("Token mismatch"));
        }

        // Get user
        var user = await _identityService.GetUserByIdAsync(userId.Value, cancellationToken);
        if (user == null || !user.IsActive)
        {
            return Unauthorized(AuthResponse.Failure("User not found or inactive"));
        }

        // Generate new tokens
        var roles = await _identityService.GetUserRolesAsync(user.Id);
        var tokenResult = await _tokenService.GenerateAccessTokenAsync(
            user.Id,
            user.Email,
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

        return Ok(AuthResponse.Success(
            tokenResult.AccessToken,
            newRefreshToken,
            tokenResult.ExpiresAt,
            new UserInfoResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles.ToArray(),
                EmailConfirmed = user.EmailConfirmed,
                OrganizationId = user.OrganizationId
            }));
    }

    /// <summary>
    /// Logout and revoke refresh token.
    /// </summary>
    /// <param name="request">Refresh token to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var refreshTokenHash = TokenService.HashToken(request.RefreshToken);
        var storedToken = await _refreshTokenService.GetByTokenHashAsync(refreshTokenHash, cancellationToken);

        if (storedToken != null)
        {
            await _refreshTokenService.RevokeTokenAsync(storedToken.Id, "logout", cancellationToken);
        }

        _logger.LogInformation("User logged out");

        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Logout from all devices by revoking all refresh tokens.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("uid")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            await _refreshTokenService.RevokeAllUserTokensAsync(userId, "logout_all", cancellationToken);
            _logger.LogInformation("All tokens revoked for user: {UserId}", userId);
        }

        return Ok(new { message = "Logged out from all devices" });
    }

    /// <summary>
    /// Confirm email address.
    /// </summary>
    /// <param name="request">User ID and confirmation token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] string userId,
        [FromQuery] string token,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return BadRequest(new { error = "Invalid user ID" });
        }

        var (succeeded, errors) = await _identityService.ConfirmEmailAsync(userGuid, token);

        if (!succeeded)
        {
            return BadRequest(new { errors });
        }

        _logger.LogInformation("Email confirmed for user: {UserId}", userId);

        return Ok(new { message = "Email confirmed successfully. You can now login." });
    }

    /// <summary>
    /// Request password reset email.
    /// </summary>
    /// <param name="request">Email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
        
        // Always return success to prevent email enumeration
        if (user == null)
        {
            _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
            return Ok(new { message = "If your email exists in our system, you will receive a password reset link." });
        }

        var resetToken = await _identityService.GeneratePasswordResetTokenAsync(user.Id);
        var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?userId={user.Id}&token={Uri.EscapeDataString(resetToken)}";

        await _emailService.SendPasswordResetAsync(user.Email, resetLink, cancellationToken);

        _logger.LogInformation("Password reset requested for: {Email}", request.Email);

        return Ok(new { message = "If your email exists in our system, you will receive a password reset link." });
    }

    /// <summary>
    /// Reset password with token.
    /// </summary>
    /// <param name="request">User ID, token, and new password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.UserId, out var userGuid))
        {
            return BadRequest(new { error = "Invalid user ID" });
        }

        var (succeeded, errors) = await _identityService.ResetPasswordAsync(
            userGuid,
            request.Token,
            request.NewPassword);

        if (!succeeded)
        {
            return BadRequest(new { errors });
        }

        // Revoke all refresh tokens for security
        await _refreshTokenService.RevokeAllUserTokensAsync(userGuid, "password_reset", cancellationToken);

        _logger.LogInformation("Password reset for user: {UserId}", request.UserId);

        return Ok(new { message = "Password reset successfully. Please login with your new password." });
    }

    /// <summary>
    /// Get current user information.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserInfoResponse>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("uid")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await _identityService.GetUserByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Unauthorized();
        }

        var roles = await _identityService.GetUserRolesAsync(userId);

        return Ok(new UserInfoResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles.ToArray(),
            EmailConfirmed = user.EmailConfirmed,
            OrganizationId = user.OrganizationId
        });
    }
}
