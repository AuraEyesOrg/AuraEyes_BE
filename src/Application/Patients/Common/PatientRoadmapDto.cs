namespace Application.Patients.Common;

public class PatientRoadmapDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid MedicalDiagnosisId { get; set; }
    public Guid? ScreeningId { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public IReadOnlyList<string> NextSteps { get; set; } = [];
    public IReadOnlyList<string> LifestyleAdvice { get; set; } = [];
    public IReadOnlyList<string> WarningSigns { get; set; } = [];
    public PatientRoadmapFollowUpDto FollowUp { get; set; } = new();
    public string Source { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class PatientRoadmapFollowUpDto
{
    public bool Needed { get; set; }
    public string Timeframe { get; set; } = string.Empty;
}
