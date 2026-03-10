using Domain.Enums;

namespace Application.Ophthalmologists.Schedules.Common;

public record ScheduleListDto
{
    public Guid Id { get; init; }
    public Guid AvailableSlotId { get; init; }
    public Guid PatientId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public ScheduleStatus Status { get; init; }
    public string StatusName => Status.ToString();
    public SlotType SlotType { get; init; }
    public string SlotTypeName => SlotType.ToString();
    public decimal? Cost { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record ScheduleDto
{
    public Guid Id { get; init; }
    public Guid AvailableSlotId { get; init; }
    public Guid PatientId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public ScheduleStatus Status { get; init; }
    public string StatusName => Status.ToString();
    public SlotType SlotType { get; init; }
    public string SlotTypeName => SlotType.ToString();
    public decimal? Cost { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record ScheduleStatsDto
{
    public int TotalCount { get; init; }
    public int AvailableCount { get; init; }
    public int BookedCount { get; init; }
    public int CompletedCount { get; init; }
    public int CancelledCount { get; init; }
    public int NoShowCount { get; init; }
}

