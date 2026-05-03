using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Screenings.Queries.GetRecentScreeningSessions;

public class GetRecentScreeningSessionsQueryHandler
    : IQueryHandler<GetRecentScreeningSessionsQuery, IReadOnlyList<ScreeningSessionSummaryDto>>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetRecentScreeningSessionsQueryHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        ICurrentUserService currentUserService)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<ScreeningSessionSummaryDto>>> Handle(
        GetRecentScreeningSessionsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Result<IReadOnlyList<ScreeningSessionSummaryDto>>.Unauthorized("User not authenticated");

        var patients = await _patientRepository.FindAsNoTrackingAsync(
            p => p.UserId == _currentUserService.UserId.Value,
            cancellationToken);

        var patient = patients.FirstOrDefault();
        if (patient is null)
            return Result<IReadOnlyList<ScreeningSessionSummaryDto>>.NotFound("Patient profile not found");

        var cappedLimit = Math.Clamp(request.Limit, 1, 50);

        var sessions = await _screeningRepository
            .Query().AsNoTracking()
            .Where(s => s.PatientId == patient.Id && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .Take(cappedLimit)
            .Select(s => new ScreeningSessionSummaryDto
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

        return Result<IReadOnlyList<ScreeningSessionSummaryDto>>.Success(sessions);
    }
}


