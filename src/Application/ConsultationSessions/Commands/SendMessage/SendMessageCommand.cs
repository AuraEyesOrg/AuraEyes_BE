using Application.Common.Interfaces;

namespace Application.ConsultationSessions.Commands.SendMessage;

public record SendMessageCommand : ICommand
{
    public Guid SessionId { get; init; }
    public string Message { get; init; } = string.Empty;
}
