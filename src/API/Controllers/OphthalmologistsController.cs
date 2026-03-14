using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Commands.CreateOphthalmologist;
using Application.Ophthalmologists.Commands.DeleteOphthalmologist;
using Application.Ophthalmologists.Commands.UnverifyOphthalmologist;
using Application.Ophthalmologists.Commands.UpdateOphthalmologist;
using Application.Ophthalmologists.Commands.VerifyOphthalmologist;
using Application.Ophthalmologists.Common;
using Application.Ophthalmologists.Contracts.GetMyContract;
using Application.Ophthalmologists.Contracts.UploadSignedContract;
using Application.Ophthalmologists.Queries.GetOphthalmologist;
using Application.Ophthalmologists.Queries.GetOphthalmologists;
using Application.SystemAdmin.Contracts.Common;
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
