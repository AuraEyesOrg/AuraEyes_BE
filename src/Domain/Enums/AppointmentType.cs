namespace Domain.Enums;

/// <summary>
/// Type of appointment - determines the booking flow and required fields.
/// </summary>
public enum AppointmentType
{
    /// <summary>
    /// Remote video consultation with an ophthalmologist.
    /// - doctor_id is required
    /// - organisation_id is optional
    /// - slot capacity = 1
    /// - Creates a ConsultationSession for the meeting
    /// </summary>
    OnlineConsultation = 1,

    /// <summary>
    /// In-person visit at a clinic/organisation.
    /// - organisation_id is required
    /// - doctor_id is NULL (assigned later at clinic)
    /// - slot capacity may be > 1
    /// - No ConsultationSession created by platform
    /// </summary>
    ClinicVisit = 2
}
