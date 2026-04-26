using Application.Common.Constants;
using Application.Common.Models;
using Application.Feedback.Commands.CreateClinicFeedback;
using Application.Feedback.Commands.CreateWebsiteFeedback;
using Application.Feedback.Common;
using Application.Feedback.Queries.GetOphthalmologistFeedback;
using Application.Feedback.Queries.GetOphthalmologistRatingSummary;
using Application.Feedback.Queries.GetClinicFeedback;
using Application.Feedback.Queries.GetClinicRatingSummary;
using Application.Feedback.Queries.GetWebsiteFeedback;
using Application.Feedback.Queries.ListOphthalmologistFeedback;
using Application.Feedback.Queries.ListClinicFeedback;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/feedback")]
public class FeedbackController : BaseApiController
{
    private readonly IMediator _mediator;

    public FeedbackController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("website")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateWebsiteFeedback([FromBody] CreateWebsiteFeedbackRequest request)
    {
        var command = new CreateWebsiteFeedbackCommand
        {
            Rating = request.Rating,
            Category = request.Category,
            Comment = request.Comment
        };

        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetWebsiteFeedback),
                new { feedbackId = result.Data },
                ApiResponseFactory.Success(result.Data, "Website feedback created successfully."));
        }

        return HandleResult(result);
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
            OrganisationId = clinicId,
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

    [HttpPost("ophthalmologists/{ophthalmologistId:guid}")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOphthalmologistFeedback(
        Guid ophthalmologistId,
        [FromBody] CreateOphthalmologistFeedbackRequest request)
    {
        var command = new CreateOphthalmologistFeedbackCommand
        {
            OphthalmologistId = ophthalmologistId,
            ConsultationSessionId = request.ConsultationSessionId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetOphthalmologistFeedback),
                new { ophthalmologistId, feedbackId = result.Data },
                ApiResponseFactory.Success(result.Data, "Ophthalmologist feedback created successfully."));
        }

        return HandleResult(result);
    }

    [HttpGet("website/{feedbackId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<WebsiteFeedbackDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWebsiteFeedback(Guid feedbackId)
    {
        var result = await _mediator.Send(new GetWebsiteFeedbackQuery(feedbackId));
        return HandleResult(result);
    }

    [HttpGet("clinics/{clinicId:guid}/items/{feedbackId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ClinicFeedbackDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClinicFeedback(Guid clinicId, Guid feedbackId)
    {
        var result = await _mediator.Send(new GetClinicFeedbackQuery(clinicId, feedbackId));
        return HandleResult(result);
    }

    [HttpGet("ophthalmologists/{ophthalmologistId:guid}/items/{feedbackId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OphthalmologistFeedbackDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOphthalmologistFeedback(Guid ophthalmologistId, Guid feedbackId)
    {
        var result = await _mediator.Send(new GetOphthalmologistFeedbackQuery(ophthalmologistId, feedbackId));
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
            OrganisationId = clinicId,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return HandleResult(result);
    }

    [HttpGet("ophthalmologists/{ophthalmologistId:guid}/items")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OphthalmologistFeedbackDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListOphthalmologistFeedback(
        Guid ophthalmologistId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new ListOphthalmologistFeedbackQuery
        {
            OphthalmologistId = ophthalmologistId,
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
        var result = await _mediator.Send(new GetClinicRatingSummaryQuery(clinicId));
        return HandleResult(result);
    }

    [HttpGet("ophthalmologists/{ophthalmologistId:guid}/rating")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<FeedbackRatingSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOphthalmologistRatingSummary(Guid ophthalmologistId)
    {
        var result = await _mediator.Send(new GetOphthalmologistRatingSummaryQuery(ophthalmologistId));
        return HandleResult(result);
    }
}

#region Request Models

public record CreateWebsiteFeedbackRequest
{
    public int Rating { get; init; }
    public Domain.Enums.WebsiteFeedbackCategory Category { get; init; }
    public string? Comment { get; init; }
}

public record CreateClinicFeedbackRequest
{
    public Guid AppointmentId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public Guid? DoctorId { get; init; }
    public Guid? StaffId { get; init; }
}

public record CreateOphthalmologistFeedbackRequest
{
    public Guid ConsultationSessionId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}

#endregion
