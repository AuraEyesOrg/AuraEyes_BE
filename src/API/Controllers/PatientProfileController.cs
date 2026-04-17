using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Patients.Commands.ChangePassword;
using Application.Patients.Commands.UpdatePatientProfile;
using Application.Patients.Commands.UploadAvatar;
using Application.Patients.Common;
using Application.Patients.Queries.GetPatientProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Patient profile management endpoints.
/// Provides profile CRUD, avatar upload, and password change.
/// </summary>
[Route("api/patient/profile")]
[Authorize]
public class PatientProfileController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PatientProfileController> _logger;

    public PatientProfileController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        ILogger<PatientProfileController> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Get current patient's profile.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PatientProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var result = await _mediator.Send(
            new GetPatientProfileQuery(_currentUserService.UserId.Value),
            cancellationToken);

        return HandleResult(result, "Profile retrieved successfully");
    }

    /// <summary>
    /// Update current patient's profile information.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<PatientProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var command = new UpdatePatientProfileCommand
        {
            UserId = _currentUserService.UserId.Value,
            FullName = request.FullName,
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Address = request.Address,
            CitizenId = request.CitizenId,
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Profile updated successfully");
    }

    /// <summary>
    /// Upload a new profile avatar.
    /// Accepts multipart form data with an image file.
    /// </summary>
    [HttpPost("avatar")]
    [ProducesResponseType(typeof(ApiResponse<UploadAvatarResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadAvatar(
        IFormFile avatar,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        if (avatar.Length == 0)
            return BadRequest(ApiResponseFactory.Error("No file uploaded"));

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(avatar.ContentType.ToLowerInvariant()))
            return BadRequest(ApiResponseFactory.Error("Invalid file type. Supported: JPG, PNG, GIF, WebP"));

        // Validate file size (5MB max)
        if (avatar.Length > 5 * 1024 * 1024)
            return BadRequest(ApiResponseFactory.Error("File must be smaller than 5MB"));

        using var stream = avatar.OpenReadStream();
        var command = new UploadAvatarCommand
        {
            UserId = _currentUserService.UserId.Value,
            FileStream = stream,
            FileName = avatar.FileName,
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Avatar uploaded successfully");
    }

    /// <summary>
    /// Change account password.
    /// </summary>
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var command = new ChangePasswordCommand
        {
            UserId = _currentUserService.UserId.Value,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword,
            ConfirmNewPassword = request.ConfirmNewPassword,
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Password changed successfully");
    }
}

// ============ REQUEST DTOs ============

/// <summary>
/// Request DTO for profile update.
/// </summary>
public record UpdateProfileRequest
{
    public string FullName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? Address { get; init; }
    public string? CitizenId { get; init; }
}

/// <summary>
/// Request DTO for password change.
/// </summary>
public record ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmNewPassword { get; init; } = string.Empty;
}
