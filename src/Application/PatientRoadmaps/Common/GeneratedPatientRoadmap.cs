namespace Application.PatientRoadmaps.Common;

public class GeneratedPatientRoadmap
{
    public string RiskLevel { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public IReadOnlyList<string> NextSteps { get; init; } = [];
    public IReadOnlyList<string> LifestyleAdvice { get; init; } = [];
    public bool FollowUpNeeded { get; init; }
    public string FollowUpTimeframe { get; init; } = string.Empty;
    public IReadOnlyList<string> WarningSigns { get; init; } = [];
    public string RawAiResponse { get; init; } = string.Empty;
}
