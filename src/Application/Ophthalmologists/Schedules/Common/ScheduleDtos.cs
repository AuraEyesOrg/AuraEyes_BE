using Domain.Enums;

namespace Application.Ophthalmologists.Schedules.Common;

/// <summary>
/// DTO for schedule list items.
/// </summary>
public record ScheduleListDto
{
    public Guid Id { get; init; }
    public Guid OphthalmologistId { get; init; }
    public Guid? OrganisationId { get; init; }
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

/// <summary>
/// DTO for schedule details.
/// </summary>
public record ScheduleDto
{
    public Guid Id { get; init; }
    public Guid OphthalmologistId { get; init; }
    public string? OphthalmologistName { get; init; }
    public Guid? OrganisationId { get; init; }
    public string? OrganisationName { get; init; }
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

/// <summary>
/// DTO for schedule statistics.
/// </summary>
public record ScheduleStatsDto
{
    public int TotalCount { get; init; }
    public int AvailableCount { get; init; }
    public int BookedCount { get; init; }
    public int CompletedCount { get; init; }
    public int CancelledCount { get; init; }
    public int NoShowCount { get; init; }
}
