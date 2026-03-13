using Application.Common.Interfaces;

namespace Application.Feedback.Commands.CreateOphthalmologistFeedback;

public record CreateOphthalmologistFeedbackCommand : ICommand<Guid>
{
    public Guid OphthalmologistId { get; init; }
    public Guid ConsultationSessionId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}
