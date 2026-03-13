using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.BookAppointmentSlot;

/// <summary>
/// Command to book an appointment slot for a patient.
/// Validates capacity before transitioning status to Booked.
/// </summary>
public record BookAppointmentSlotCommand : ICommand
{
    /// <summary>The appointment slot to book.</summary>
    public Guid AppointmentSlotId { get; init; }

    /// <summary>Patient confirming the booking (used for authorization check).</summary>
    public Guid PatientId { get; init; }
}
