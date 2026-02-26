using Application.Common.Interfaces;

namespace Application.ConsultationSessions.Commands.SubmitVerificationReport;

public record SubmitVerificationReportCommand : ICommand
{
    public Guid SessionId { get; init; }
    public Guid DoctorId { get; init; }
    public string DiagnosesCode { get; init; } = string.Empty;
    public string DiagnosesText { get; init; } = string.Empty;
    public string? TreatmentPlan { get; init; }
}
