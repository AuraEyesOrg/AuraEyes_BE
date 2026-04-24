using Application.Common.Constants;
using Application.Common.Models;
using Application.Scheduling.ScheduleTemplates.Commands.CreateScheduleTemplate;
using Application.Scheduling.ScheduleTemplates.Commands.DeleteScheduleTemplate;
using Application.Scheduling.ScheduleTemplates.Commands.UpdateScheduleTemplate;
using Application.Scheduling.ScheduleTemplates.Common;
using Application.Scheduling.ScheduleTemplates.Queries.GetScheduleTemplate;
using Application.Scheduling.ScheduleTemplates.Queries.GetScheduleTemplates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Schedule template management endpoints for creating recurring availability patterns.
/// </summary>
[Route("api/schedule-templates")]
public class ScheduleTemplatesController : BaseApiController
{
    private readonly IMediator _mediator;

    public ScheduleTemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get schedule templates with pagination and filtering.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ScheduleTemplateListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetScheduleTemplates(
        [FromQuery] DayOfWeek? dayOfWeek = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetScheduleTemplatesQuery
        {
            DayOfWeek = dayOfWeek,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a specific schedule template by ID.
    /// </summary>
    [HttpGet("{templateId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ScheduleTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetScheduleTemplate(Guid templateId)
    {
        var result = await _mediator.Send(new GetScheduleTemplateQuery(templateId));
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new schedule template.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateScheduleTemplate([FromBody] CreateScheduleTemplateRequest request)
    {
        var command = new CreateScheduleTemplateCommand
        {
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotDuration = request.SlotDuration,
            MaxCapacity = request.MaxCapacity
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetScheduleTemplate),
                new { templateId = result.Data },
                ApiResponseFactory.Success(result.Data, "Schedule template created successfully."));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing schedule template.
    /// </summary>
    [HttpPut("{templateId:guid}")]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateScheduleTemplate(Guid templateId, [FromBody] UpdateScheduleTemplateRequest request)
    {
        var command = new UpdateScheduleTemplateCommand
        {
            ScheduleTemplateId = templateId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotDuration = request.SlotDuration,
            MaxCapacity = request.MaxCapacity,
            IsActive = request.IsActive
        };

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a schedule template (soft delete).
    /// </summary>
    [HttpDelete("{templateId:guid}")]
    [Authorize(Policy = Policies.OphthalmologistOrOrgAdmin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteScheduleTemplate(Guid templateId)
    {
        var command = new DeleteScheduleTemplateCommand { ScheduleTemplateId = templateId };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }
}

public record CreateScheduleTemplateRequest
{
    public DayOfWeek DayOfWeek { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int SlotDuration { get; init; }
    public int MaxCapacity { get; init; }
}

public record UpdateScheduleTemplateRequest
{
    public DayOfWeek DayOfWeek { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int SlotDuration { get; init; }
    public int MaxCapacity { get; init; }
    public bool IsActive { get; init; }
}
