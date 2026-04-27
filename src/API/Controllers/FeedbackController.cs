using Application.Common.Constants;
using Application.Common.Models;
using Application.Feedback.Common;
using Application.Feedback.Queries.ListClinicFeedback;
using Application.Feedback.Queries.GetClinicRatingSummary;
using Application.Feedback.Queries.GetClinicFeedback;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Feedback.Commands.CreateClinicFeedback;

namespace API.Controllers;

[Route("api/feedback")]
public class FeedbackController : BaseApiController
{
    private readonly IMediator _mediator;

    public FeedbackController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("clinics/{clinicId:guid}")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateClinicFeedback(
        Guid clinicId,
        [FromBody] CreateClinicFeedbackRequest request)
    {
        var command = new CreateClinicFeedbackCommand
        {
            AppointmentId = request.AppointmentId,
            Rating = request.Rating,
            Comment = request.Comment,
            DoctorId = request.DoctorId,
            StaffId = request.StaffId
        };

        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetClinicFeedback),
                new { clinicId, feedbackId = result.Data },
                ApiResponseFactory.Success(result.Data, "Clinic feedback created successfully."));
        }

        return HandleResult(result);
    }

    [HttpGet("clinics/{clinicId:guid}/items/{feedbackId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ClinicFeedbackDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClinicFeedback(Guid clinicId, Guid feedbackId)
    {
        var result = await _mediator.Send(new GetClinicFeedbackQuery(feedbackId));
        return HandleResult(result);
    }

    [HttpGet("clinics/{clinicId:guid}/items")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClinicFeedbackDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListClinicFeedback(
        Guid clinicId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new ListClinicFeedbackQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return HandleResult(result);
    }

    [HttpGet("clinics/{clinicId:guid}/rating")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<FeedbackRatingSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClinicRatingSummary(Guid clinicId)
    {
        var result = await _mediator.Send(new GetClinicRatingSummaryQuery());
        return HandleResult(result);
    }
}

#region Request Models

public record CreateClinicFeedbackRequest
{
    public Guid AppointmentId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public Guid? DoctorId { get; init; }
    public Guid? StaffId { get; init; }
}

#endregion
