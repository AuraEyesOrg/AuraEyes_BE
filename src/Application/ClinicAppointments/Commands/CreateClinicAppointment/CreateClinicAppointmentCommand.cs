using Application.Common.Interfaces;

namespace Application.ClinicAppointments.Commands.CreateClinicAppointment;

/// <summary>
/// Command to create a clinic appointment (patient booking at organisation).
/// Uses atomic capacity check to prevent race conditions.
/// </summary>
public record CreateClinicAppointmentCommand : ICommand<Guid>
{
    /// <summary>Patient making the booking.</summary>
    public Guid PatientId { get; init; }

    /// <summary>Organisation being booked.</summary>
    public Guid OrganisationId { get; init; }

    /// <summary>Appointment slot to book.</summary>
    public Guid SlotId { get; init; }

    /// <summary>Reason for the visit.</summary>
    public string? VisitReason { get; init; }
}
