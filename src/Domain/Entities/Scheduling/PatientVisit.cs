using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// PatientVisit - Tracks the lifecycle of a patient's physical visit at the clinic.
/// Can be linked to an Appointment (scheduled visit) or created standalone (walk-in visit).
/// Doctor assignment happens during the visit, not at booking time.
/// </summary>
public class PatientVisit : BaseEntity, IAggregateRoot
{
    /// <summary>FK to Appointment (nullable - supports walk-in visits without prior appointment).</summary>
    public Guid? AppointmentId { get; private set; }

    /// <summary>FK to Patient.</summary>
    public Guid PatientId { get; private set; }

    /// <summary>FK to Ophthalmologist assigned during the visit (nullable until assigned).</summary>
    public Guid? AssignedDoctorId { get; private set; }

    /// <summary>When the patient checked in at the clinic.</summary>
    public DateTime? CheckedInAt { get; private set; }

    /// <summary>When the consultation started.</summary>
    public DateTime? StartedAt { get; private set; }

    /// <summary>When the visit was completed.</summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>Current status of the visit.</summary>
    public PatientVisitStatus Status { get; private set; }

    /// <summary>Notes from the visit.</summary>
    public string? Notes { get; private set; }

    // Navigation properties
    public Appointment? Appointment { get; private set; }
    public Patient? Patient { get; private set; }
    public Ophthalmologist? AssignedDoctor { get; private set; }

    private PatientVisit() { } // EF Core

    /// <summary>
    /// Create a visit from an existing appointment (scheduled visit).
    /// </summary>
    public static PatientVisit CreateFromAppointment(Guid appointmentId, Guid patientId)
    {
        if (appointmentId == Guid.Empty)
            throw new ArgumentException("Appointment ID is required.", nameof(appointmentId));
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID is required.", nameof(patientId));

        return new PatientVisit
        {
            AppointmentId = appointmentId,
            PatientId = patientId,
            Status = PatientVisitStatus.CheckedIn,
            CheckedInAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Create a walk-in visit without an appointment.
    /// </summary>
    public static PatientVisit CreateWalkIn(Guid patientId)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient ID is required.", nameof(patientId));

        return new PatientVisit
        {
            AppointmentId = null,
            PatientId = patientId,
            Status = PatientVisitStatus.CheckedIn,
            CheckedInAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Assign a doctor to this visit. Can only be done during the visit flow.
    /// </summary>
    public void AssignDoctor(Guid doctorId)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor ID is required.", nameof(doctorId));

        AssignedDoctorId = doctorId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Start the consultation. Requires doctor to be assigned.
    /// </summary>
    public void Start()
    {
        if (Status != PatientVisitStatus.CheckedIn)
            throw new InvalidOperationException($"Cannot start visit with status {Status}. Patient must be checked in first.");

        if (!AssignedDoctorId.HasValue)
            throw new InvalidOperationException("Cannot start visit without an assigned doctor.");

        Status = PatientVisitStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Complete the visit.
    /// </summary>
    public void Complete(string? notes = null)
    {
        if (Status != PatientVisitStatus.InProgress)
            throw new InvalidOperationException($"Cannot complete visit with status {Status}. Visit must be in progress.");

        Status = PatientVisitStatus.Completed;
        Notes = notes;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
