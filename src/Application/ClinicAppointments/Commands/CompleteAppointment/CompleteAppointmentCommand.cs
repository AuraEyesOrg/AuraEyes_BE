using Application.Common.Interfaces;

namespace Application.ClinicAppointments.Commands.CompleteAppointment;

/// <summary>
/// Command to complete a clinic appointment.
/// </summary>
public record CompleteAppointmentCommand : ICommand<bool>
{
    /// <summary>Appointment to complete.</summary>
    public Guid AppointmentId { get; init; }

    /// <summary>Optional notes from the consultation.</summary>
    public string? Notes { get; init; }
}
