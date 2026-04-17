using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Ophthalmologists.Commands.PaySalary;
using Application.SystemAdmin.Ophthalmologists.Commands.ConfirmWithdrawalRequest;
using Application.SystemAdmin.Ophthalmologists.Commands.RejectWithdrawalRequest;
using Application.SystemAdmin.Ophthalmologists.Commands.BackfillFullTimeSchedule;
using Application.SystemAdmin.Ophthalmologists.Commands.DeleteFutureOphthalmologistSlots;
using Application.SystemAdmin.Ophthalmologists.Commands.NormalizeAllFullTimeSchedules;
using Application.SystemAdmin.Ophthalmologists.Commands.NormalizeFullTimeSchedule;
using Application.SystemAdmin.Ophthalmologists.Commands.ApproveLeaveRequest;
using Application.SystemAdmin.Ophthalmologists.Commands.ApproveEmploymentTypeChangeRequest;
using Application.SystemAdmin.Ophthalmologists.Commands.RejectEmploymentTypeChangeRequest;
using Application.SystemAdmin.Ophthalmologists.Commands.RejectLeaveRequest;
using Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;
using Application.SystemAdmin.Ophthalmologists.Queries.GetEmploymentTypeChangeRequests;
using Application.SystemAdmin.Ophthalmologists.Queries.GetLeaveRequests;
using Application.SystemAdmin.Ophthalmologists.Queries.GetWithdrawalRequests;
using Application.Wallets.Common;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Ophthalmologists.Contracts.UploadSignedContract;
using Application.Ophthalmologists.Common;
using Application.Ophthalmologists.Queries.GetOphthalmologist;
using Application.Ophthalmologists.Commands.CreateOphthalmologist;
using Application.Ophthalmologists.Commands.UpdateOphthalmologist;
using Application.Patients.Commands.UploadAvatar;
using Application.SystemAdmin.Contracts.Common;
using Application.Ophthalmologists.Contracts.GetMyContract;
using Application.Ophthalmologists.Commands.UnverifyOphthalmologist;
using Application.Ophthalmologists.Commands.DeleteOphthalmologist;
using Application.Ophthalmologists.Queries.GetDashboardMetrics;
using Application.Ophthalmologists.Commands.VerifyOphthalmologist;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// System Admin Ophthalmologist Management endpoints.
/// Credential verification, search, and listing.
/// </summary>
[Route("api/system-admin/[controller]")]
[Authorize(Policy = Policies.SystemAdminOnly)]
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
    [Authorize(Policy = Policies.AdminsOnly)]
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
    [Authorize(Policy = Policies.AdminsOnly)]
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
    [Authorize(Policy = Policies.OphthalmologistOnly)]
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
    [Authorize(Policy = Policies.OphthalmologistOnly)]
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
    [Authorize(Policy = Policies.OphthalmologistOnly)]
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
    [Authorize(Policy = Policies.SystemAdminOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOphthalmologist(Guid id)
    {
        var result = await _mediator.Send(new DeleteOphthalmologistCommand(id));
        return HandleResult(result, "Ophthalmologist profile deleted successfully.");
    }

    /// <summary>
    /// Verify an ophthalmologist profile.
    /// </summary>
    /// <param name="id">Ophthalmologist ID.</param>
    /// <returns>Success response.</returns>
    [HttpPost("{id:guid}/verify")]
    [Authorize(Policy = Policies.AdminsOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyOphthalmologist(Guid id)
    {
        var result = await _mediator.Send(new VerifyOphthalmologistCommand(id));
        return HandleResult(result, "Ophthalmologist verified successfully.");
    }

    /// <summary>
    /// Unverify (revoke verification of) an ophthalmologist profile.
    /// </summary>
    /// <param name="id">Ophthalmologist ID.</param>
    /// <returns>Success response.</returns>
    [HttpPost("{id:guid}/unverify")]
    [Authorize(Policy = Policies.AdminsOnly)]
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

    /// <summary>
    /// Get the current ophthalmologist's contract.
    /// </summary>
    [HttpGet("my-contract")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ContractDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyContract()
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(new GetMyContractQuery(userId.Value));
        return HandleResult(result);
    }

    [HttpGet("dashboard-metrics")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<OphthalmologistDashboardMetricsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(new GetDashboardMetricsQuery(_currentUserService.UserId.Value));
        return HandleResult(result);
    }

    /// <summary>
    /// Upload a signed contract document (scanned image).
    /// </summary>
    [HttpPost("my-contract/upload")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadSignedContract(IFormFile contractImage)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        if (contractImage == null || contractImage.Length == 0)
            return BadRequest(ApiResponseFactory.Error("Contract image file is required."));

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "application/pdf" };
        if (!allowedTypes.Contains(contractImage.ContentType.ToLowerInvariant()))
            return BadRequest(ApiResponseFactory.Error("Only JPEG, PNG, WebP and PDF files are allowed."));

        // Validate file size (max 10MB)
        if (contractImage.Length > 10 * 1024 * 1024)
            return BadRequest(ApiResponseFactory.Error("File size must not exceed 10MB."));

        // Upload to storage
        string scannedUrl;
        await using (var stream = contractImage.OpenReadStream())
        {
            scannedUrl = await _fileStorageService.SaveFileAsync(
                stream,
                contractImage.FileName,
                $"contracts/{userId.Value}");
        }

        var command = new UploadSignedContractCommand
        {
            UserId = userId.Value,
            ScannedDocumentUrl = scannedUrl
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Contract uploaded successfully. Waiting for admin verification.");
    }

    /// <summary>
    /// Pay monthly salary (or custom amount) into ophthalmologist wallet.
    /// </summary>
    [HttpPost("{id:guid}/salary-payout")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PaySalary(
        Guid id,
        [FromBody] PaySalaryRequest request)
    {
        var command = new PayOphthalmologistSalaryCommand
        {
            OphthalmologistId = id,
            Amount = request.Amount,
            Note = request.Note
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get withdrawal requests from ophthalmologists.
    /// </summary>
    [HttpGet("withdrawal-requests")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminWithdrawalRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWithdrawalRequests(
        [FromQuery] PaymentStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetWithdrawalRequestsQuery
        {
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Confirm that transfer for a withdrawal request has been completed.
    /// </summary>
    [HttpPost("withdrawal-requests/{requestId:guid}/confirm")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmWithdrawalRequest(
        Guid requestId,
        [FromBody] ConfirmWithdrawalRequestApi request)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));
        }

        var command = new ConfirmWithdrawalRequestCommand
        {
            WithdrawalRequestId = requestId,
            AdminUserId = currentUserId.Value,
            TransferReference = request.TransferReference,
            Note = request.Note
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Reject a withdrawal request.
    /// </summary>
    [HttpPost("withdrawal-requests/{requestId:guid}/reject")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectWithdrawalRequest(
        Guid requestId,
        [FromBody] RejectWithdrawalRequestApi request)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));
        }

        var command = new RejectWithdrawalRequestCommand
        {
            WithdrawalRequestId = requestId,
            AdminUserId = currentUserId.Value,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get ophthalmologist leave requests for review.
    /// </summary>
    [HttpGet("leave-requests")]
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

    /// <summary>
    /// Get ophthalmologist employment type change requests for review.
    /// </summary>
    [HttpGet("employment-type-change-requests")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AdminOphthalmologistEmploymentTypeChangeRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmploymentTypeChangeRequests(
        [FromQuery] OphthalmologistEmploymentTypeChangeRequestStatus? status = null,
        [FromQuery] Guid? ophthalmologistId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetEmploymentTypeChangeRequestsQuery
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
    /// Approve an ophthalmologist employment type change request.
    /// </summary>
    [HttpPost("employment-type-change-requests/{requestId:guid}/approve")]
    [ProducesResponseType(typeof(ApiResponse<ApproveEmploymentTypeChangeRequestResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveEmploymentTypeChangeRequest(
        Guid requestId,
        [FromBody] ReviewEmploymentTypeChangeRequestApi request)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));

        var result = await _mediator.Send(new ApproveEmploymentTypeChangeRequestCommand
        {
            RequestId = requestId,
            ReviewedByAdminUserId = _currentUserService.UserId.Value,
            AdminNote = request.AdminNote
        });

        return HandleResult(result, "Employment type change request approved successfully.");
    }

    /// <summary>
    /// Reject an ophthalmologist employment type change request.
    /// </summary>
    [HttpPost("employment-type-change-requests/{requestId:guid}/reject")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RejectEmploymentTypeChangeRequest(
        Guid requestId,
        [FromBody] ReviewEmploymentTypeChangeRequestApi request)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated."));

        var result = await _mediator.Send(new RejectEmploymentTypeChangeRequestCommand
        {
            RequestId = requestId,
            ReviewedByAdminUserId = _currentUserService.UserId.Value,
            AdminNote = request.AdminNote
        });

        return HandleResult(result, "Employment type change request rejected successfully.");
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

public class PaySalaryRequest
{
    public decimal? Amount { get; set; }
    public string? Note { get; set; }
}

public class ConfirmWithdrawalRequestApi
{
    public string? TransferReference { get; set; }
    public string? Note { get; set; }
}

public class RejectWithdrawalRequestApi
{
    public string? Reason { get; set; }
}

public class ReviewLeaveRequestApi
{
    public string? AdminNote { get; set; }
}

public class ReviewEmploymentTypeChangeRequestApi
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
