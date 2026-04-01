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
using Application.Ophthalmologists.Contracts.GetMyContract;
using Application.Ophthalmologists.Contracts.UploadSignedContract;
using Application.Ophthalmologists.Queries.GetDashboardMetrics;
using Application.Ophthalmologists.Queries.GetOphthalmologist;
using Application.Ophthalmologists.Queries.GetOphthalmologists;
using Application.SystemAdmin.Contracts.Common;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Ophthalmologist management endpoints.
/// Provides CRUD operations for ophthalmologist profiles.
/// </summary>
public class OphthalmologistsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public OphthalmologistsController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
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
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OphthalmologistListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOphthalmologists(
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isVerified = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOphthalmologistsQuery
        {
            SearchTerm = searchTerm,
            IsVerified = isVerified,
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
    [HttpGet("~/api/ophthalmologist/profile")]
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
    /// Get current authenticated ophthalmologist profile (alias endpoint for settings page).
    /// </summary>
    [HttpGet("~/api/ophthalmologists/me")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<OphthalmologistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentOphthalmologist(CancellationToken cancellationToken)
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

        if (_currentUserService.ProfileId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found in token"));

        var command = new UpdateOphthalmologistCommand
        {
            Id = _currentUserService.ProfileId.Value,
            UserId = _currentUserService.UserId.Value,
            FullName = request.FullName,
            Phone = request.Phone,
            Address = request.Address,
            Bio = request.Bio,
            YearsOfExperience = request.YearsOfExperience,
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Profile updated successfully");
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

        if (_currentUserService.ProfileId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("Ophthalmologist profile not found in token"));

        var form = await Request.ReadFormAsync(cancellationToken);
        var certificates = new List<UploadCredentialItemDto>();

        // Parse certificates from form data
        // Expected format: certificates[0][type], certificates[0][name], certificates[0][file], etc.
        var certificateCount = form.Keys
            .Where(k => k.StartsWith("certificates["))
            .Select(k => int.Parse(k.Split('[', ']')[1]))
            .Distinct()
            .Count();

        for (int i = 0; i < certificateCount; i++)
        {
            var typeStr = form[$"certificates[{i}][type]"].FirstOrDefault();
            var name = form[$"certificates[{i}][name]"].FirstOrDefault();
            var issuingAuthority = form[$"certificates[{i}][issuingAuthority]"].FirstOrDefault();
            var issuedDateStr = form[$"certificates[{i}][issuedDate]"].FirstOrDefault();
            var expiryDateStr = form[$"certificates[{i}][expiryDate]"].FirstOrDefault();
            var file = form.Files.FirstOrDefault(f => f.Name == $"certificates[{i}][file]");

            if (file?.Length > 0 &&
                !string.IsNullOrEmpty(typeStr) &&
                !string.IsNullOrEmpty(name) &&
                !string.IsNullOrEmpty(issuingAuthority) &&
                DateTime.TryParse(issuedDateStr, out var issuedDate))
            {
                var certificate = new UploadCredentialItemDto
                {
                    Type = Enum.Parse<CertificateType>(typeStr ?? "License"),
                    Name = name,
                    IssuingAuthority = issuingAuthority,
                    IssuedDate = issuedDate,
                    ExpiryDate = DateTime.TryParse(expiryDateStr, out var expiryDate) ? expiryDate : null,
                    File = file
                };

                // Set DegreeLevel for degrees
                if (certificate.Type == CertificateType.Degree)
                {
                    var degreeLevelStr = form[$"certificates[{i}][degreeLevel]"].FirstOrDefault();
                    if (Enum.TryParse<DegreeLevel>(degreeLevelStr ?? "Bachelor", out var degreeLevel))
                        certificate.DegreeLevel = degreeLevel;
                }

                certificates.Add(certificate);
            }
        }

        if (certificates.Count == 0)
            return BadRequest(ApiResponseFactory.Error("No valid certificates provided"));

        var command = new UploadCredentialsCommand
        {
            OphthalmologistId = _currentUserService.ProfileId.Value,
            Certificates = certificates
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Certificates uploaded successfully. Awaiting verification.");
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
}

public record UpdateOphthalmologistProfileRequest
{
    public string FullName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? Bio { get; init; }
    public int YearsOfExperience { get; init; }
}
