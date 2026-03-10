using Application.Common.Interfaces;

namespace Application.ConsultationSessions.Commands.CreateVideoCallSession;

public record CreateVideoCallSessionCommand : ICommand<Guid>
{
    public Guid PatientId { get; init; }
    public decimal PlatformFee { get; init; }
    public DateTime AppointmentTime { get; init; }
    public Guid? OphthalmologistId { get; init; }
}
