using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Patients.Common;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Patients.Queries.GetPatientRoadmaps;

public class GetPatientRoadmapsQueryHandler : IQueryHandler<GetPatientRoadmapsQuery, IReadOnlyList<PatientRoadmapDto>>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<PatientRoadmap> _roadmapRepository;

    public GetPatientRoadmapsQueryHandler(
        IRepository<Patient> patientRepository,
        IRepository<PatientRoadmap> roadmapRepository)
    {
        _patientRepository = patientRepository;
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<IReadOnlyList<PatientRoadmapDto>>> Handle(
        GetPatientRoadmapsQuery request,
        CancellationToken cancellationToken)
    {
        var patient = await _patientRepository
            .Query()
            .Where(p => p.UserId == request.UserId)
            .Select(p => new { p.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (patient is null)
            return Result<IReadOnlyList<PatientRoadmapDto>>.NotFound("Patient profile not found.");

        var roadmaps = await _roadmapRepository
            .Query()
            .Where(r => r.PatientId == patient.Id)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync(cancellationToken);

        var data = roadmaps
            .Select(Map)
            .ToList();

        return Result<IReadOnlyList<PatientRoadmapDto>>.Success(data);
    }

    private static PatientRoadmapDto Map(PatientRoadmap roadmap)
    {
        return new PatientRoadmapDto
        {
            Id = roadmap.Id,
            PatientId = roadmap.PatientId,
            MedicalDiagnosisId = roadmap.MedicalDiagnosisId,
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
