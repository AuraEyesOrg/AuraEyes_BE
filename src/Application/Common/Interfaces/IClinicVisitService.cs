using Application.Common.Models;
using Domain.Entities.Financial;
using Domain.Entities.Scheduling;

namespace Application.Common.Interfaces;

/// <summary>
/// Service to handle post-payment clinic visit transitions.
/// </summary>
public interface IClinicVisitService
{
    /// <summary>
    /// Checks if an order payment completes a clinic visit and handles state transitions + chat creation.
    /// </summary>
    Task ProcessPaymentCompletionAsync(Order order, string paymentMethod, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an appointment by its ID.
    /// </summary>
    Task<Appointment?> GetAppointmentByIdAsync(Guid appointmentId, CancellationToken cancellationToken);

    /// <summary>
    /// Cancels an appointment and releases its slot.
    /// </summary>
    Task CancelAppointmentAsync(Guid appointmentId, string? reason, CancellationToken cancellationToken);
}
