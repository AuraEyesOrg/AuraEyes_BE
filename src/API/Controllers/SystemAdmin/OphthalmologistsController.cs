using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Ophthalmologists.Commands.PaySalary;
using Application.SystemAdmin.Ophthalmologists.Commands.ConfirmWithdrawalRequest;
using Application.SystemAdmin.Ophthalmologists.Commands.RejectWithdrawalRequest;
using Application.SystemAdmin.Ophthalmologists.Commands.VerifyOphthalmologist;
using Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;
using Application.SystemAdmin.Ophthalmologists.Queries.GetWithdrawalRequests;
using Application.Wallets.Common;
using Domain.Enums;
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
    private readonly ICurrentUserService _currentUserService;

    public OphthalmologistsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
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
