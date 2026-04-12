using Application.Common.Interfaces;

namespace Application.ConsultationSessions.Commands.EndSession;

public record EndSessionCommand : ICommand
{
    public Guid SessionId { get; init; }
    public Guid DoctorId { get; init; }
    public string Reason { get; init; } = "DoctorFinished";
}
