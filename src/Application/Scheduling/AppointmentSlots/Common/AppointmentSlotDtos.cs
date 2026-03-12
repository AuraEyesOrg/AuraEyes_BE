namespace Application.Scheduling.AppointmentSlots.Common;

public record AppointmentSlotListDto
{
    public Guid Id { get; init; }
    public Guid ScheduleTemplateId { get; init; }
    public Guid? OphthalId { get; init; }
    public Guid? OrgId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal? Cost { get; init; }
    public int MaxCapacity { get; init; }
    public int BookedCount { get; init; }
    public int AvailableCapacity { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record AppointmentSlotDto
{
    public Guid Id { get; init; }
    public Guid ScheduleTemplateId { get; init; }
    public Guid? OphthalId { get; init; }
    public Guid? OrgId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal? Cost { get; init; }
    public int MaxCapacity { get; init; }
    public int BookedCount { get; init; }
    public int AvailableCapacity { get; init; }
    public Guid? ReservedBy { get; init; }
    public DateTime? ReservationExpireAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record AppointmentSlotStatsDto
{
    public int TotalCount { get; init; }
    public int AvailableCount { get; init; }
    public int BookedCount { get; init; }
    public int CompletedCount { get; init; }
    public int CancelledCount { get; init; }
    public int NoShowCount { get; init; }
}
