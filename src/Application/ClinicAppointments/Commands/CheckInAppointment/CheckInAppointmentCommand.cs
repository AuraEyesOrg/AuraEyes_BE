using Application.Common.Interfaces;

namespace Application.ClinicAppointments.Commands.CheckInAppointment;

/// <summary>
/// Command to check in a patient at the clinic.
/// </summary>
public record CheckInAppointmentCommand : ICommand<bool>
{
    /// <summary>Appointment to check in.</summary>
    public Guid AppointmentId { get; init; }
}
