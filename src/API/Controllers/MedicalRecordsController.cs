using Application.Common.Constants;
using Application.Common.Models;
using Application.MedicalRecords.Commands.CreateMedicalRecord;
using Application.MedicalRecords.Commands.FinalizeMedicalRecord;
using Application.MedicalRecords.Commands.UpdateMedicalRecordClinical;
using Application.MedicalRecords.Common;
using Application.MedicalRecords.Queries.GetMedicalRecordById;
using Application.MedicalRecords.Queries.GetMedicalRecords;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// EMR 23/BV-01 Medical Records management.
/// Lifecycle: Draft (Reception) -> ClinicalFilled (Doctor) -> Locked (Cashier/Finalize).
/// </summary>
public class MedicalRecordsController : BaseApiController
{
    private readonly IMediator _mediator;

    public MedicalRecordsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Step 1: Initialize a new medical record (Section I & II - Administrative).
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
    /// Step 2: Fill clinical pathology and diagnosis (Section III & Part A).
    /// Called by Ophthalmologists only.
    /// </summary>
    [HttpPut("{id:guid}/clinical")]
    [HttpPut("{id:guid}/diagnosis")] // Alias for exact prompt requirement
    // [Authorize(Policy = Permissions.MedicalRecordsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateClinical(Guid id, [FromBody] UpdateMedicalRecordClinicalRequest request)
    {
        var command = new UpdateMedicalRecordClinicalCommand
        {
            Id = id,
            ClinicalDataJson = request.ClinicalDataJson,
            FinalDiagnosis = request.FinalDiagnosis,
            TreatmentPlan = request.TreatmentPlan
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Clinical information updated.");
    }

    /// <summary>
    /// Step 3: Finalize and lock the medical record for legal archiving.
    /// Called by Cashier/Clinic Staff.
    /// </summary>
    [HttpPatch("{id:guid}/finalize")]
    [HttpPost("{id:guid}/finalize")] // Supporting exact prompt requirement
    // [Authorize(Policy = Permissions.MedicalRecordsFinalize)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Finalize(Guid id)
    {
        var result = await _mediator.Send(new FinalizeMedicalRecordCommand(id));
        return HandleResult(result, "Medical record finalized and locked.");
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
}

#region Request Models
public record UpdateMedicalRecordClinicalRequest
{
    public string ClinicalDataJson { get; init; } = string.Empty;
    public string FinalDiagnosis { get; init; } = string.Empty;
    public string TreatmentPlan { get; init; } = string.Empty;
}
#endregion
