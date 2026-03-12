using Application.Common.Interfaces;

namespace Application.ClinicAppointments.Commands.StartAppointment;

/// <summary>
/// Command to start a clinic appointment (begin consultation).
/// </summary>
public record StartAppointmentCommand : ICommand<bool>
{
    /// <summary>Appointment to start.</summary>
    public Guid AppointmentId { get; init; }
}
