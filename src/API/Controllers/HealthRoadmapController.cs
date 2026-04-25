using Application.CarePlan.HealthRoadmaps.Commands.CompleteRoadmapStep;
using Application.CarePlan.HealthRoadmaps.Commands.CreateRoadmapStep;
using Application.CarePlan.HealthRoadmaps.Commands.DeleteRoadmapStep;
using Application.CarePlan.HealthRoadmaps.Commands.UpdateRoadmapStep;
using Application.CarePlan.HealthRoadmaps.Common;
using Application.CarePlan.HealthRoadmaps.Queries.GetPatientHealthRoadmap;
using Application.Common.Constants;
using Application.Common.Models;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Healthcare Roadmap (Patient Care Plan) — doctor-authored, structured timeline of future care steps.
/// Distinct from the AI-generated <c>PatientRoadmap</c> exposed under <c>api/patient/roadmaps</c>.
/// </summary>
[Route("api/roadmap")]
[Authorize]
public class HealthRoadmapController : BaseApiController
{
    private readonly IMediator _mediator;

    public HealthRoadmapController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public sealed record CreateRoadmapStepRequest(
        Guid PatientId,
        string Title,
        string? Description,
        RoadmapStepType StepType,
        DateOnly PlannedDate,
        Guid? CreatedFromVisitId,
        int? OrderIndex);

    public sealed record UpdateRoadmapStepRequest(
        string? Title,
        string? Description,
        RoadmapStepType? StepType,
        DateOnly? PlannedDate,
        int? OrderIndex);

    /// <summary>POST /api/roadmap/steps - doctor creates a new roadmap step for a patient.</summary>
    [HttpPost("steps")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<HealthRoadmapStepDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateStep([FromBody] CreateRoadmapStepRequest request)
    {
        var command = new CreateRoadmapStepCommand
        {
            PatientId = request.PatientId,
            Title = request.Title,
            Description = request.Description,
            StepType = request.StepType,
            PlannedDate = request.PlannedDate,
            CreatedFromVisitId = request.CreatedFromVisitId,
            OrderIndex = request.OrderIndex ?? 0
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>GET /api/roadmap/{patientId} - returns roadmap and steps sorted by planned date.</summary>
    [HttpGet("{patientId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<HealthRoadmapDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatientRoadmap(Guid patientId)
    {
        var result = await _mediator.Send(new GetPatientHealthRoadmapQuery(patientId));
        return HandleResult(result);
    }

    /// <summary>PATCH /api/roadmap/steps/{id} - update a roadmap step (only Upcoming steps).</summary>
    [HttpPatch("steps/{id:guid}")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<HealthRoadmapStepDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStep(Guid id, [FromBody] UpdateRoadmapStepRequest request)
    {
        var command = new UpdateRoadmapStepCommand
        {
            StepId = id,
            Title = request.Title,
            Description = request.Description,
            StepType = request.StepType,
            PlannedDate = request.PlannedDate,
            OrderIndex = request.OrderIndex
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>POST /api/roadmap/steps/{id}/complete - mark step as Completed.</summary>
    [HttpPost("steps/{id:guid}/complete")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<HealthRoadmapStepDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CompleteStep(Guid id)
    {
        var result = await _mediator.Send(new CompleteRoadmapStepCommand(id));
        return HandleResult(result);
    }

    /// <summary>DELETE /api/roadmap/steps/{id} - delete a non-completed step.</summary>
    [HttpDelete("steps/{id:guid}")]
    [Authorize(Policy = Policies.OphthalmologistOnly)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteStep(Guid id)
    {
        var result = await _mediator.Send(new DeleteRoadmapStepCommand(id));
        return HandleResult(result);
    }
}
