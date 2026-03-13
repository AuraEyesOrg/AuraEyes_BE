using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.ConfirmReservation;

/// <summary>
/// Command to confirm a slot reservation after payment.
/// Transitions slot from Reserved to Booked and creates a ConsultationSession.
/// </summary>
public record ConfirmReservationCommand : ICommand<ConfirmReservationResult>
{
    /// <summary>The reserved appointment slot.</summary>
    public Guid AppointmentSlotId { get; init; }

    /// <summary>The patient who made the reservation.</summary>
    public Guid PatientId { get; init; }

    /// <summary>Optional AI screening ID to link to the consultation.</summary>
    public Guid? AiScreeningId { get; init; }

    /// <summary>Whether patient consents to share retinal images with doctor.</summary>
    public bool ShareRetinalImages { get; init; }

    /// <summary>Whether patient consents to share AI results with doctor.</summary>
    public bool ShareAiResults { get; init; }
}

/// <summary>
/// Result of confirming a reservation.
/// </summary>
public record ConfirmReservationResult
{
    /// <summary>The created consultation session ID.</summary>
    public Guid ConsultationSessionId { get; init; }

    /// <summary>The booked appointment slot ID.</summary>
    public Guid AppointmentSlotId { get; init; }

    /// <summary>The appointment date and time.</summary>
    public DateTime AppointmentTime { get; init; }
}
