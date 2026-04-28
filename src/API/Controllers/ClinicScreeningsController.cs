using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Screenings.Queries.GetClinicScreeningHistory;
using Application.Screenings.Queries.GetScreeningSessionDetail;
using Application.Screenings.Commands.CreateAiScreeningSession;
using Application.Screenings.Commands.CreateClinicScreeningSession;
using Application.Screenings.Queries.ExportPatientScreeningReportPdf;
using Domain.Entities.Screening;
using Domain.Repositories;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity.Authorization;

namespace API.Controllers;

[Route("api/clinic-screenings")]
public class ClinicScreeningsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Domain.Entities.Users.Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public ClinicScreeningsController(
        IMediator mediator,
        IRepository<AiScreening> screeningRepository,
        IRepository<Domain.Entities.Users.Patient> patientRepository,
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    [HttpGet("history")]
    [AuthorizePermission(Permissions.ScreeningRead)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ClinicScreeningHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory([FromQuery] int take = 100)
    {
        var result = await _mediator.Send(new GetClinicScreeningHistoryQuery { Take = take });
        return HandleResult(result);
    }

    [HttpGet("{screeningId:guid}")]
    [AuthorizePermission(Permissions.ScreeningRead)]
    [ProducesResponseType(typeof(ApiResponse<ScreeningSessionDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid screeningId, CancellationToken cancellationToken)
    {
        // For staff, we bypass the "current patient" check by fetching manually here 
        // because the GetScreeningSessionDetailQueryHandler is strictly patient-bound.
        var session = await _screeningRepository
            .Query()
            .Where(s => s.Id == screeningId && !s.IsDeleted)
            .Select(s => new ScreeningSessionDetailDto
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
                    .Select(i => new RetinalImageItemDto
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
                    .Select(r => new ScreeningResultItemDto
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

        if (session == null)
            return NotFound(ApiResponseFactory.NotFound("Screening session not found"));

        // Enrich with patient info
        var patient = await _patientRepository.GetByIdAsync(session.PatientId, cancellationToken);
        if (patient != null)
        {
            string? patientName = patient.FullName;
            string? patientEmail = null;

            if (patient.UserId.HasValue)
            {
                var identityUser = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
                patientName = identityUser?.FullName ?? patientName;
                patientEmail = identityUser?.Email;
            }

            session = session with
            {
                PatientName = patientName,
                PatientEmail = patientEmail,
                IsWalkIn = patient.IsWalkIn
            };
        }

        return Ok(ApiResponseFactory.Success(session));
    }

    [HttpPost("create-session")]
    [AuthorizePermission(Permissions.ScreeningCreate)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSession([FromBody] CreateClinicScreeningSessionCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("{screeningId:guid}/report-pdf")]
    [AuthorizePermission(Permissions.ScreeningRead)]
    public async Task<IActionResult> GetReportPdf(Guid screeningId)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized(ApiResponseFactory.Unauthorized());

        var query = new ExportPatientScreeningReportPdfQuery(
            _currentUserService.UserId.Value,
            screeningId)
        {
            BypassAccessCheck = true // We are authorizing via Permission, so bypass the patient-owner check
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return HandleResult(result);

        return File(result.Data!.Content, result.Data.ContentType, result.Data.FileName);
    }
}
