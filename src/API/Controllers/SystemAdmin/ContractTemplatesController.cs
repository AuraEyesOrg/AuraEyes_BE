using Application.Common.Constants;
using Application.Common.Models;
using Application.SystemAdmin.ContractTemplates.Commands.CreateContractTemplate;
using Application.SystemAdmin.ContractTemplates.Commands.DeleteContractTemplate;
using Application.SystemAdmin.ContractTemplates.Commands.SetContractTemplateStatus;
using Application.SystemAdmin.ContractTemplates.Commands.UpdateContractTemplate;
using Application.SystemAdmin.ContractTemplates.Common;
using Application.SystemAdmin.ContractTemplates.Queries.GetContractTemplateById;
using Application.SystemAdmin.ContractTemplates.Queries.GetContractTemplates;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// Contract Template Management — CRUD for HTML contract templates and their typed variable definitions.
/// All endpoints require SystemAdmin role.
/// </summary>
[Route("api/system-admin/contract-templates")]
[Authorize(Policy = Policies.SystemAdminOnly)]
public class ContractTemplatesController : BaseApiController
{
    private readonly IMediator _mediator;

    public ContractTemplatesController(IMediator mediator) => _mediator = mediator;

    // =========================================================================
    // QUERIES
    // =========================================================================

    /// <summary>Get a paged list of contract templates.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ContractTemplateDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContractTemplates(
        [FromQuery] string? searchTerm = null,
        [FromQuery] ContractType? type = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetContractTemplatesQuery
        {
            SearchTerm = searchTerm,
            Type = type,
            IsActive = isActive,
            PageNumber = pageNumber,
            PageSize = Math.Min(pageSize, 100)
        };
        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>Get a single contract template with its full variable definitions and HTML content.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ContractTemplateDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContractTemplateById(Guid id)
    {
        var result = await _mediator.Send(new GetContractTemplateByIdQuery(id));
        return HandleResult(result);
    }

    // =========================================================================
    // COMMANDS
    // =========================================================================

    /// <summary>Create a new contract template with variable definitions.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ContractTemplateDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateContractTemplate([FromBody] CreateContractTemplateCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "Contract template created successfully.");
    }

    /// <summary>Update a contract template — replaces content and variable definitions.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ContractTemplateDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateContractTemplate(Guid id, [FromBody] UpdateContractTemplateRequest request)
    {
        var command = new UpdateContractTemplateCommand
        {
            Id = id,
            Title = request.Title,
            Type = request.Type,
            ContractVersion = request.ContractVersion,
            ContentTemplate = request.ContentTemplate,
            EffectiveDate = request.EffectiveDate,
            Variables = request.Variables
        };
        var result = await _mediator.Send(command);
        return HandleResult(result, "Contract template updated successfully.");
    }

    /// <summary>Delete a contract template permanently.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContractTemplate(Guid id)
    {
        var result = await _mediator.Send(new DeleteContractTemplateCommand(id));
        return HandleResult(result, "Contract template deleted successfully.");
    }

    /// <summary>Activate or deactivate a contract template.</summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetContractTemplateStatus(Guid id, [FromBody] SetStatusRequest request)
    {
        var result = await _mediator.Send(new SetContractTemplateStatusCommand(id, request.IsActive));
        return HandleResult(result, request.IsActive ? "Template activated." : "Template deactivated.");
    }
}

// =========================================================================
// Request body types (thin wrappers to allow route id binding)
// =========================================================================

public record UpdateContractTemplateRequest(
    string Title,
    ContractType Type,
    string ContractVersion,
    string ContentTemplate,
    DateTime? EffectiveDate,
    List<UpsertVariableRequest> Variables);

public record SetStatusRequest(bool IsActive);
