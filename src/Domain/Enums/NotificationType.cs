namespace Domain.Enums;

/// <summary>
/// Defines the types of notifications in the system
/// Used for categorizing and routing notifications to appropriate handlers
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// FR-45: Patient receives notification when AI screening is completed
    /// Payload: { "ScreeningId": "guid", "ResultStatus": "string" }
    /// </summary>
    AiScreeningCompleted = 0,

    /// <summary>
    /// FR-46: Patient receives notification when doctor accepts consultation request
    /// Payload: { "ConsultationId": "guid", "DoctorId": "guid" }
    /// </summary>
    ConsultationAccepted = 1,

    /// <summary>
    /// FR-46: Patient receives notification when consultation result is provided
    /// Payload: { "ConsultationId": "guid", "DoctorId": "guid" }
    /// </summary>
    ConsultationResultProvided = 2,

    /// <summary>
    /// FR-47: Doctor receives notification for new consultation request
    /// Payload: { "ConsultationId": "guid", "PatientId": "guid" }
    /// </summary>
    NewConsultationRequest = 3,

    /// <summary>
    /// FR-47: Doctor receives notification for new patient message
    /// Payload: { "ConsultationId": "guid", "PatientId": "guid" }
    /// </summary>
    NewPatientMessage = 4,

    /// <summary>
    /// FR-48: Admin receives notification for new appointment booking
    /// Payload: { "AppointmentId": "guid", "Action": "string" }
    /// </summary>
    NewAppointmentBooked = 5,

    /// <summary>
    /// FR-48: Admin receives notification when schedule changes
    /// Payload: { "AppointmentId": "guid", "Action": "string" }
    /// </summary>
    ScheduleChanged = 6,

    /// <summary>
    /// FR-49: User receives notification when wallet deposit is successful
    /// Payload: { "TransactionId": "guid", "Amount": "decimal", "Action": "string" }
    /// </summary>
    WalletDepositSuccess = 7,

    /// <summary>
    /// FR-49: User receives notification when wallet payment is processed
    /// Payload: { "TransactionId": "guid", "Amount": "decimal", "Action": "string" }
    /// </summary>
    WalletPaymentProcessed = 8,

    /// <summary>
    /// Internal platform alert for admin-facing operational actions.
    /// </summary>
    SystemAlert = 9
}
