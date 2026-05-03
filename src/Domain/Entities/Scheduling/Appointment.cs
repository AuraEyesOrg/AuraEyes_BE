using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// Appointment - A booking-only entity linking a patient to an appointment slot.
/// Simplified to clinic-centric model: no doctor/organisation/consultation linkage.
/// Lifecycle operations (check-in, start, complete) are handled by PatientVisit.
/// </summary>
public class Appointment : BaseEntity, IAggregateRoot
{
    /// <summary>FK to Patient who booked the appointment.</summary>
    public Guid PatientId { get; private set; }

    /// <summary>FK to AppointmentSlot - the time slot for this appointment.</summary>
    public Guid AppointmentSlotId { get; private set; }

    /// <summary>Current status of the appointment.</summary>
    public AppointmentStatus Status { get; private set; }

    /// <summary>Reason for the visit provided by patient.</summary>
    public string? VisitReason { get; private set; }

    /// <summary>Who cancelled the appointment (PatientId or StaffId).</summary>
    public Guid? CancelledBy { get; private set; }

    /// <summary>Reason for cancellation.</summary>
    public string? CancellationReason { get; private set; }
    
    /// <summary>Optional preferred doctor for this appointment.</summary>
    public Guid? RequestedDoctorId { get; private set; }

    /// <summary>Determines if price is base or doctor-specific.</summary>
    public PricingType PricingType { get; private set; }

    /// <summary>Snapshot of the price at the time of booking.</summary>
    public decimal Price { get; private set; }

    /// <summary>Bank number for refund if cancelled.</summary>
    public string? RefundBankNumber { get; private set; }

    /// <summary>Account name for refund if cancelled.</summary>
    public string? RefundAccountName { get; private set; }

    /// <summary>Bank name for refund if cancelled.</summary>
    public string? RefundBankName { get; private set; }

    // Navigation properties
    public Patient? Patient { get; private set; }
    public AppointmentSlot? AppointmentSlot { get; private set; }
    public Ophthalmologist? RequestedDoctor { get; private set; }

    private Appointment() { } // EF Core

    /// <summary>
    /// Create a new appointment (booking).
    /// </summary>
    public Appointment(
        Guid patientId,
        Guid appointmentSlotId,
        decimal price,
        PricingType pricingType = PricingType.AutoAssign,
        Guid? requestedDoctorId = null,
        string? visitReason = null)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        PatientId = patientId;
        AppointmentSlotId = appointmentSlotId;
        Price = price;
        PricingType = pricingType;
        RequestedDoctorId = requestedDoctorId;
        VisitReason = visitReason;
        Status = AppointmentStatus.Pending;
    }

    /// <summary>
    /// Confirm the appointment (by clinic staff).
    /// </summary>
    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm appointment with status {Status}. Only pending appointments can be confirmed.");

        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark patient as checked in.
    /// </summary>
    public void CheckIn()
    {
        if (Status is not (AppointmentStatus.Pending or AppointmentStatus.Confirmed))
            throw new InvalidOperationException($"Cannot check in appointment with status {Status}.");

        Status = AppointmentStatus.CheckedIn;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Start the consultation.
    /// </summary>
    public void Start()
    {
        if (Status != AppointmentStatus.CheckedIn)
            throw new InvalidOperationException($"Cannot start appointment with status {Status}. Patient must be checked in first.");

        Status = AppointmentStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Complete the appointment.
    /// </summary>
    public void Complete()
    {
        if (Status != AppointmentStatus.InProgress)
            throw new InvalidOperationException($"Cannot complete appointment with status {Status}. Visit must be in progress.");

        Status = AppointmentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancel the appointment.
    /// </summary>
    public void Cancel(Guid cancelledBy, string? reason = null)
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Appointment is already cancelled.");

        Status = AppointmentStatus.Cancelled;
        CancelledBy = cancelledBy;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Request a cancellation (by patient).
    /// </summary>
    public void RequestCancellation(string? bankNumber, string? accountName, string? bankName, string? reason = null)
    {
        if (Status is AppointmentStatus.Cancelled or AppointmentStatus.CancellationRequested)
            throw new InvalidOperationException("Appointment is already cancelled or cancellation is already requested.");

        if (Status is AppointmentStatus.Completed or AppointmentStatus.InProgress or AppointmentStatus.CheckedIn)
            throw new InvalidOperationException($"Cannot cancel an appointment that is {Status}.");

        Status = AppointmentStatus.CancellationRequested;
        RefundBankNumber = bankNumber;
        RefundAccountName = accountName;
        RefundBankName = bankName;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark patient as no-show.
    /// </summary>
    public void MarkNoShow()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot mark a cancelled appointment as no-show.");

        Status = AppointmentStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update visit reason.
    /// </summary>
    public void UpdateVisitReason(string? visitReason)
    {
        VisitReason = visitReason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark appointment as cancelled due to late arrival and rebooking.
    /// </summary>
    public void MarkLateAndRelease(Guid cancelledBy)
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Appointment is already cancelled.");

        Status = AppointmentStatus.Cancelled;
        CancelledBy = cancelledBy;
        CancellationReason = "Late arrival — rebooked";
        UpdatedAt = DateTime.UtcNow;
    }
}
