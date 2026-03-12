using Application.Common.Interfaces;

namespace Application.ClinicAppointments.Commands.MarkNoShow;

/// <summary>
/// Command to mark a patient as no-show for an appointment.
/// </summary>
public record MarkNoShowCommand : ICommand<bool>
{
    /// <summary>Appointment to mark as no-show.</summary>
    public Guid AppointmentId { get; init; }
}
