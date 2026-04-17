using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Screenings.Queries.GetScreeningSessionDetail;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.OrganisationScreenings.Queries.GetOrgScreeningSessionDetail;

public sealed class GetOrgScreeningSessionDetailQueryHandler
    : IQueryHandler<GetOrgScreeningSessionDetailQuery, ScreeningSessionDetailDto>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IOrganisationPatientsRepository _organisationPatientsRepository;
    private readonly IIdentityService _identityService;

    public GetOrgScreeningSessionDetailQueryHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        IOrganisationPatientsRepository organisationPatientsRepository,
        IIdentityService identityService)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _organisationPatientsRepository = organisationPatientsRepository;
        _identityService = identityService;
    }

    public async Task<Result<ScreeningSessionDetailDto>> Handle(
        GetOrgScreeningSessionDetailQuery request,
        CancellationToken cancellationToken)
    {
        var hasScreeningAccess = await _organisationPatientsRepository.IsScreeningManagedByOrganisationAdminAsync(
            request.OrgAdminUserId,
            request.ScreeningId,
            cancellationToken);

        if (!hasScreeningAccess)
            return Result<ScreeningSessionDetailDto>.NotFound("Screening session not found");

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

        var patient = await _patientRepository.GetByIdAsync(session.PatientId, cancellationToken);
        if (patient is null)
            return Result<ScreeningSessionDetailDto>.NotFound("Patient not found");

        var patientName = session.PatientName;
        var patientEmail = session.PatientEmail;
        var isWalkIn = patient.IsWalkIn;

        if (patient.IsWalkIn)
        {
            if (string.IsNullOrWhiteSpace(patientName))
            {
                patientName = patient.FullName;
            }
        }
        else if (patient.UserId.HasValue)
        {
            var user = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
            if (!string.IsNullOrWhiteSpace(user?.FullName) && string.IsNullOrWhiteSpace(patientName))
            {
                patientName = user.FullName;
            }

            patientEmail = user?.Email;
        }

        if (string.IsNullOrWhiteSpace(patientName))
        {
            patientName = await _organisationPatientsRepository.GetPatientDisplayNameForOrganisationAdminAsync(
                request.OrgAdminUserId,
                session.PatientId,
                cancellationToken);
        }

        session = session with
        {
            PatientName = patientName,
            PatientEmail = patientEmail,
            IsWalkIn = isWalkIn
        };

        return Result<ScreeningSessionDetailDto>.Success(session);
    }
}
