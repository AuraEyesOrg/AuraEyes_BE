using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Ophthalmologists.Commands.BackfillFullTimeSchedule;
using Application.SystemAdmin.Ophthalmologists.Commands.DeleteFutureOphthalmologistSlots;
using Application.SystemAdmin.Ophthalmologists.Commands.NormalizeAllFullTimeSchedules;
using Application.SystemAdmin.Ophthalmologists.Commands.NormalizeFullTimeSchedule;
using Application.SystemAdmin.Ophthalmologists.Commands.ApproveLeaveRequest;
using Application.SystemAdmin.Ophthalmologists.Commands.RejectLeaveRequest;
using Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;
using Application.SystemAdmin.Ophthalmologists.Queries.GetLeaveRequests;
using Domain.Enums;
using Infrastructure.Identity.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.Authorization;
using Application.Ophthalmologists.Common;
using Application.Ophthalmologists.Queries.GetOphthalmologist;
using Application.Ophthalmologists.Commands.CreateOphthalmologist;
using Application.Ophthalmologists.Commands.UpdateOphthalmologist;
using Application.Patients.Commands.UploadAvatar;
using Application.Ophthalmologists.Commands.UnverifyOphthalmologist;
using Application.Ophthalmologists.Commands.DeleteOphthalmologist;
using Application.Ophthalmologists.Queries.GetDashboardMetrics;
using SystemAdminVerify = Application.SystemAdmin.Ophthalmologists.Commands.VerifyOphthalmologist;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin Ophthalmologist Management endpoints.
/// Credential verification, search, and listing.
/// </summary>
[Route("api/system-admin/[controller]")]
[AuthorizePermission(Permissions.OphthalmologistsRead)]
public class OphthalmologistsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public OphthalmologistsController(IMediator mediator, ICurrentUserService currentUserService, IFileStorageService fileStorageService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Get ophthalmologists with pagination and filtering.
    /// </summary>
    /// <param name="searchTerm">Search term for filtering.</param>
    /// <param name="isVerified">Filter by verification status.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <returns>Paginated list of ophthalmologists.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<Application.Ophthalmologists.Common.OphthalmologistListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOphthalmologists(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? verificationStatus = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOphthalmologistsQuery
        {
            SearchTerm = searchTerm,
            VerificationStatus = verificationStatus,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get ophthalmologist by ID.
    /// </summary>
    /// <param name="id">Ophthalmologist ID.</param>
    /// <returns>Ophthalmologist details with certificates.</returns>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OphthalmologistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOphthalmologist(Guid id)
    {
        var result = await _mediator.Send(new GetOphthalmologistQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new ophthalmologist profile.
    /// </summary>
    /// <param name="command">Create ophthalmologist command.</param>
    /// <returns>Created ophthalmologist ID.</returns>
    [HttpPost]
    [AuthorizePermission(Permissions.OphthalmologistsCreate)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOphthalmologist([FromBody] CreateOphthalmologistCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetOphthalmologist),
                new { id = result.Data },
                ApiResponseFactory.Success(result.Data, "Ophthalmologist profile created successfully."));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing ophthalmologist profile.
    /// </summary>
    /// <param name="id">Ophthalmologist ID.</param>
    /// <param name="command">Update ophthalmologist command.</param>
    /// <returns>Success response.</returns>
    [HttpPut("{id:guid}")]
    [AuthorizePermission(Permissions.OphthalmologistsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOphthalmologist(Guid id, [FromBody] UpdateOphthalmologistCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponseFactory.Error("ID in URL does not match ID in request body."));
        }

        var result = await _mediator.Send(command);
        return HandleResult(result, "Ophthalmologist profile updated successfully.");
    }

    /// <summary>
    /// Get current authenticated ophthalmologist profile.
    /// </summary>
    [HttpGet("profile")]
    [AuthorizePermission(Permissions.OphthalmologistsRead)]
    [ProducesResponseType(typeof(ApiResponse<OphthalmologistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        if (_currentUserService.ProfileId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found in token"));

        var result = await _mediator.Send(
            new GetOphthalmologistQuery(_currentUserService.ProfileId.Value),
            cancellationToken);

        return HandleResult(result, "Profile retrieved successfully");
    }

    /// <summary>
    /// Update current authenticated ophthalmologist profile information.
    /// </summary>
    [HttpPut("profile")]
    [AuthorizePermission(Permissions.OphthalmologistsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateOphthalmologistProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        if (_currentUserService.ProfileId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found in token"));

        var existingProfileResult = await _mediator.Send(
            new GetOphthalmologistQuery(_currentUserService.ProfileId.Value),
            cancellationToken);

        if (!existingProfileResult.IsSuccess || existingProfileResult.Data is null)
            return HandleResult(existingProfileResult, "Unable to resolve current profile.");

        var command = new UpdateOphthalmologistCommand
        {
            Id = _currentUserService.ProfileId.Value,
            UserId = _currentUserService.UserId.Value,
            FullName = request.FullName,
            Phone = request.Phone,
            Address = request.Address,
            Bio = request.Bio,
            YearsOfExperience = existingProfileResult.Data.YearsOfExperience,
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Profile updated successfully");
    }

    /// <summary>
    /// Upload a new profile avatar for the authenticated ophthalmologist.
    /// Accepts multipart form data with an image file.
    /// </summary>
    [HttpPost("profile/avatar")]
    [AuthorizePermission(Permissions.OphthalmologistsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<UploadAvatarResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadMyAvatar(
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
    /// Delete (soft-delete) an ophthalmologist profile.
    /// </summary>
    /// <param name="id">Ophthalmologist ID.</param>
    /// <returns>Success response.</returns>
    [HttpDelete("{id:guid}")]
    [AuthorizePermission(Permissions.OphthalmologistsDelete)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOphthalmologist(Guid id)
    {
        var result = await _mediator.Send(new DeleteOphthalmologistCommand(id));
        return HandleResult(result, "Ophthalmologist profile deleted successfully.");
    }

    /// <summary>
    /// Verify an ophthalmologist profile (Approve or Reject).
    /// </summary>
    /// <param name="id">Ophthalmologist ID.</param>
    /// <param name="request">Approval or rejection details.</param>
    /// <returns>Success response.</returns>
    [HttpPost("{id:guid}/verify")]
    [AuthorizePermission(Permissions.OphthalmologistsVerify)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyOphthalmologist(Guid id, [FromBody] VerifyOphthalmologistRequest request)
    {
        var command = new SystemAdminVerify.VerifyOphthalmologistCommand
        {
            OphthalmologistId = id,
            Approve = request.Approve,
            RejectionReason = request.RejectionReason
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Unverify (revoke verification of) an ophthalmologist profile.
    /// </summary>
    /// <param name="id">Ophthalmologist ID.</param>
    /// <returns>Success response.</returns>
    [HttpPost("{id:guid}/unverify")]
    [AuthorizePermission(Permissions.OphthalmologistsVerify)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnverifyOphthalmologist(Guid id)
    {
        var result = await _mediator.Send(new UnverifyOphthalmologistCommand(id));
        return HandleResult(result, "Ophthalmologist verification revoked successfully.");
    }
    // =========================================================================
    // CONTRACT ENDPOINTS (for the authenticated ophthalmologist)
    // =========================================================================


    [HttpGet("dashboard-metrics")]
    [AuthorizePermission(Permissions.DashboardRead)]
    [ProducesResponseType(typeof(ApiResponse<OphthalmologistDashboardMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(new GetDashboardMetricsQuery(_currentUserService.UserId.Value));
        return HandleResult(result);
    }


    /// <summary>
    /// Get ophthalmologist leave requests for review.
    /// </summary>
    [HttpGet("leave-requests")]
    [AuthorizePermission(Permissions.SchedulesManage)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminOphthalmologistLeaveRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaveRequests(
        [FromQuery] OphthalmologistLeaveRequestStatus? status = null,
        [FromQuery] Guid? ophthalmologistId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetLeaveRequestsQuery
        {
            Status = status,
            OphthalmologistId = ophthalmologistId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Approve an ophthalmologist leave request.
    /// </summary>
    [HttpPost("leave-requests/{leaveRequestId:guid}/approve")]
    [AuthorizePermission(Permissions.SchedulesManage)]
    [ProducesResponseType(typeof(ApiResponse<ApproveLeaveRequestResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveLeaveRequest(
        Guid leaveRequestId,
        [FromBody] ReviewLeaveRequestApi request)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));

        var result = await _mediator.Send(new ApproveLeaveRequestCommand
        {
            LeaveRequestId = leaveRequestId,
            ReviewedByAdminUserId = _currentUserService.UserId.Value,
            AdminNote = request.AdminNote
        });

        return HandleResult(result, "Leave request approved successfully.");
    }

    /// <summary>
    /// Reject an ophthalmologist leave request.
    /// </summary>
    [HttpPost("leave-requests/{leaveRequestId:guid}/reject")]
    [AuthorizePermission(Permissions.SchedulesManage)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RejectLeaveRequest(
        Guid leaveRequestId,
        [FromBody] ReviewLeaveRequestApi request)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));

        var result = await _mediator.Send(new RejectLeaveRequestCommand
        {
            LeaveRequestId = leaveRequestId,
            ReviewedByAdminUserId = _currentUserService.UserId.Value,
            AdminNote = request.AdminNote
        });

        return HandleResult(result, "Leave request rejected successfully.");
    }
}

/// <summary>
/// Request body for ophthalmologist verification.
/// </summary>
public class VerifyOphthalmologistRequest
{
    public bool Approve { get; set; }
    public string? RejectionReason { get; set; }
}


public class ReviewLeaveRequestApi
{
    public string? AdminNote { get; set; }
}

public record UpdateOphthalmologistProfileRequest
{
    public string FullName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? Bio { get; init; }
    public int YearsOfExperience { get; init; }
}
