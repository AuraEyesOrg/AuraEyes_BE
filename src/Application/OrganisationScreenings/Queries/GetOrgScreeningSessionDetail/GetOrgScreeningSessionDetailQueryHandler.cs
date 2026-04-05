using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Screenings.Queries.GetScreeningSessionDetail;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.OrganisationScreenings.Queries.GetOrgScreeningSessionDetail;

public sealed class GetOrgScreeningSessionDetailQueryHandler
    : IQueryHandler<GetOrgScreeningSessionDetailQuery, ScreeningSessionDetailDto>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IOrganisationPatientsRepository _organisationPatientsRepository;

    public GetOrgScreeningSessionDetailQueryHandler(
        IRepository<AiScreening> screeningRepository,
        IOrganisationPatientsRepository organisationPatientsRepository)
    {
        _screeningRepository = screeningRepository;
        _organisationPatientsRepository = organisationPatientsRepository;
    }

    public async Task<Result<ScreeningSessionDetailDto>> Handle(
        GetOrgScreeningSessionDetailQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _screeningRepository
            .Query()
            .Where(s => s.Id == request.ScreeningId && !s.IsDeleted)
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

        var hasAccess = await _organisationPatientsRepository.IsPatientManagedByOrganisationAdminAsync(
            request.OrgAdminUserId,
            session.PatientId,
            cancellationToken);

        if (!hasAccess)
            return Result<ScreeningSessionDetailDto>.NotFound("Screening session not found");

        return Result<ScreeningSessionDetailDto>.Success(session);
    }
}
