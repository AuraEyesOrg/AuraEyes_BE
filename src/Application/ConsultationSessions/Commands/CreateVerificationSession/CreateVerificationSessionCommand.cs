using Application.Common.Interfaces;

namespace Application.ConsultationSessions.Commands.CreateVerificationSession;

public record CreateVerificationSessionCommand : ICommand<Guid>
{
    public Guid PatientId { get; init; }
    public Guid AiScreeningId { get; init; }
    public decimal Price { get; init; }
    public Guid? OphthalmologistId { get; init; }
}
