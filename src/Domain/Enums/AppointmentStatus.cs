namespace Domain.Enums;

/// <summary>
/// Status for clinic appointments (organisation-based bookings).
/// </summary>
public enum AppointmentStatus
{
    /// <summary>Appointment created, waiting for confirmation.</summary>
    Pending = 1,

    /// <summary>Appointment confirmed by organisation.</summary>
    Confirmed = 2,

    /// <summary>Patient has checked in at the clinic.</summary>
    CheckedIn = 3,

    /// <summary>Consultation is in progress.</summary>
    InProgress = 4,

    /// <summary>Consultation completed successfully.</summary>
    Completed = 5,

    /// <summary>Appointment cancelled by patient or organisation.</summary>
    Cancelled = 6,

    /// <summary>Patient did not show up for the appointment.</summary>
    NoShow = 7
}
