namespace Application.Scheduling.ScheduleTemplates.Common;

/// <summary>
/// DTO for schedule template list items.
/// </summary>
public record ScheduleTemplateListDto
{
    public Guid Id { get; init; }
    public Guid? OphthalId { get; init; }
    public Guid? OrgId { get; init; }
    public string DayOfWeek { get; init; } = string.Empty;
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int SlotDuration { get; init; }
    public int MaxCapacity { get; init; }
    public decimal? Cost { get; init; }
    public string Source { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record ScheduleTemplateDto
{
    public Guid Id { get; init; }
    public Guid? OphthalId { get; init; }
    public Guid? OrgId { get; init; }
    public string DayOfWeek { get; init; } = string.Empty;
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int SlotDuration { get; init; }
    public int MaxCapacity { get; init; }
    public decimal? Cost { get; init; }
    public string Source { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
