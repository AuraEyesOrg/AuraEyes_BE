using Application.Common.Interfaces;

namespace Application.ConsultationSessions.Commands.CancelSession;

public record CancelSessionCommand : ICommand
{
    public Guid SessionId { get; init; }
    public Guid CancelledByUserId { get; init; }
    public string? Reason { get; init; }
}
