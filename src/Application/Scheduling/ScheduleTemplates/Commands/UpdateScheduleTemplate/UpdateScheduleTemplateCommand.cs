using Application.Common.Interfaces;

namespace Application.Scheduling.ScheduleTemplates.Commands.UpdateScheduleTemplate;

/// <summary>
/// Command to update an existing schedule template.
/// </summary>
public record UpdateScheduleTemplateCommand : ICommand
{
    public Guid ScheduleTemplateId { get; init; }
    public DayOfWeek DayOfWeek { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int SlotDuration { get; init; }
    public int MaxCapacity { get; init; }
    public decimal? Cost { get; init; }
    public bool IsActive { get; init; }
}
