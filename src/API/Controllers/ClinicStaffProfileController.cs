using Application.ClinicStaffs.Commands.UpdateClinicStaffProfile;
using Application.ClinicStaffs.Commands.UploadClinicStaffAvatar;
using Application.ClinicStaffs.Common;
using Application.ClinicStaffs.Queries.GetClinicStaffProfile;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Infrastructure.Identity.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Self-service profile endpoints for clinic staff portal settings.
/// </summary>
[Route("api/clinic-staff/profile")]
[AuthorizePermission(Permissions.SettingsRead)]
public class ClinicStaffProfileController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ClinicStaffProfileController(
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ClinicStaffProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var result = await _mediator.Send(
            new GetClinicStaffProfileQuery(_currentUserService.UserId.Value),
            cancellationToken);

        return HandleResult(result, "Clinic staff profile retrieved successfully");
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<ClinicStaffProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateClinicStaffProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var command = new UpdateClinicStaffProfileCommand
        {
            UserId = _currentUserService.UserId.Value,
            FullName = request.FullName,
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Address = request.Address,
            CitizenId = request.CitizenId,
            Department = request.Department,
            EmployeeCode = request.EmployeeCode,
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Clinic staff profile updated successfully");
    }

    [HttpPost("avatar")]
    [ProducesResponseType(typeof(ApiResponse<UploadClinicStaffAvatarResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadAvatar(
        IFormFile avatar,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        if (avatar.Length == 0)
            return BadRequest(ApiResponseFactory.Error("No file uploaded"));

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(avatar.ContentType.ToLowerInvariant()))
            return BadRequest(ApiResponseFactory.Error("Invalid file type. Supported: JPG, PNG, GIF, WebP"));

        if (avatar.Length > 5 * 1024 * 1024)
            return BadRequest(ApiResponseFactory.Error("File must be smaller than 5MB"));

        using var stream = avatar.OpenReadStream();
        var command = new UploadClinicStaffAvatarCommand
        {
            UserId = _currentUserService.UserId.Value,
            FileStream = stream,
            FileName = avatar.FileName,
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Avatar uploaded successfully");
    }
}

public record UpdateClinicStaffProfileRequest
{
    public string FullName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? Address { get; init; }
    public string? CitizenId { get; init; }
    public string? Department { get; init; }
    public string? EmployeeCode { get; init; }
}

