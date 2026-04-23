using Domain.Enums;

namespace Application.Scheduling.Appointments.Common;

public class ClinicAppointmentDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string? PatientName { get; init; }
    public string? PatientAvatarUrl { get; init; }
    public Guid SlotId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string? VisitReason { get; init; }
    public AppointmentStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// True when the current patient has already submitted feedback for this
    /// </summary>
    public bool HasFeedback { get; init; }
}

public class OrganisationAvailableSlotDto
{
    public Guid SlotId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int MaxCapacity { get; init; }
    public int BookedCount { get; init; }
    public int Remaining { get; init; }
    public decimal? Cost { get; init; }
}

public class CreateClinicAppointmentResult
{
    public Guid AppointmentId { get; init; }
    public AppointmentStatus Status { get; init; }
}
