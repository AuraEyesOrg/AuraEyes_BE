using Application.Common.Interfaces;

namespace Application.ConsultationSessions.Commands.SubmitVerificationReport;

public record SubmitVerificationReportCommand : ICommand
{
    public Guid SessionId { get; init; }
    public Guid DoctorId { get; init; }

    // New contract fields.
    public string? DiagnosisCode { get; init; }
    public string? CodingSystem { get; init; }
    public string? ClinicalFindings { get; init; }
    public string? SeverityLevel { get; init; }
    public decimal? ConfidenceLevel { get; init; }
    public string? TreatmentPlan { get; init; }
    public string? Recommendations { get; init; }
    public string? LifestyleAdvice { get; init; }
    public bool IsUrgent { get; init; }
    public string? Status { get; init; }
    public DateTime? FollowUpDate { get; init; }
    public bool IsReferralNeeded { get; init; }
    public DateTime? FinalizedAt { get; init; }

    // Backward-compatible legacy aliases still accepted by existing callers.
    public string? DiagnosesCode { get; init; }
    public string? DiagnosesText { get; init; }
}
