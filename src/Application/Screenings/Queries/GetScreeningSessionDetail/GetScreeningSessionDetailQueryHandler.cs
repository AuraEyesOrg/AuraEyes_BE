using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Screenings.Queries.GetScreeningSessionDetail;

public class GetScreeningSessionDetailQueryHandler
    : IQueryHandler<GetScreeningSessionDetailQuery, ScreeningSessionDetailDto>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetScreeningSessionDetailQueryHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        ICurrentUserService currentUserService)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ScreeningSessionDetailDto>> Handle(
        GetScreeningSessionDetailQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Result<ScreeningSessionDetailDto>.Unauthorized("User not authenticated");

        var patients = await _patientRepository.FindAsync(
            p => p.UserId == _currentUserService.UserId.Value,
            cancellationToken);

        var patient = patients.FirstOrDefault();
        if (patient is null)
            return Result<ScreeningSessionDetailDto>.NotFound("Patient profile not found");

        var session = await _screeningRepository
            .Query()
            .Where(s => s.Id == request.ScreeningId && s.PatientId == patient.Id && !s.IsDeleted)
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

        if (session is null)
            return Result<ScreeningSessionDetailDto>.NotFound("Screening session not found");

        return Result<ScreeningSessionDetailDto>.Success(session);
    }
}
