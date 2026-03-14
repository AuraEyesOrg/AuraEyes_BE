using Application.Common.Interfaces;

namespace Application.Scheduling.ScheduleTemplates.Commands.CreateScheduleTemplate;

/// <summary>
/// Command to create a new schedule template.
/// </summary>
public record CreateScheduleTemplateCommand : ICommand<Guid>
{
    public Guid? OrgId { get; init; }
    public Guid? OphthalId { get; init; }
    public DayOfWeek DayOfWeek { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int SlotDuration { get; init; }
    public int MaxCapacity { get; init; }
    public decimal? Cost { get; init; }
}
