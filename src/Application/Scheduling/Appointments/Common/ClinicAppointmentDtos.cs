using Domain.Enums;

namespace Application.Scheduling.Appointments.Common;

public class ClinicAppointmentDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public Guid OrganisationId { get; init; }
    public string? OrganisationName { get; init; }
    public Guid SlotId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string? VisitReason { get; init; }
    public AppointmentStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
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
