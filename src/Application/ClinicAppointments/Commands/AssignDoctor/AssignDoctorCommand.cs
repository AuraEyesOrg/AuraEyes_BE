using Application.Common.Interfaces;

namespace Application.ClinicAppointments.Commands.AssignDoctor;

/// <summary>
/// Command to assign a doctor to a clinic appointment.
/// Typically done by staff after patient check-in.
/// </summary>
public record AssignDoctorCommand : ICommand<bool>
{
    /// <summary>Appointment to assign doctor to.</summary>
    public Guid AppointmentId { get; init; }

    /// <summary>Doctor (Ophthalmologist) to assign.</summary>
    public Guid DoctorId { get; init; }
}
