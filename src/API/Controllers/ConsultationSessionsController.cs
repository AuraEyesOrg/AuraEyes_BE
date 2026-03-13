using Application.Common.Constants;
using Application.Common.Models;
using Application.ConsultationSessions.Commands.CreateVerificationSession;
using Application.ConsultationSessions.Commands.CreateVideoCallSession;
using Application.ConsultationSessions.Commands.CancelSession;
using Application.ConsultationSessions.Commands.EndSession;
using Application.ConsultationSessions.Commands.SendMessage;
using Application.ConsultationSessions.Commands.SubmitVerificationReport;
using Application.ConsultationSessions.Common;
using Application.ConsultationSessions.Queries.GetConsultationSession;
using Application.ConsultationSessions.Queries.GetConsultationSessions;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Common.Interfaces;

namespace API.Controllers;

/// <summary>
/// Consultation session management endpoints.
/// Handles Verification, VideoCall, and ClinicBooking session lifecycles.
/// </summary>
[Route("api/consultation-sessions")]
public class ConsultationSessionsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public ConsultationSessionsController(
        IMediator mediator,
        ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Get paginated list of consultation sessions with optional filters.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ConsultationSessionListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSessions(
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? ophthalmologistId = null,
        [FromQuery] ConsultationSessionType? type = null,
        [FromQuery] SessionStatus? status = null,
        [FromQuery] ChatStatus? chatStatus = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetConsultationSessionsQuery
        {
            PatientId = patientId,
            OphthalmologistId = ophthalmologistId,
            Type = type,
            Status = status,
            ChatStatus = chatStatus,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a specific consultation session by ID.
    /// </summary>
    [HttpGet("{sessionId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ConsultationSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSession(Guid sessionId)
    {
        var result = await _mediator.Send(new GetConsultationSessionQuery(sessionId));
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new Verification session (Paid AI-report review by doctor).
    /// </summary>
    [HttpPost("verification")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateVerificationSession(
        [FromBody] CreateVerificationSessionRequest request)
    {
        var command = new CreateVerificationSessionCommand
        {
            PatientId = request.PatientId,
            AiScreeningId = request.AiScreeningId,
            Price = request.Price,
            OphthalmologistId = request.OphthalmologistId
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetSession),
                new { sessionId = result.Data },
                ApiResponseFactory.Success(result.Data, "Verification session created successfully."));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Create a new VideoCall session.
    /// </summary>
    [HttpPost("video-call")]
    [Authorize(Policy = Policies.PatientOnly)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateVideoCallSession(
        [FromBody] CreateVideoCallSessionRequest request)
    {
        var command = new CreateVideoCallSessionCommand
        {
            PatientId = request.PatientId,
            Price = request.Price,
            AppointmentTime = request.AppointmentTime,
            OphthalmologistId = request.OphthalmologistId
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetSession),
                new { sessionId = result.Data },
                ApiResponseFactory.Success(result.Data, "Video call session created successfully."));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Doctor submits a verification report. Opens the 2-way chat.
    /// </summary>
    [HttpPost("{sessionId:guid}/verification-report")]
    [Authorize(Policy = Policies.VerifiedOphthalmologist)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitVerificationReport(
        Guid sessionId,
        [FromBody] SubmitVerificationReportRequest request)
    {
        var command = new SubmitVerificationReportCommand
        {
            SessionId = sessionId,
            DoctorId = request.DoctorId,
            DiagnosesCode = request.DiagnosesCode,
            DiagnosesText = request.DiagnosesText,
            TreatmentPlan = request.TreatmentPlan
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Verification report submitted. Chat is now open.");
    }

    /// <summary>
    /// Send a message in a session's conversation. Updates LastActivityAt.
    /// </summary>
    [HttpPost("{sessionId:guid}/messages")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendMessage(
        Guid sessionId,
        [FromBody] SendMessageRequest request)
    {
        if (!_currentUser.ProfileId.HasValue)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized(
                "Authenticated profile is required to send messages."));
        }

        var command = new SendMessageCommand
        {
            SessionId = sessionId,
            Message = request.Message
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Message sent successfully.");
    }

    /// <summary>
    /// Cancel a session. Deletes the associated Google Calendar event if present.
    /// </summary>
    [HttpPost("{sessionId:guid}/cancel")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSession(
        Guid sessionId,
        [FromBody] CancelSessionRequest request)
    {
        var command = new CancelSessionCommand
        {
            SessionId = sessionId,
            CancelledByUserId = request.CancelledByUserId,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Session cancelled successfully.");
    }

    /// <summary>
    /// Doctor manually ends/closes a session. Archives the chat.
    /// </summary>
    [HttpPost("{sessionId:guid}/end")]
    [Authorize(Policy = Policies.VerifiedOphthalmologist)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EndSession(
        Guid sessionId,
        [FromBody] EndSessionRequest request)
    {
        var command = new EndSessionCommand
        {
            SessionId = sessionId,
            DoctorId = request.DoctorId
        };

        var result = await _mediator.Send(command);
        return HandleResult(result, "Session ended successfully.");
    }
}

#region Request Models

public record CreateVerificationSessionRequest
{
    public Guid PatientId { get; init; }
    public Guid AiScreeningId { get; init; }
    public decimal Price { get; init; }
    public Guid? OphthalmologistId { get; init; }
}

public record CreateVideoCallSessionRequest
{
    public Guid PatientId { get; init; }
    public decimal Price { get; init; }
    public DateTime AppointmentTime { get; init; }
    public Guid? OphthalmologistId { get; init; }
}

public record SubmitVerificationReportRequest
{
    public Guid DoctorId { get; init; }
    public string DiagnosesCode { get; init; } = string.Empty;
    public string DiagnosesText { get; init; } = string.Empty;
    public string? TreatmentPlan { get; init; }
}

public record SendMessageRequest
{
    public string Message { get; init; } = string.Empty;
}

public record CancelSessionRequest
{
    public Guid CancelledByUserId { get; init; }
    public string? Reason { get; init; }
}

public record EndSessionRequest
{
    public Guid DoctorId { get; init; }
}

#endregion
