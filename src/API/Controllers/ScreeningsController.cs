using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Screenings.Commands.CreateAiScreeningSession;
using Application.Screenings.Commands.SaveAiScreeningResults;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// AI Screening endpoints.
/// Provides screening operations including saving AI analysis results.
/// </summary>
[Route("api/screenings")]
[Authorize]
public class ScreeningsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<ScreeningsController> _logger;

    public ScreeningsController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        IFileStorageService fileStorageService,
        ILogger<ScreeningsController> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    [HttpGet("recent")]
    [ProducesResponseType(typeof(ApiResponse<List<ScreeningSessionSummaryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecentSessions(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var patients = await _patientRepository.FindAsync(
            p => p.UserId == _currentUserService.UserId.Value,
            cancellationToken);

        var patient = patients.FirstOrDefault();
        if (patient is null)
            return NotFound(ApiResponseFactory.NotFound("Patient profile not found"));

        var cappedLimit = Math.Clamp(limit, 1, 50);

        var sessions = await _screeningRepository
            .Query()
            .Where(s => s.PatientId == patient.Id && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .Take(cappedLimit)
            .Select(s => new ScreeningSessionSummaryResponse
            {
                ScreeningId = s.Id,
                ModelVersion = s.ModelVersion,
                CreatedAt = s.CreatedAt,
                ProcessedAt = s.ProcessedAt,
                IsActive = s.IsActive,
                ImagesCount = s.RetinalImages.Count,
                ThumbnailUrl = s.RetinalImages
                    .OrderBy(i => i.CreatedAt)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),
                LatestRiskLevel = s.ScreeningResults
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => r.RiskLevel.ToString())
                    .FirstOrDefault(),
            })
            .ToListAsync(cancellationToken);

        return Ok(ApiResponseFactory.Success(sessions, "Recent screening sessions loaded"));
    }

    [HttpGet("{screeningId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ScreeningSessionDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSessionById(
        [FromRoute] Guid screeningId,
        CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var patients = await _patientRepository.FindAsync(
            p => p.UserId == _currentUserService.UserId.Value,
            cancellationToken);

        var patient = patients.FirstOrDefault();
        if (patient is null)
            return NotFound(ApiResponseFactory.NotFound("Patient profile not found"));

        var session = await _screeningRepository
            .Query()
            .Where(s => s.Id == screeningId && s.PatientId == patient.Id && !s.IsDeleted)
            .Select(s => new ScreeningSessionDetailResponse
            {
                ScreeningId = s.Id,
                PatientId = s.PatientId,
                ModelVersion = s.ModelVersion,
                CreatedAt = s.CreatedAt,
                ProcessedAt = s.ProcessedAt,
                RawJsonOutput = s.RawJsonOutput,
                IsActive = s.IsActive,
                Images = s.RetinalImages
                    .OrderBy(i => i.CreatedAt)
                    .Select(i => new RetinalImageDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        EyeSide = i.EyeSide.ToString(),
                        DeviceName = i.DeviceName,
                        QualityScore = i.QualityScore,
                        CapturedAt = i.CapturedAt,
                    })
                    .ToList(),
                LatestResult = s.ScreeningResults
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new ScreeningResultDto
                    {
                        ScreeningResultId = r.Id,
                        RiskLevel = r.RiskLevel.ToString(),
                        ConfidenceScore = r.ConfidenceScore,
                        Summary = r.Summary,
                        Findings = r.Findings,
                        AssessedAt = r.CreatedAt,
                    })
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
            return NotFound(ApiResponseFactory.NotFound("Screening session not found"));

        return Ok(ApiResponseFactory.Success(session, "Screening session loaded"));
    }

    /// <summary>
    /// Create a new AI screening session with optional retinal images.
    /// Should be called at the start of screening workflow.
    /// </summary>
    [HttpPost("create-session")]
    [ProducesResponseType(typeof(ApiResponse<CreateAiScreeningSessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateScreeningSession(
        [FromBody] CreateScreeningSessionRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        var command = new CreateAiScreeningSessionCommand
        {
            ModelVersion = request.ModelVersion ?? "CFP_v1",
            RetinalImages = request.RetinalImages ?? new List<RetinalImageData>()
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "Screening session created successfully");
    }

    /// <summary>
    /// Save AI screening results after analysis completes.
    /// Stores raw JSON output, risk assessment, and retinal images.
    /// </summary>
    /// <remarks>
    /// This endpoint should be called after AI analysis finishes and results are ready.
    /// Images should already be uploaded to Supabase and URLs provided.
    /// </remarks>
    [HttpPost("{screeningId:guid}/save-results")]
    [ProducesResponseType(typeof(ApiResponse<SaveAiScreeningResultsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveAiScreeningResults(
        [FromRoute] Guid screeningId,
        [FromBody] SaveAiScreeningResultsRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Unauthorized(ApiResponseFactory.Unauthorized("User not authenticated"));

        // Validate request
        if (string.IsNullOrWhiteSpace(request.RawJsonOutput))
            return BadRequest(ApiResponseFactory.Error("Raw JSON output is required"));

        if (request.ConfidenceScore < 0 || request.ConfidenceScore > 100)
            return BadRequest(ApiResponseFactory.Error("Confidence score must be between 0 and 100"));

        _logger.LogInformation(
            "Saving AI screening results for screening {ScreeningId}, user {UserId}, " +
            "risk level {RiskLevel}, confidence {Score}%",
            screeningId,
            _currentUserService.UserId,
            request.RiskLevel,
            request.ConfidenceScore);

        var command = new SaveAiScreeningResultsCommand
        {
            ScreeningId = screeningId,
            RawJsonOutput = request.RawJsonOutput,
            RiskLevel = request.RiskLevel,
            ConfidenceScore = request.ConfidenceScore,
            Summary = request.Summary,
            Findings = request.Findings
        };

        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result, "AI screening results saved successfully");
    }

    /// <summary>
    /// Upload retinal images to Supabase storage.
    /// Returns public URLs. Images are persisted to database when creating screening session.
    /// </summary>
    [HttpPost]
    [Route("upload-images")]
    [ProducesResponseType(typeof(ApiResponse<UploadRetinalImagesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadRetinalImages(
        [FromForm] List<IFormFile> images,
        CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId is null)
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
                // Validate file
                if (image.Length == 0)
                    return BadRequest(ApiResponseFactory.Error($"File '{image.FileName}' is empty"));

                if (image.Length > 50 * 1024 * 1024) // 50MB max
                    return BadRequest(ApiResponseFactory.Error($"File '{image.FileName}' exceeds 50MB limit"));

                if (!allowedTypes.Contains(image.ContentType?.ToLowerInvariant() ?? ""))
                    return BadRequest(ApiResponseFactory.Error(
                        $"File '{image.FileName}' has unsupported format. Only JPG, JPEG, PNG, BMP, TIFF, and WebP are allowed"));

                // Upload to Supabase storage
                await using var stream = image.OpenReadStream();
                var url = await _fileStorageService.SaveFileAsync(
                    stream,
                    image.FileName,
                    $"screenings/{_currentUserService.UserId}/images",
                    cancellationToken);

                uploadedUrls.Add(url);
                _logger.LogInformation(
                    "Uploaded retinal image to storage: {Url}",
                    url);
            }

            return Ok(ApiResponseFactory.Success(
                new UploadRetinalImagesResponse
                {
                    UploadedUrls = uploadedUrls,
                    Count = uploadedUrls.Count
                },
                "Images uploaded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading retinal images");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponseFactory.Error($"Failed to upload images: {ex.Message}"));
        }
    }
}

/// <summary>
/// Request to save AI screening results
/// </summary>
public record SaveAiScreeningResultsRequest
{
    /// <summary>
    /// Raw JSON output from AI model with all detection details
    /// </summary>
    public string RawJsonOutput { get; init; } = string.Empty;

    /// <summary>
    /// Overall risk level assessment
    /// </summary>
    public RiskLevel RiskLevel { get; init; }

    /// <summary>
    /// Confidence score for the risk assessment (0-100)
    /// </summary>
    public decimal ConfidenceScore { get; init; }

    /// <summary>
    /// Summary of findings
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Detailed findings from the analysis
    /// </summary>
    public string? Findings { get; init; }
}

/// <summary>
/// Request to create a screening session
/// </summary>
public record CreateScreeningSessionRequest
{
    /// <summary>
    /// AI model version to use
    /// </summary>
    public string? ModelVersion { get; init; }

    /// <summary>
    /// Optional: retinal images with URLs already uploaded to Supabase
    /// </summary>
    public List<RetinalImageData>? RetinalImages { get; init; }
}

/// <summary>
/// Response from uploading retinal images
/// </summary>
public record UploadRetinalImagesResponse
{
    /// <summary>
    /// URLs of uploaded images
    /// </summary>
    public List<string> UploadedUrls { get; init; } = new();

    /// <summary>
    /// Number of uploaded images
    /// </summary>
    public int Count { get; init; }
}

public record ScreeningSessionSummaryResponse
{
    public Guid ScreeningId { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public bool IsActive { get; init; }
    public int ImagesCount { get; init; }
    public string? ThumbnailUrl { get; init; }
    public string? LatestRiskLevel { get; init; }
}

public record ScreeningSessionDetailResponse
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string ModelVersion { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public string? RawJsonOutput { get; init; }
    public bool IsActive { get; init; }
    public List<RetinalImageDto> Images { get; init; } = new();
    public ScreeningResultDto? LatestResult { get; init; }
}

public record RetinalImageDto
{
    public Guid Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string EyeSide { get; init; } = string.Empty;
    public string? DeviceName { get; init; }
    public decimal? QualityScore { get; init; }
    public DateTime CapturedAt { get; init; }
}

public record ScreeningResultDto
{
    public Guid ScreeningResultId { get; init; }
    public string RiskLevel { get; init; } = string.Empty;
    public decimal ConfidenceScore { get; init; }
    public string? Summary { get; init; }
    public string? Findings { get; init; }
    public DateTime AssessedAt { get; init; }
}
