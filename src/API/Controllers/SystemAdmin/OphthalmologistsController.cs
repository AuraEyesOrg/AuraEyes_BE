using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.Ophthalmologists.Commands.VerifyOphthalmologist;
using Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    public OphthalmologistsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get ophthalmologists with pagination and filtering
    /// </summary>
    /// <param name="searchTerm">Search by name, email, or phone</param>
    /// <param name="verificationStatus">Filter: PendingVerification, Approved, Rejected</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OphthalmologistListDto>>), StatusCodes.Status200OK)]
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
    /// Approve or reject ophthalmologist credential verification.
    /// </summary>
    /// <param name="id">Ophthalmologist ID</param>
    /// <param name="request">Verification decision</param>
    [HttpPut("{id:guid}/verify")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyOphthalmologist(
        Guid id,
        [FromBody] VerifyOphthalmologistRequest request)
    {
        var command = new VerifyOphthalmologistCommand
        {
            OphthalmologistId = id,
            Approve = request.Approve,
            RejectionReason = request.RejectionReason
        };
        var result = await _mediator.Send(command);
        return HandleResult(result);
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
