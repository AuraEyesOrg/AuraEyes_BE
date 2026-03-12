using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// ClinicAppointment - A patient's booking at an organisation's appointment slot.
/// Unlike online consultations, doctor assignment happens at the clinic on the day of visit.
/// </summary>
public class ClinicAppointment : BaseEntity, IAggregateRoot
{
    /// <summary>FK to Patient who booked the appointment.</summary>
    public Guid PatientId { get; private set; }

    /// <summary>FK to Organisation where the appointment is scheduled.</summary>
    public Guid OrganisationId { get; private set; }

    /// <summary>FK to AppointmentSlot - the time slot for this appointment.</summary>
    public Guid AppointmentSlotId { get; private set; }

    /// <summary>FK to Ophthalmologist assigned at check-in (nullable until assigned).</summary>
    public Guid? AssignedDoctorId { get; private set; }

    /// <summary>Current status of the appointment.</summary>
    public AppointmentStatus Status { get; private set; }

    /// <summary>Reason for the visit provided by patient.</summary>
    public string? VisitReason { get; private set; }

    /// <summary>Additional notes from staff or doctor.</summary>
    public string? Notes { get; private set; }

    /// <summary>When the patient checked in at the clinic.</summary>
    public DateTime? CheckedInAt { get; private set; }

    /// <summary>When the consultation was completed.</summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>Who cancelled the appointment (PatientId or StaffId).</summary>
    public Guid? CancelledBy { get; private set; }

    /// <summary>Reason for cancellation.</summary>
    public string? CancellationReason { get; private set; }

    // Navigation properties
    public Patient? Patient { get; private set; }
    public Organisation? Organisation { get; private set; }
    public AppointmentSlot? AppointmentSlot { get; private set; }
    public Ophthalmologist? AssignedDoctor { get; private set; }

    private ClinicAppointment() { } // EF Core

    public ClinicAppointment(
        Guid patientId,
        Guid organisationId,
        Guid appointmentSlotId,
        string? visitReason = null)
    {
        PatientId = patientId;
        OrganisationId = organisationId;
        AppointmentSlotId = appointmentSlotId;
        VisitReason = visitReason;
        Status = AppointmentStatus.Pending;
    }

    /// <summary>
    /// Confirm the appointment (by organisation staff).
    /// </summary>
    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm appointment with status {Status}. Only pending appointments can be confirmed.");

        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Check in the patient at the clinic.
    /// </summary>
    public void CheckIn()
    {
        if (Status != AppointmentStatus.Pending && Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException($"Cannot check in appointment with status {Status}.");

        Status = AppointmentStatus.CheckedIn;
        CheckedInAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Assign a doctor to this appointment (at clinic).
    /// </summary>
    public void AssignDoctor(Guid doctorId)
    {
        if (Status == AppointmentStatus.Cancelled || Status == AppointmentStatus.NoShow)
            throw new InvalidOperationException($"Cannot assign doctor to {Status} appointment.");

        AssignedDoctorId = doctorId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Start the consultation.
    /// </summary>
    public void Start()
    {
        if (Status != AppointmentStatus.CheckedIn)
            throw new InvalidOperationException($"Cannot start appointment with status {Status}. Patient must be checked in first.");

        if (!AssignedDoctorId.HasValue)
            throw new InvalidOperationException("Cannot start appointment without assigned doctor.");

        Status = AppointmentStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Complete the consultation.
    /// </summary>
    public void Complete(string? notes = null)
    {
        if (Status != AppointmentStatus.InProgress)
            throw new InvalidOperationException($"Cannot complete appointment with status {Status}. Appointment must be in progress.");

        Status = AppointmentStatus.Completed;
        Notes = notes;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancel the appointment.
    /// </summary>
    public void Cancel(Guid cancelledBy, string? reason = null)
    {
        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed appointment.");

        if (Status == AppointmentStatus.InProgress)
            throw new InvalidOperationException("Cannot cancel an appointment that is in progress.");

        Status = AppointmentStatus.Cancelled;
        CancelledBy = cancelledBy;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark the patient as no-show.
    /// </summary>
    public void MarkNoShow()
    {
        if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.InProgress)
            throw new InvalidOperationException($"Cannot mark {Status} appointment as no-show.");

        Status = AppointmentStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update the visit reason.
    /// </summary>
    public void UpdateVisitReason(string visitReason)
    {
        VisitReason = visitReason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Add or update notes.
    /// </summary>
    public void UpdateNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Check if the appointment can be cancelled.
    /// </summary>
    public bool CanBeCancelled()
    {
        return Status != AppointmentStatus.Completed &&
               Status != AppointmentStatus.InProgress &&
               Status != AppointmentStatus.Cancelled;
    }
}
