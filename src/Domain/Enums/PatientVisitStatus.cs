namespace Domain.Enums;

/// <summary>
/// Status for patient visit lifecycle at the clinic.
/// </summary>
public enum PatientVisitStatus
{
    /// <summary>Patient has checked in at the clinic.</summary>
    CheckedIn = 1,

    /// <summary>Consultation with doctor is in progress.</summary>
    InProgress = 2,

    /// <summary>Visit completed successfully (after payment).</summary>
    Completed = 3,

    /// <summary>Doctor has completed consultation, patient is waiting at cashier to pay.</summary>
    WaitingForPayment = 4
}
