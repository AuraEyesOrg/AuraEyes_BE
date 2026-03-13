using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Feedback.Commands.CreateWebsiteFeedback;

public record CreateWebsiteFeedbackCommand : ICommand<Guid>
{
    public int Rating { get; init; }
    public WebsiteFeedbackCategory Category { get; init; }
    public string? Comment { get; init; }
}
