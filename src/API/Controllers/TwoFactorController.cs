using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Two-Factor Authentication (2FA) management endpoints.
/// Handles setup, enable, disable, and recovery codes for TOTP authenticator apps.
/// </summary>
[Authorize]
[Route("api/two-factor")]
public class TwoFactorController : BaseApiController
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<TwoFactorController> _logger;

    public TwoFactorController(
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        IRefreshTokenService refreshTokenService,
        ILogger<TwoFactorController> logger)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Get 2FA status for the current user.
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(ApiResponse<TwoFactorStatusResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetStatus()
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid token"));
            }

            var isEnabled = await _identityService.IsTwoFactorEnabledAsync(userId.Value);
            var recoveryCodesCount = await _identityService.GetRecoveryCodesCountAsync(userId.Value);
            var authenticatorKey = await _identityService.GetAuthenticatorKeyAsync(userId.Value);

            var response = new TwoFactorStatusResponse
            {
                IsEnabled = isEnabled,
                RecoveryCodesRemaining = recoveryCodesCount,
                HasAuthenticator = !string.IsNullOrEmpty(authenticatorKey)
            };

            return OkResponse(response, "2FA status retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting 2FA status for user: {UserId}", _currentUserService.UserId);
            return InternalError("An error occurred while retrieving 2FA status");
        }
    }

    /// <summary>
    /// Setup 2FA: Generate authenticator key and QR code URI.
    /// Call this endpoint first before enabling 2FA.
    /// </summary>
    [HttpPost("setup")]
    [ProducesResponseType(typeof(ApiResponse<TwoFactorSetupResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Setup()
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid token"));
            }

            // Check if already enabled
            if (await _identityService.IsTwoFactorEnabledAsync(userId.Value))
            {
                return BadRequest(ApiResponseFactory.Error("2FA is already enabled. Disable it first to set up a new authenticator."));
            }

            var email = _currentUserService.Email;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("User email not found"));
            }

            // Generate new authenticator key
            var sharedKey = await _identityService.GetOrCreateAuthenticatorKeyAsync(userId.Value);
            
            // Generate QR code URI and formatted key using IdentityService
            var authenticatorUri = _identityService.GenerateAuthenticatorUri(email, sharedKey);
            var formattedKey = _identityService.FormatAuthenticatorKey(sharedKey);

            var response = new TwoFactorSetupResponse
            {
                SharedKey = sharedKey,
                AuthenticatorUri = authenticatorUri,
                FormattedKey = formattedKey
            };

            _logger.LogInformation("2FA setup initiated for user: {UserId}", userId);
            return OkResponse(response, "Scan the QR code or enter the key manually in your authenticator app.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up 2FA for user: {UserId}", _currentUserService.UserId);
            return InternalError("An error occurred while setting up 2FA");
        }
    }

    /// <summary>
    /// Enable 2FA after verifying the TOTP code from authenticator app.
    /// </summary>
    [HttpPost("enable")]
    [ProducesResponseType(typeof(ApiResponse<EnableTwoFactorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Enable([FromBody] EnableTwoFactorRequest request)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid token"));
            }

            // Strip spaces and dashes from the verification code
            var verificationCode = request.VerificationCode
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);

            var (succeeded, errors, recoveryCodes) = await _identityService.EnableTwoFactorAsync(
                userId.Value, 
                verificationCode);

            if (!succeeded)
            {
                return BadRequest(ApiResponseFactory.Error("Failed to enable 2FA", errors.ToList()));
            }

            _logger.LogInformation("2FA enabled for user: {UserId}", userId);

            var response = new EnableTwoFactorResponse
            {
                Succeeded = true,
                RecoveryCodes = recoveryCodes ?? Array.Empty<string>()
            };

            return OkResponse(response, "Two-factor authentication has been enabled. Save your recovery codes securely!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling 2FA for user: {UserId}", _currentUserService.UserId);
            return InternalError("An error occurred while enabling 2FA");
        }
    }

    /// <summary>
    /// Disable 2FA for the current user. Requires password confirmation.
    /// </summary>
    [HttpPost("disable")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Disable(
        [FromBody] DisableTwoFactorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid token"));
            }

            // Verify password using IdentityService
            if (!await _identityService.CheckPasswordAsync(userId.Value, request.Password))
            {
                return BadRequest(ApiResponseFactory.Error("Invalid password"));
            }

            // Check if 2FA is enabled
            if (!await _identityService.IsTwoFactorEnabledAsync(userId.Value))
            {
                return BadRequest(ApiResponseFactory.Error("Two-factor authentication is not enabled"));
            }

            var (succeeded, errors) = await _identityService.DisableTwoFactorAsync(userId.Value);

            if (!succeeded)
            {
                return BadRequest(ApiResponseFactory.Error("Failed to disable 2FA", errors.ToList()));
            }

            // Revoke all refresh tokens for security
            await _refreshTokenService.RevokeAllUserTokensAsync(userId.Value, "2fa_disabled", cancellationToken);

            _logger.LogInformation("2FA disabled for user: {UserId}", userId);
            return OkResponse("Two-factor authentication has been disabled. Please login again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling 2FA for user: {UserId}", _currentUserService.UserId);
            return InternalError("An error occurred while disabling 2FA");
        }
    }

    /// <summary>
    /// Generate new recovery codes. Old codes will be invalidated. Requires password confirmation.
    /// </summary>
    [HttpPost("recovery-codes")]
    [ProducesResponseType(typeof(ApiResponse<RecoveryCodesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GenerateRecoveryCodes([FromBody] GenerateRecoveryCodesRequest request)
    {
        try
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return Unauthorized(ApiResponseFactory.Unauthorized("Invalid token"));
            }

            // Verify password using IdentityService
            if (!await _identityService.CheckPasswordAsync(userId.Value, request.Password))
            {
                return BadRequest(ApiResponseFactory.Error("Invalid password"));
            }

            // Check if 2FA is enabled
            if (!await _identityService.IsTwoFactorEnabledAsync(userId.Value))
            {
                return BadRequest(ApiResponseFactory.Error("Two-factor authentication must be enabled to generate recovery codes"));
            }

            var recoveryCodes = await _identityService.GenerateNewRecoveryCodesAsync(userId.Value);

            _logger.LogInformation("New recovery codes generated for user: {UserId}", userId);

            var response = new RecoveryCodesResponse
            {
                Succeeded = true,
                RecoveryCodes = recoveryCodes
            };

            return OkResponse(response, "New recovery codes have been generated. Old codes are now invalid.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recovery codes for user: {UserId}", _currentUserService.UserId);
            return InternalError("An error occurred while generating recovery codes");
        }
    }
}
