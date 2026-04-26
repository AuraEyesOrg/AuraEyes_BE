using Application.Common.Interfaces;

namespace Application.Feedback.Commands.CreateClinicFeedback;

/// <summary>
/// CreateClinicFeedbackCommand - Submit feedback for a clinic visit.
/// Clinic-centric feedback model.
/// </summary>
public record CreateClinicFeedbackCommand : ICommand<Guid>
{
    public Guid AppointmentId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
    
    /// <summary>Optional doctor being specifically reviewed.</summary>
    public Guid? DoctorId { get; init; }

    /// <summary>Optional staff member being specifically reviewed.</summary>
    public Guid? StaffId { get; init; }
}
