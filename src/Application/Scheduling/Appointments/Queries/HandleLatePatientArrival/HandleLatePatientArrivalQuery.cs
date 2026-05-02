using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Queries.HandleLatePatientArrival;

public record HandleLatePatientArrivalQuery(Guid AppointmentId) : IQuery<LateArrivalCheckResult>;

public class LateArrivalCheckResult
{
    public bool IsLate { get; init; }
    public int LateMinutes { get; init; }
    public int ThresholdMinutes { get; init; }
    public Guid CurrentSlotId { get; init; }
    public DateOnly SlotDate { get; init; }
    public TimeOnly SlotStartTime { get; init; }
    public TimeOnly SlotEndTime { get; init; }
    public int SlotDurationMinutes { get; init; }
    public IReadOnlyList<AvailableSlotOption> AvailableSlots { get; init; } = new List<AvailableSlotOption>();
    public AdHocDefaults AdHocDefaults { get; init; } = new();
}

public class AvailableSlotOption
{
    public Guid SlotId { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int RemainingCapacity { get; init; }
    public decimal? Cost { get; init; }
}

public class AdHocDefaults
{
    public TimeOnly SuggestedStartTime { get; init; }
    public int DurationMinutes { get; init; }
    public int DefaultCapacity { get; init; }
    public decimal? DefaultCost { get; init; }
}
