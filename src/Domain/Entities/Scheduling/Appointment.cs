using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// Unified Appointment entity for both online consultations and clinic visits.
/// This is the core booking entity in the scheduling system.
/// 
/// Flow 1 - ONLINE_CONSULTATION:
///   - DoctorId is required (set at booking time)
///   - OrganisationId is optional
///   - Slot capacity = 1
///   - A ConsultationSession is created for the meeting
///   
/// Flow 2 - CLINIC_VISIT:
///   - OrganisationId is required
///   - DoctorId is NULL
///   - Slot capacity may be > 1
///   - No ConsultationSession created by platform
/// </summary>
public class Appointment : BaseEntity, IAggregateRoot
{
    /// <summary>Type of appointment (determines booking flow).</summary>
    public AppointmentType Type { get; private set; }

    /// <summary>FK to Patient who booked the appointment.</summary>
    public Guid PatientId { get; private set; }

    /// <summary>FK to AppointmentSlot - the time slot for this appointment.</summary>
    public Guid AppointmentSlotId { get; private set; }

    /// <summary>
    /// FK to Organisation where the appointment is scheduled.
    /// Required for CLINIC_VISIT, optional for ONLINE_CONSULTATION.
    /// </summary>
    public Guid? OrganisationId { get; private set; }

    /// <summary>
    /// FK to Ophthalmologist.
    /// - ONLINE_CONSULTATION: Required, set at booking time
/// - CLINIC_VISIT: Must remain NULL
    /// </summary>
    public Guid? DoctorId { get; private set; }

    /// <summary>
    /// FK to ConsultationSession (only for ONLINE_CONSULTATION).
    /// Created when the appointment is confirmed.
    /// </summary>
    public Guid? ConsultationSessionId { get; private set; }

    /// <summary>Current status of the appointment.</summary>
    public AppointmentStatus Status { get; private set; }

    /// <summary>Reason for the visit provided by patient.</summary>
    public string? VisitReason { get; private set; }

    /// <summary>Additional notes from staff or doctor.</summary>
    public string? Notes { get; private set; }

    /// <summary>Patient consent: share retinal images with the doctor.</summary>
    public bool IsRetinalImagesShared { get; private set; }

    /// <summary>Patient consent: share AI screening result with the doctor.</summary>
    public bool IsAiResultShared { get; private set; }

    /// <summary>When the patient checked in (for clinic visits).</summary>
    public DateTime? CheckedInAt { get; private set; }

    /// <summary>When the consultation started.</summary>
    public DateTime? StartedAt { get; private set; }

    /// <summary>When the consultation was completed.</summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>Who cancelled the appointment (PatientId or StaffId).</summary>
    public Guid? CancelledBy { get; private set; }

    /// <summary>Reason for cancellation.</summary>
    public string? CancellationReason { get; private set; }

    // Navigation properties
    public Patient? Patient { get; private set; }
    public Organisation? Organisation { get; private set; }
    public Ophthalmologist? Doctor { get; private set; }
    public AppointmentSlot? AppointmentSlot { get; private set; }
    public ConsultationSession? ConsultationSession { get; private set; }

    private Appointment() { } // EF Core

    /// <summary>
    /// Factory: Create an online consultation appointment.
    /// </summary>
    public static Appointment CreateOnlineConsultation(
        Guid patientId,
        Guid appointmentSlotId,
        Guid doctorId,
        string? visitReason = null,
        Guid? organisationId = null,
        bool isRetinalImagesShared = false,
        bool isAiResultShared = false)
    {
        return new Appointment
        {
            Type = AppointmentType.OnlineConsultation,
            PatientId = patientId,
            AppointmentSlotId = appointmentSlotId,
            DoctorId = doctorId,
            OrganisationId = organisationId,
            VisitReason = visitReason,
            IsRetinalImagesShared = isRetinalImagesShared,
            IsAiResultShared = isAiResultShared,
            Status = AppointmentStatus.Pending
        };
    }

    /// <summary>
    /// Factory: Create a clinic visit appointment.
    /// </summary>
    public static Appointment CreateClinicVisit(
        Guid patientId,
        Guid appointmentSlotId,
        Guid organisationId,
        string? visitReason = null)
    {
        return new Appointment
        {
            Type = AppointmentType.ClinicVisit,
            PatientId = patientId,
            AppointmentSlotId = appointmentSlotId,
            OrganisationId = organisationId,
            DoctorId = null,
            VisitReason = visitReason,
            Status = AppointmentStatus.Pending
        };
    }

    /// <summary>
    /// Confirm the appointment.
    /// For ONLINE_CONSULTATION: typically after payment is confirmed.
    /// For CLINIC_VISIT: typically by organisation staff.
    /// </summary>
    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm appointment with status {Status}. Only pending appointments can be confirmed.");

        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Link a ConsultationSession to this appointment (for ONLINE_CONSULTATION).
    /// </summary>
    public void LinkConsultationSession(Guid consultationSessionId)
    {
        if (Type != AppointmentType.OnlineConsultation)
            throw new InvalidOperationException("ConsultationSession can only be linked to online consultations.");

        ConsultationSessionId = consultationSessionId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Check in the patient at the clinic (for CLINIC_VISIT).
    /// </summary>
    public void CheckIn()
    {
        if (Type != AppointmentType.ClinicVisit)
            throw new InvalidOperationException("Check-in is only applicable to clinic visits.");

        if (Status != AppointmentStatus.Pending && Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException($"Cannot check in appointment with status {Status}.");

        Status = AppointmentStatus.CheckedIn;
        CheckedInAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Start the consultation.
    /// </summary>
    public void Start()
    {
        if (Type == AppointmentType.ClinicVisit && Status != AppointmentStatus.CheckedIn)
            throw new InvalidOperationException("Clinic visit must be checked in before starting.");

        if (Type == AppointmentType.OnlineConsultation && Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Online consultation must be confirmed before starting.");

        if (Type == AppointmentType.OnlineConsultation && !DoctorId.HasValue)
            throw new InvalidOperationException("Cannot start appointment without assigned doctor.");

        Status = AppointmentStatus.InProgress;
        StartedAt = DateTime.UtcNow;
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
    /// Mark patient as no-show.
    /// </summary>
    public void MarkNoShow()
    {
        if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.InProgress)
            throw new InvalidOperationException($"Cannot mark {Status} appointment as no-show.");

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
    /// Update sharing consents.
    /// </summary>
    public void UpdateSharingConsents(bool shareRetinalImages, bool shareAiResult)
    {
        IsRetinalImagesShared = shareRetinalImages;
        IsAiResultShared = shareAiResult;
        UpdatedAt = DateTime.UtcNow;
    }
}
