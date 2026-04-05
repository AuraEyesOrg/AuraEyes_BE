using Application.Common.Models;
using Application.PatientRoadmaps.Common;

namespace Application.Common.Interfaces;

public interface IPatientRoadmapGenerationService
{
    Task<Result<GeneratedPatientRoadmap>> GenerateFromDiagnosisAsync(
        PatientRoadmapGenerationInput input,
        CancellationToken cancellationToken = default);
}
