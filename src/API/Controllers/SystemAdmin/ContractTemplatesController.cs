using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.ContractTemplates.Commands.CreateContractTemplate;
using Application.SystemAdmin.ContractTemplates.Commands.DeleteContractTemplate;
using Application.SystemAdmin.ContractTemplates.Commands.DuplicateContractTemplate;
using Application.SystemAdmin.ContractTemplates.Commands.SetContractTemplateStatus;
using Application.SystemAdmin.ContractTemplates.Commands.UpdateContractTemplate;
using Application.SystemAdmin.ContractTemplates.Common;
using Application.SystemAdmin.ContractTemplates.Queries.GetContractTemplateById;
using Application.SystemAdmin.ContractTemplates.Queries.GetContractTemplates;
using Domain.Enums;
using Infrastructure.Identity.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SystemAdmin;

/// <summary>
/// Contract Template Management.
/// All endpoints require SystemAdmin role.
/// </summary>
[Route("api/system-admin/contract-templates")]
[AuthorizePermission(Permissions.ContractsRead)]
public partial class ContractTemplatesController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorageService;

    public ContractTemplatesController(IMediator mediator, IFileStorageService fileStorageService)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
    }

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

    /// <summary>Get a single contract template.</summary>
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

    /// <summary>Create a new contract template from an uploaded DOCX file.</summary>
    [HttpPost]
    [AuthorizePermission(Permissions.ContractTemplatesManage)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<ContractTemplateDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateContractTemplate([FromForm] UpsertContractTemplateRequest request)
    {
        var uploadResult = await UploadTemplateFileAsync(request.TemplateFile);
        if (!uploadResult.Success)
            return BadRequest(ApiResponseFactory.Error(uploadResult.ErrorMessage!));

        var command = new CreateContractTemplateCommand
        {
            Title = request.Title,
            Type = request.Type,
            EmploymentType = request.EmploymentType,
            ContractVersion = request.ContractVersion,
            ContentTemplate = uploadResult.StoragePath!,
            EffectiveDate = request.EffectiveDate
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Contract template created successfully.");
    }

    /// <summary>Update a contract template metadata and optionally replace its DOCX file.</summary>
    [HttpPut("{id:guid}")]
    [AuthorizePermission(Permissions.ContractTemplatesManage)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<ContractTemplateDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateContractTemplate(Guid id, [FromForm] UpsertContractTemplateRequest request)
    {
        string contentTemplate;
        if (request.TemplateFile is not null)
        {
            var uploadResult = await UploadTemplateFileAsync(request.TemplateFile);
            if (!uploadResult.Success)
                return BadRequest(ApiResponseFactory.Error(uploadResult.ErrorMessage!));

            contentTemplate = uploadResult.StoragePath!;
        }
        else
        {
            var existing = await _mediator.Send(new GetContractTemplateByIdQuery(id));
            if (!existing.IsSuccess || existing.Data is null)
                return HandleResult(existing);

            contentTemplate = existing.Data.ContentTemplate;
        }

        var command = new UpdateContractTemplateCommand
        {
            Id = id,
            Title = request.Title,
            Type = request.Type,
            EmploymentType = request.EmploymentType,
            ContractVersion = request.ContractVersion,
            ContentTemplate = contentTemplate,
            EffectiveDate = request.EffectiveDate
        };
        var result = await _mediator.Send(command);
        return HandleResult(result, "Contract template updated successfully.");
    }

    /// <summary>Delete a contract template permanently.</summary>
    [HttpDelete("{id:guid}")]
    [AuthorizePermission(Permissions.ContractTemplatesManage)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContractTemplate(Guid id)
    {
        var result = await _mediator.Send(new DeleteContractTemplateCommand(id));
        return HandleResult(result, "Contract template deleted successfully.");
    }

    /// <summary>Duplicate a contract template.</summary>
    [HttpPost("{id:guid}/duplicate")]
    [AuthorizePermission(Permissions.ContractTemplatesManage)]
    [ProducesResponseType(typeof(ApiResponse<ContractTemplateDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DuplicateContractTemplate(Guid id)
    {
        var result = await _mediator.Send(new DuplicateContractTemplateCommand(id));
        return HandleResult(result, "Contract template duplicated successfully.");
    }

    /// <summary>Activate or deactivate a contract template.</summary>
    [HttpPatch("{id:guid}/status")]
    [AuthorizePermission(Permissions.ContractTemplatesManage)]
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

public record UpsertContractTemplateRequest
{
    public string Title { get; init; } = string.Empty;
    public ContractType Type { get; init; }
    public OphthalmologistEmploymentType? EmploymentType { get; init; }
    public string ContractVersion { get; init; } = string.Empty;
    public DateTime? EffectiveDate { get; init; }
    public IFormFile? TemplateFile { get; init; }
}

public record TemplateUploadResult(bool Success, string? StoragePath = null, string? ErrorMessage = null);

public partial class ContractTemplatesController
{
    private async Task<TemplateUploadResult> UploadTemplateFileAsync(IFormFile? templateFile)
    {
        if (templateFile is null || templateFile.Length == 0)
            return new TemplateUploadResult(false, ErrorMessage: "Template DOCX file is required.");

        var extension = Path.GetExtension(templateFile.FileName);
        if (!string.Equals(extension, ".docx", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(extension, ".doc", StringComparison.OrdinalIgnoreCase))
            return new TemplateUploadResult(false, ErrorMessage: "Only .doc and .docx files are allowed for contract templates.");

        const long maxSizeBytes = 20 * 1024 * 1024;
        if (templateFile.Length > maxSizeBytes)
            return new TemplateUploadResult(false, ErrorMessage: "Template file size must not exceed 20MB.");

        await using var stream = templateFile.OpenReadStream();
        var storagePath = await _fileStorageService.SaveFileAsync(
            stream,
            templateFile.FileName,
            "admin/contract_templates");

        return new TemplateUploadResult(true, StoragePath: storagePath);
    }
}

public record SetStatusRequest(bool IsActive);
