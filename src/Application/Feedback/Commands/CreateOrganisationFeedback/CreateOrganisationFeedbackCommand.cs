using Application.Common.Interfaces;

namespace Application.Feedback.Commands.CreateOrganisationFeedback;

public record CreateOrganisationFeedbackCommand : ICommand<Guid>
{
    public Guid OrganisationId { get; init; }
    public Guid AppointmentId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
}
