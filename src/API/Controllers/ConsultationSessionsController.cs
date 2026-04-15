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
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<ConsultationSessionsController> _logger;

    public ConsultationSessionsController(
        IMediator mediator,
        ICurrentUserService currentUser,
        IFileStorageService fileStorageService,
        ILogger<ConsultationSessionsController> logger)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _fileStorageService = fileStorageService;
        _logger = logger;
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
        [FromQuery] Guid? aiScreeningId = null,
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
            AiScreeningId = aiScreeningId,
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
            DiagnosisCode = request.DiagnosisCode,
            CodingSystem = request.CodingSystem,
            ClinicalFindings = request.ClinicalFindings,
            SeverityLevel = request.SeverityLevel,
            ConfidenceLevel = request.ConfidenceLevel,
            TreatmentPlan = request.TreatmentPlan,
            Recommendations = request.Recommendations,
            LifestyleAdvice = request.LifestyleAdvice,
            IsUrgent = request.IsUrgent,
            Status = request.Status,
            FollowUpDate = request.FollowUpDate,
            IsReferralNeeded = request.IsReferralNeeded,
            FinalizedAt = request.FinalizedAt,
            DiagnosesCode = request.DiagnosesCode,
            DiagnosesText = request.DiagnosesText
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
    /// Upload chat images for consultation conversations.
    /// Uses a dedicated storage folder separate from AI screening uploads.
    /// </summary>
    [HttpPost("upload-images")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UploadChatImagesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadChatImages(
        [FromForm] List<IFormFile> images,
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        if (images is null || images.Count == 0)
            return BadRequest(ApiResponseFactory.Error("No images provided"));

        if (images.Count > 10)
            return BadRequest(ApiResponseFactory.Error("Maximum 10 images allowed"));

        var allowedTypes = new[]
        {
            "image/jpeg",
            "image/jpg",
            "image/png",
            "image/bmp",
            "image/tiff",
            "image/x-tiff",
            "image/webp"
        };

        var uploadedUrls = new List<string>();

        try
        {
            foreach (var image in images)
            {
                if (image.Length == 0)
                    return BadRequest(ApiResponseFactory.Error($"File '{image.FileName}' is empty"));

                if (image.Length > 50 * 1024 * 1024)
                    return BadRequest(ApiResponseFactory.Error($"File '{image.FileName}' exceeds 50MB limit"));

                if (!allowedTypes.Contains(image.ContentType?.ToLowerInvariant() ?? string.Empty))
                    return BadRequest(ApiResponseFactory.Error(
                        $"File '{image.FileName}' has unsupported format. Only JPG, JPEG, PNG, BMP, TIFF, and WebP are allowed"));

                await using var stream = image.OpenReadStream();
                var url = await _fileStorageService.SaveFileAsync(
                    stream,
                    image.FileName,
                    $"consultations/chat_images/{_currentUser.UserId}",
                    cancellationToken);

                uploadedUrls.Add(url);

                _logger.LogInformation(
                    "Uploaded consultation chat image to storage: {Url}",
                    url);
            }

            return Ok(ApiResponseFactory.Success(
                new UploadChatImagesResponse
                {
                    UploadedUrls = uploadedUrls,
                    Count = uploadedUrls.Count
                },
                "Chat images uploaded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading consultation chat images");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponseFactory.Error($"Failed to upload chat images: {ex.Message}"));
        }
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

    // New contract fields.
    public string? DiagnosisCode { get; init; }
    public string? CodingSystem { get; init; }
    public string? ClinicalFindings { get; init; }
    public string? SeverityLevel { get; init; }
    public decimal? ConfidenceLevel { get; init; }
    public string? TreatmentPlan { get; init; }
    public string? Recommendations { get; init; }
    public string? LifestyleAdvice { get; init; }
    public bool IsUrgent { get; init; }
    public string? Status { get; init; }
    public DateTime? FollowUpDate { get; init; }
    public bool IsReferralNeeded { get; init; }
    public DateTime? FinalizedAt { get; init; }

    // Backward-compatible aliases for older FE payloads.
    public string? DiagnosesCode { get; init; }
    public string? DiagnosesText { get; init; }
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

public record UploadChatImagesResponse
{
    public List<string> UploadedUrls { get; init; } = new();
    public int Count { get; init; }
}

#endregion
