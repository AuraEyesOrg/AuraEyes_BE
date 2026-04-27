using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.MedicalRecords.Commands.CreateMedicalRecord;
using Application.MedicalRecords.Queries.ExportMedicalRecordPdf;
using Application.MedicalRecords.Commands.FinalizeMedicalRecord;
using Application.MedicalRecords.Commands.UpdateMedicalRecordClinical;
using Application.MedicalRecords.Commands.StartDoctorFilling;
using Application.MedicalRecords.Commands.UpdateMedicalRecordAdministrative;
using Application.MedicalRecords.Common;
using Application.MedicalRecords.Queries.GetMedicalRecordById;
using Application.MedicalRecords.Queries.GetMedicalRecords;
using Domain.Entities.MedicalRecords;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers;

/// <summary>
/// EMR 23/BV-01 Medical Records management.
/// Lifecycle: Draft_Admin -> Pending_Clinical -> Finalized.
/// </summary>
public class MedicalRecordsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public MedicalRecordsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Step 1: Initialize a new medical record (Section I and II - Administrative).
    /// Called by Receptionist/Clinic Staff.
    /// </summary>
    [HttpPost]
    // [Authorize(Policy = Permissions.MedicalRecordsCreate)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateMedicalRecordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result, "Medical record initialized successfully.");
    }

    /// <summary>
    /// Update administrative data (Section I and II).
    /// Called by Clinic Staff.
    /// </summary>
    [HttpPut("{id:guid}/administrative")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAdministrative(Guid id, [FromBody] UpdateMedicalRecordAdministrativeRequest request)
    {
        if (!CanWriteAdministrativeData())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponseFactory.Forbidden("Only Clinic Staff or System Admin can update administrative EMR data."));

        var administrativeDataJson = ResolveJsonPayload(request.AdministrativeDataJson, request.AdministrativeData);

        var command = new UpdateMedicalRecordAdministrativeCommand
        {
            Id = id,
            AdministrativeDataJson = administrativeDataJson
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Administrative information updated.");
    }

    /// <summary>
    /// Step 1.5: Transition record to DoctorFilling status.
    /// Called when an ophthalmologist begins working on a record.
    /// </summary>
    [HttpPost("{id:guid}/start-consultation")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartDoctorFilling(Guid id)
    {
        if (!CanWriteClinicalData())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponseFactory.Forbidden("Only Ophthalmologist or System Admin can start clinical EMR workflow."));

        var result = await _mediator.Send(new StartDoctorFillingCommand(id));
        return HandleResult(result, "Medical record status updated to Doctor Filling.");
    }

    /// <summary>
    /// Step 2: Fill clinical pathology and diagnosis (Section III and Part A).
    /// Called by Ophthalmologists only.
    /// </summary>
    [HttpPut("{id:guid}/clinical")]
    [HttpPut("{id:guid}/diagnosis")] // Alias for exact prompt requirement
    // [Authorize(Policy = Permissions.MedicalRecordsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateClinical(Guid id, [FromBody] UpdateMedicalRecordClinicalRequest request)
    {
        if (!CanWriteClinicalData())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponseFactory.Forbidden("Only Ophthalmologist or System Admin can update clinical EMR data."));

        var clinicalDataJson = ResolveJsonPayload(request.ClinicalDataJson, request.ClinicalData);

        var command = new UpdateMedicalRecordClinicalCommand
        {
            Id = id,
            ClinicalDataJson = clinicalDataJson,
            FinalDiagnosis = request.FinalDiagnosis,
            TreatmentPlan = request.TreatmentPlan
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Clinical information updated.");
    }

    /// <summary>
    /// Step 3: Finalize and lock the medical record for legal archiving.
    /// Called by Ophthalmologist.
    /// </summary>
    [HttpPatch("{id:guid}/finalize")]
    [HttpPost("{id:guid}/finalize")] // Supporting exact prompt requirement
    // [Authorize(Policy = Permissions.MedicalRecordsFinalize)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Finalize(Guid id)
    {
        if (!CanFinalize())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponseFactory.Forbidden("Only Ophthalmologist or System Admin can finalize EMR."));

        var result = await _mediator.Send(new FinalizeMedicalRecordCommand(id));
        return HandleResult(result, "Medical record finalized and locked.");
    }

    /// <summary>
    /// Download finalized EMR as PDF.
    /// </summary>
    [HttpGet("{id:guid}/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadPdf(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ExportMedicalRecordPdfQuery(id), cancellationToken);
        if (!result.IsSuccess || result.Data is null)
            return HandleResult(result, "EMR PDF generated");

        Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

        return File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
    }

    /// <summary>
    /// Retrieve medical record details.
    /// </summary>
    [HttpGet("{id:guid}")]
    // [Authorize(Policy = Permissions.MedicalRecordsRead)]
    [ProducesResponseType(typeof(ApiResponse<MedicalRecordDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetMedicalRecordByIdQuery(id));
        return HandleResult(result);
    }

    /// <summary>
    /// Step 4: Retrieve all medical records with filtering (Staff/Admin).
    /// </summary>
    [HttpGet("all")]
    // [Authorize(Policy = Permissions.MedicalRecordsRead)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MedicalRecordDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] MedicalRecordStatus? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetMedicalRecordsQuery
        {
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return HandleResult(result);
    }

    private bool CanWriteAdministrativeData()
    {
        return _currentUserService.IsInRole(Roles.ClinicStaff) || _currentUserService.IsInRole(Roles.SystemAdmin);
    }

    private bool CanWriteClinicalData()
    {
        return _currentUserService.IsInRole(Roles.Ophthalmologist) || _currentUserService.IsInRole(Roles.SystemAdmin);
    }

    private bool CanFinalize()
    {
        return _currentUserService.IsInRole(Roles.Ophthalmologist) || _currentUserService.IsInRole(Roles.SystemAdmin);
    }

    private static string ResolveJsonPayload(string? rawJson, JsonElement? jsonElement)
    {
        if (!string.IsNullOrWhiteSpace(rawJson))
            return rawJson;

        if (jsonElement.HasValue && jsonElement.Value.ValueKind != JsonValueKind.Undefined && jsonElement.Value.ValueKind != JsonValueKind.Null)
            return jsonElement.Value.GetRawText();

        return "{}";
    }
}

#region Request Models
public record UpdateMedicalRecordAdministrativeRequest
{
    public string? AdministrativeDataJson { get; init; }
    public JsonElement? AdministrativeData { get; init; }
}

public record UpdateMedicalRecordClinicalRequest
{
    public string? ClinicalDataJson { get; init; }
    public JsonElement? ClinicalData { get; init; }
    public string FinalDiagnosis { get; init; } = string.Empty;
    public string TreatmentPlan { get; init; } = string.Empty;
}
#endregion
