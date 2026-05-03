using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Commands.CreateOphthalmologist;
using Application.Ophthalmologists.Commands.DeleteOphthalmologist;
using Application.Ophthalmologists.Commands.UnverifyOphthalmologist;
using Application.Ophthalmologists.Commands.UpdateOphthalmologist;
using Application.Ophthalmologists.Commands.VerifyOphthalmologist;
using Application.Ophthalmologists.Commands.UploadCredentials;
using Application.Ophthalmologists.Common;
using Application.Patients.Commands.UploadAvatar;
using Application.Ophthalmologists.Queries.GetDashboardMetrics;
using Application.Ophthalmologists.Queries.GetReviewQueue;
using Application.Ophthalmologists.Queries.GetOphthalmologist;
using Application.Ophthalmologists.Queries.GetOphthalmologists;
using Application.Ophthalmologists.LeaveRequests.Commands.CancelLeaveRequest;
using Application.Ophthalmologists.LeaveRequests.Commands.CreateLeaveRequest;
using Application.Ophthalmologists.LeaveRequests.Queries.GetMyLeaveRequests;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.Authorization;

namespace API.Controllers;

/// <summary>
/// Ophthalmologist management endpoints.
/// Provides CRUD operations for ophthalmologist profiles.
/// </summary>
public class OphthalmologistsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public OphthalmologistsController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        IOphthalmologistRepository ophthalmologistRepository)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    private async Task<Guid?> ResolveCurrentOphthalmologistProfileIdAsync(CancellationToken cancellationToken)
    {
        if (_currentUserService.ProfileId.HasValue)
            return _currentUserService.ProfileId.Value;

        if (!_currentUserService.UserId.HasValue)
            return null;

        var ophthalmologist = await _ophthalmologistRepository.GetByUserIdAsync(
            _currentUserService.UserId.Value,
            cancellationToken);

        return ophthalmologist?.Id;
    }

    /// <summary>
    /// Get ophthalmologists with pagination and filtering.
    /// </summary>
    /// <param name="searchTerm">Search term for filtering.</param>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 10).</param>
    /// <returns>Paginated list of ophthalmologists.</returns>
    [HttpGet]
    [AuthorizePermission(Permissions.OphthalmologistsRead)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OphthalmologistListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOphthalmologists(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOphthalmologistsQuery
        {
            SearchTerm = searchTerm,
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
    [AuthorizePermission(Permissions.OphthalmologistsRead)]
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
    [HttpGet("~/api/ophthalmologist/profile")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<OphthalmologistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var profileId = await ResolveCurrentOphthalmologistProfileIdAsync(cancellationToken);
        if (!profileId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found for current user"));

        var result = await _mediator.Send(
            new GetOphthalmologistQuery(profileId.Value),
            cancellationToken);

        return HandleResult(result, "Profile retrieved successfully");
    }

    /// <summary>
    /// Get current authenticated ophthalmologist profile (alias endpoint for settings page).
    /// </summary>
    [HttpGet("~/api/ophthalmologists/me")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<OphthalmologistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentOphthalmologist(CancellationToken cancellationToken)
    {
        var profileId = await ResolveCurrentOphthalmologistProfileIdAsync(cancellationToken);
        if (!profileId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found for current user"));

        var result = await _mediator.Send(
            new GetOphthalmologistQuery(profileId.Value),
            cancellationToken);

        return HandleResult(result, "Profile retrieved successfully");
    }

    /// <summary>
    /// Update current authenticated ophthalmologist profile information.
    /// </summary>
    [HttpPut("~/api/ophthalmologist/profile")]
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

        var profileId = await ResolveCurrentOphthalmologistProfileIdAsync(cancellationToken);
        if (!profileId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found for current user"));

        var existingProfileResult = await _mediator.Send(
            new GetOphthalmologistQuery(profileId.Value),
            cancellationToken);

        if (!existingProfileResult.IsSuccess || existingProfileResult.Data is null)
            return HandleResult(existingProfileResult, "Unable to resolve current profile.");

        var command = new UpdateOphthalmologistCommand
        {
            Id = profileId.Value,
            UserId = _currentUserService.UserId.Value,
            FullName = request.FullName,
            Phone = request.Phone,
            Address = request.Address,
            Bio = request.Bio,
            CitizenId = request.CitizenId,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
        };

        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
            return HandleResult(result);

        // Fetch the updated profile to return to the frontend
        var updatedProfileResult = await _mediator.Send(
            new GetOphthalmologistQuery(profileId.Value),
            cancellationToken);

        return HandleResult(updatedProfileResult, "Profile updated successfully");
    }

    /// <summary>
    /// Upload a new profile avatar for the authenticated ophthalmologist.
    /// Accepts multipart form data with an image file.
    /// </summary>
    [HttpPost("~/api/ophthalmologist/profile/avatar")]
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
    /// Upload certificates/credentials (degrees and licenses) for the authenticated ophthalmologist.
    /// Supports incremental uploads after initial registration.
    /// </summary>
    [HttpPost("~/api/ophthalmologist/profile/certificates")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadCertificates(
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var profileId = await ResolveCurrentOphthalmologistProfileIdAsync(cancellationToken);
        if (!profileId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found for current user"));

        var form = await Request.ReadFormAsync(cancellationToken);
        var certificates = ParseCertificatesFromForm(form);

        if (certificates.Count == 0)
            return BadRequest(ApiResponseFactory.Error("No valid certificates provided"));

        var result = await _mediator.Send(new UploadCredentialsCommand
        {
            OphthalmologistId = profileId.Value,
            Certificates = certificates
        }, cancellationToken);

        return HandleResult(result, "Certificates uploaded successfully. Awaiting verification.");
    }

    /// <summary>
    /// Delete a specific certificate for the authenticated ophthalmologist.
    /// </summary>
    [HttpDelete("~/api/ophthalmologist/profile/certificates/{certificateId:guid}")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCertificate(
        Guid certificateId,
        CancellationToken cancellationToken)
    {
        var profileId = await ResolveCurrentOphthalmologistProfileIdAsync(cancellationToken);
        if (!profileId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found for current user"));

        var result = await _mediator.Send(new Application.Ophthalmologists.Commands.DeleteCertificate.DeleteCertificateCommand(
            profileId.Value,
            certificateId), cancellationToken);

        return HandleResult(result, "Certificate deleted successfully.");
    }

    /// <summary>
    /// Update a specific certificate for the authenticated ophthalmologist.
    /// </summary>
    [HttpPut("~/api/ophthalmologist/profile/certificates/{certificateId:guid}")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCertificate(
        Guid certificateId,
        [FromForm] string name,
        [FromForm] string? degreeLevel,
        [FromForm] string? issuingAuthority,
        [FromForm] DateTime issuedDate,
        [FromForm] DateTime? expiryDate,
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        var profileId = await ResolveCurrentOphthalmologistProfileIdAsync(cancellationToken);
        if (!profileId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found for current user"));

        var result = await _mediator.Send(new Application.Ophthalmologists.Commands.UpdateCertificate.UpdateCertificateCommand
        {
            OphthalmologistId = profileId.Value,
            CertificateId = certificateId,
            Name = name,
            DegreeLevel = degreeLevel,
            IssuingAuthority = issuingAuthority,
            IssuedDate = issuedDate,
            ExpiryDate = expiryDate,
            File = file
        }, cancellationToken);

        return HandleResult(result, "Certificate updated successfully.");
    }

    private List<UploadCredentialItemDto> ParseCertificatesFromForm(IFormCollection form)
    {
        var certificates = new List<UploadCredentialItemDto>();
        var certificateCount = GetCertificateCountFromForm(form);

        for (int i = 0; i < certificateCount; i++)
        {
            var item = ParseCertificateItem(form, i);
            if (item != null)
            {
                certificates.Add(item);
            }
        }

        return certificates;
    }

    private int GetCertificateCountFromForm(IFormCollection form)
    {
        return form.Keys
            .Where(k => k.StartsWith("certificates["))
            .Select(k =>
            {
                var parts = k.Split('[', ']');
                return parts.Length > 1 && int.TryParse(parts[1], out var index) ? index : -1;
            })
            .Where(index => index >= 0)
            .DefaultIfEmpty(-1)
            .Max() + 1;
    }

    private UploadCredentialItemDto? ParseCertificateItem(IFormCollection form, int index)
    {
        var typeStr = form[$"certificates[{index}][type]"].FirstOrDefault();
        var name = form[$"certificates[{index}][name]"].FirstOrDefault();
        var issuingAuthority = form[$"certificates[{index}][issuingAuthority]"].FirstOrDefault();
        var issuedDateStr = form[$"certificates[{index}][issuedDate]"].FirstOrDefault();
        var expiryDateStr = form[$"certificates[{index}][expiryDate]"].FirstOrDefault();
        var file = form.Files.FirstOrDefault(f => f.Name == $"certificates[{index}][file]");

        if (file?.Length > 0 &&
            !string.IsNullOrEmpty(typeStr) &&
            !string.IsNullOrEmpty(name) &&
            !string.IsNullOrEmpty(issuingAuthority) &&
            DateTime.TryParse(issuedDateStr, out var issuedDate))
        {
            var item = new UploadCredentialItemDto
            {
                Type = Enum.Parse<CertificateType>(typeStr ?? "License"),
                Name = name,
                IssuingAuthority = issuingAuthority,
                IssuedDate = issuedDate,
                ExpiryDate = DateTime.TryParse(expiryDateStr, out var expiryDate) ? expiryDate : null,
                File = file
            };

            if (item.Type == CertificateType.Degree)
            {
                var degreeLevelStr = form[$"certificates[{index}][degreeLevel]"].FirstOrDefault();
                if (Enum.TryParse<DegreeLevel>(degreeLevelStr ?? "Bachelor", out var degreeLevel))
                    item.DegreeLevel = degreeLevel;
            }

            return item;
        }

        return null;
    }

    /// <summary>
    /// Get leave requests for the current authenticated ophthalmologist.
    /// </summary>
    [HttpGet("~/api/ophthalmologist/leave-requests")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<Application.Ophthalmologists.Common.OphthalmologistLeaveRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyLeaveRequests(
        [FromQuery] OphthalmologistLeaveRequestStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var profileId = await ResolveCurrentOphthalmologistProfileIdAsync(cancellationToken);
        if (!profileId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found for current user"));

        var result = await _mediator.Send(
            new GetMyLeaveRequestsQuery
            {
                OphthalmologistId = profileId.Value,
                Status = status,
                PageNumber = pageNumber,
                PageSize = pageSize
            },
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Submit a leave request for the current authenticated ophthalmologist.
    /// </summary>
    [HttpPost("~/api/ophthalmologist/leave-requests")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateLeaveRequest(
        [FromBody] CreateLeaveRequestApiRequest request,
        CancellationToken cancellationToken)
    {
        var profileId = await ResolveCurrentOphthalmologistProfileIdAsync(cancellationToken);
        if (!profileId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found for current user"));

        var result = await _mediator.Send(
            new CreateLeaveRequestCommand
            {
                OphthalmologistId = profileId.Value,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason
            },
            cancellationToken);

        return HandleResult(result, "Leave request submitted successfully.");
    }

    /// <summary>
    /// Cancel a pending leave request owned by the current authenticated ophthalmologist.
    /// </summary>
    [HttpPost("~/api/ophthalmologist/leave-requests/{leaveRequestId:guid}/cancel")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelLeaveRequest(
        Guid leaveRequestId,
        CancellationToken cancellationToken)
    {
        var profileId = await ResolveCurrentOphthalmologistProfileIdAsync(cancellationToken);
        if (!profileId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found for current user"));

        var result = await _mediator.Send(
            new CancelLeaveRequestCommand
            {
                LeaveRequestId = leaveRequestId,
                OphthalmologistId = profileId.Value
            },
            cancellationToken);

        return HandleResult(result, "Leave request cancelled successfully.");
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
    /// Verify an ophthalmologist profile.
    /// </summary>
    /// <param name="id">Ophthalmologist ID.</param>
    /// <returns>Success response.</returns>
    [HttpPost("{id:guid}/verify")]
    [AuthorizePermission(Permissions.OphthalmologistsVerify)]
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
    [AuthorizePermission(Permissions.OphthalmologistsVerify)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnverifyOphthalmologist(Guid id)
    {
        var result = await _mediator.Send(new UnverifyOphthalmologistCommand(id));
        return HandleResult(result, "Ophthalmologist verification revoked successfully.");
    }

    /// <summary>
    /// Get ophthalmologists available for a specific time slot on a given date.
    /// </summary>
    [HttpGet("available-for-slot")]
    [AuthorizePermission(Permissions.AppointmentsManage)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<Application.Scheduling.Appointments.Queries.GetAvailableDoctorsForSlot.AvailableDoctorDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableDoctorsForSlot(
        [FromQuery] DateOnly date,
        [FromQuery] TimeOnly startTime,
        [FromQuery] TimeOnly endTime)
    {
        var result = await _mediator.Send(
            new Application.Scheduling.Appointments.Queries.GetAvailableDoctorsForSlot.GetAvailableDoctorsForSlotQuery(
                date, startTime, endTime));
        return HandleResult(result);
    }

    // =========================================================================
    // LEGACY CONTRACT ENDPOINTS (deprecated)
    // =========================================================================

    /// <summary>
    /// Legacy endpoint - external contract flow for ophthalmologists has been removed.
    /// </summary>
    [HttpGet("my-contract")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status410Gone)]
    public async Task<IActionResult> GetMyContract()
    {
        await Task.CompletedTask;
        return StatusCode(
            StatusCodes.Status410Gone,
            ApiResponseFactory.Error("External contract flow has been removed for ophthalmologists."));
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

    [HttpGet("review-queue")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<List<ReviewQueueItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviewQueue()
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Error("User not authenticated."));

        var result = await _mediator.Send(new GetReviewQueueQuery(_currentUserService.UserId.Value));
        return HandleResult(result);
    }

    /// <summary>
    /// Legacy endpoint - external contract flow for ophthalmologists has been removed.
    /// </summary>
    [HttpPost("my-contract/upload")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status410Gone)]
    public async Task<IActionResult> UploadSignedContract()
    {
        await Task.CompletedTask;
        return StatusCode(
            StatusCodes.Status410Gone,
            ApiResponseFactory.Error("External contract flow has been removed for ophthalmologists."));
    }
}

public record UpdateOphthalmologistProfileRequest
{
    public string FullName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? Bio { get; init; }
    public string? CitizenId { get; init; }
    public int? Gender { get; init; }
    public DateTime? DateOfBirth { get; init; }
}

public record CreateLeaveRequestApiRequest
{
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public string Reason { get; init; } = string.Empty;
}
