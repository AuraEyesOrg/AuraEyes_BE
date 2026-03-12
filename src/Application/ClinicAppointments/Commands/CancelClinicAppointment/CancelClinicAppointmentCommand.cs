using Application.Common.Interfaces;

namespace Application.ClinicAppointments.Commands.CancelClinicAppointment;

/// <summary>
/// Command to cancel a clinic appointment.
/// Decrements the slot's booked count and marks appointment as cancelled.
/// </summary>
public record CancelClinicAppointmentCommand : ICommand<bool>
{
    /// <summary>Appointment to cancel.</summary>
    public Guid AppointmentId { get; init; }

    /// <summary>User cancelling the appointment (patient or staff).</summary>
    public Guid CancelledBy { get; init; }

    /// <summary>Reason for cancellation.</summary>
    public string? Reason { get; init; }
}
