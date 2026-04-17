using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.Contracts.Commands.CancelContract;
using Application.SystemAdmin.Contracts.Commands.CreateContract;
using Application.SystemAdmin.Contracts.Commands.SendForSignature;
using Application.SystemAdmin.Contracts.Commands.SignContract;
using Application.SystemAdmin.Contracts.Commands.TerminateContract;
using Application.SystemAdmin.Contracts.Commands.UpdateContract;
using Application.SystemAdmin.Contracts.Common;
using Application.SystemAdmin.Contracts.Queries.GetContractById;
using Application.SystemAdmin.Contracts.Queries.GetContracts;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// Contract Management — full lifecycle (Draft → PendingSignature → Active → Expired/Terminated/Cancelled).
/// All endpoints require SystemAdmin role.
/// </summary>
[Route("api/system-admin/contracts")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class ContractsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ContractsController(IMediator mediator) => _mediator = mediator;

    // =========================================================================
    // QUERIES
    // =========================================================================

    /// <summary>Get a paged list of contracts with optional filters.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ContractDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContracts(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] ContractStatus? status = null,
        [FromQuery] ContractType? contractType = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetContractsQuery
        {
            SearchTerm = searchTerm,
            UserId = userId,
            Status = status,
            ContractType = contractType,
            PageNumber = pageNumber,
            PageSize = Math.Min(pageSize, 100)
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>Get a single contract with full detail (includes signed content).</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ContractDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContractById(Guid id)
    {
        var result = await _mediator.Send(new GetContractByIdQuery(id));
        return HandleResult(result);
    }

    // =========================================================================
    // COMMANDS
    // =========================================================================

    /// <summary>Create a new contract in Draft status.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "Contract created successfully.");
    }

    /// <summary>Update commercial terms of a Draft contract.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContract(Guid id, [FromBody] UpdateContractRequest request)
    {
        var command = new UpdateContractCommand
        {
            Id = id,
            TemplateId = request.TemplateId,
            AiQuotaLimit = request.AiQuotaLimit,
            MonthlyQuotaLimit = request.MonthlyQuotaLimit,
            PlatformCommissionRate = request.PlatformCommissionRate
        };
        var result = await _mediator.Send(command);
        return HandleResult(result, "Contract updated successfully.");
    }

    /// <summary>Send a Draft contract to the counterparty for signature.</summary>
    [HttpPost("{id:guid}/send-for-signature")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendForSignature(Guid id)
    {
        var result = await _mediator.Send(new SendForSignatureCommand(id));
        return HandleResult(result, "Contract sent for signature.");
    }

    /// <summary>Mark a PendingSignature contract as signed and Activate it.</summary>
    [HttpPost("{id:guid}/sign")]
    [ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SignContract(Guid id, [FromBody] SignContractRequest request)
    {
        var command = new SignContractCommand
        {
            Id = id,
            CommissionRate = request.CommissionRate,
            ActualMonthlySalary = request.ActualMonthlySalary,
            ConfirmedMonthlyQuotaLimit = request.ConfirmedMonthlyQuotaLimit,
            SignedContent = request.SignedContent,
            ScannedDocumentUrl = request.ScannedDocumentUrl
        };
        var result = await _mediator.Send(command);
        return HandleResult(result, "Contract signed and activated.");
    }

    /// <summary>Terminate an Active contract.</summary>
    [HttpPost("{id:guid}/terminate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TerminateContract(Guid id)
    {
        var result = await _mediator.Send(new TerminateContractCommand(id));
        return HandleResult(result, "Contract terminated.");
    }

    /// <summary>Cancel a Draft or PendingSignature contract.</summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelContract(Guid id)
    {
        var result = await _mediator.Send(new CancelContractCommand(id));
        return HandleResult(result, "Contract cancelled.");
    }
}

// =========================================================================
// Request body types
// =========================================================================

public record UpdateContractRequest(Guid TemplateId, int AiQuotaLimit, int MonthlyQuotaLimit, decimal PlatformCommissionRate);
public record SignContractRequest(decimal CommissionRate, decimal ActualMonthlySalary, int? ConfirmedMonthlyQuotaLimit, string? SignedContent, string? ScannedDocumentUrl);
