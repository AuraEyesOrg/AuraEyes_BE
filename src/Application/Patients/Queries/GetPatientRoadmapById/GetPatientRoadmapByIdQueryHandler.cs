using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Patients.Common;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Patients.Queries.GetPatientRoadmapById;

public class GetPatientRoadmapByIdQueryHandler : IQueryHandler<GetPatientRoadmapByIdQuery, PatientRoadmapDto>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<PatientRoadmap> _roadmapRepository;
    private readonly IRepository<MedicalDiagnosis> _medicalDiagnosisRepository;

    public GetPatientRoadmapByIdQueryHandler(
        IRepository<Patient> patientRepository,
        IRepository<PatientRoadmap> roadmapRepository,
        IRepository<MedicalDiagnosis> medicalDiagnosisRepository)
    {
        _patientRepository = patientRepository;
        _roadmapRepository = roadmapRepository;
        _medicalDiagnosisRepository = medicalDiagnosisRepository;
    }

    public async Task<Result<PatientRoadmapDto>> Handle(
        GetPatientRoadmapByIdQuery request,
        CancellationToken cancellationToken)
    {
        var patient = await _patientRepository
            .Query()
            .Where(p => p.UserId == request.UserId)
            .Select(p => new { p.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result<PatientRoadmapDto>.NotFound("Patient profile not found.");

        var roadmap = await _roadmapRepository
            .Query()
            .FirstOrDefaultAsync(
                r => r.Id == request.RoadmapId && r.PatientId == patient.Id,
                cancellationToken);

        if (roadmap is null)
            return Result<PatientRoadmapDto>.NotFound("Roadmap not found.");

        var screeningId = await _medicalDiagnosisRepository
            .Query()
            .Where(d => d.Id == roadmap.MedicalDiagnosisId)
            .Select(d => (Guid?)d.AiScreeningId)
            .FirstOrDefaultAsync(cancellationToken);

        var data = new PatientRoadmapDto
        {
            Id = roadmap.Id,
            PatientId = roadmap.PatientId,
            MedicalDiagnosisId = roadmap.MedicalDiagnosisId,
            ScreeningId = screeningId,
            RiskLevel = roadmap.RiskLevel,
            Summary = roadmap.Summary,
            NextSteps = DeserializeArray(roadmap.NextStepsJson),
            LifestyleAdvice = DeserializeArray(roadmap.LifestyleAdviceJson),
            WarningSigns = DeserializeArray(roadmap.WarningSignsJson),
            FollowUp = new PatientRoadmapFollowUpDto
            {
                Needed = roadmap.FollowUpNeeded,
                Timeframe = roadmap.FollowUpTimeframe
            },
            Source = roadmap.Source,
            GeneratedAt = roadmap.GeneratedAt
        };

        return Result<PatientRoadmapDto>.Success(data);
    }

    private static IReadOnlyList<string> DeserializeArray(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
