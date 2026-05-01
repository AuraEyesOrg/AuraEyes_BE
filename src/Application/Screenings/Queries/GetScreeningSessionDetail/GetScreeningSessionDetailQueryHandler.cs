using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.MedicalRecords;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Screenings.Queries.GetScreeningSessionDetail;

public class GetScreeningSessionDetailQueryHandler
    : IQueryHandler<GetScreeningSessionDetailQuery, ScreeningSessionDetailDto>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetScreeningSessionDetailQueryHandler(
        IRepository<AiScreening> _screeningRepository,
        IRepository<Patient> _patientRepository,
        IMedicalRecordRepository _medicalRecordRepository,
        ICurrentUserService _currentUserService,
        IIdentityService _identityService)
    {
        this._screeningRepository = _screeningRepository;
        this._patientRepository = _patientRepository;
        this._medicalRecordRepository = _medicalRecordRepository;
        this._currentUserService = _currentUserService;
        this._identityService = _identityService;
    }

    public async Task<Result<ScreeningSessionDetailDto>> Handle(
        GetScreeningSessionDetailQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
            return Result<ScreeningSessionDetailDto>.Unauthorized("User not authenticated");

        var screeningQuery = _screeningRepository
            .Query()
            .Where(s => s.Id == request.ScreeningId && !s.IsDeleted);

        if (!request.BypassPatientCheck)
        {
            var patients = await _patientRepository.FindAsync(
                p => p.UserId == _currentUserService.UserId.Value,
                cancellationToken);

            var patient = patients.FirstOrDefault();
            if (patient is null)
                return Result<ScreeningSessionDetailDto>.NotFound("Patient profile not found");

            screeningQuery = screeningQuery.Where(s => s.PatientId == patient.Id);
        }

        var session = await screeningQuery
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

        // Populate patient info if bypass check is used (for clinic staff)
        if (request.BypassPatientCheck)
        {
            var patient = await _patientRepository.GetByIdAsync(session.PatientId, cancellationToken);
            if (patient != null)
            {
                string? patientName = patient.FullName;
                string? patientEmail = null;

                if (!patient.IsWalkIn && patient.UserId.HasValue)
                {
                    var user = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
                    if (user != null)
                    {
                        patientName = user.FullName ?? patientName;
                        patientEmail = user.Email;
                    }
                }

                session = session with
                {
                    PatientName = patientName,
                    PatientEmail = patientEmail,
                    IsWalkIn = patient.IsWalkIn
                };

                // Link to most recent medical record
                var medicalRecords = await _medicalRecordRepository.FindAsync(
                    mr => mr.PatientId == patient.Id, cancellationToken);
                
                session.MedicalRecordId = medicalRecords
                    .OrderByDescending(mr => mr.CreatedAt)
                    .FirstOrDefault()?.Id;
            }
        }

        return Result<ScreeningSessionDetailDto>.Success(session);
    }
}
