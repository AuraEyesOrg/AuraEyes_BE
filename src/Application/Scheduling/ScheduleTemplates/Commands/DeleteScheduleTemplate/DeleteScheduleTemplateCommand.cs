using Application.Common.Interfaces;

namespace Application.Scheduling.ScheduleTemplates.Commands.DeleteScheduleTemplate;

/// <summary>
/// Command to delete (soft-delete) a schedule template.
/// </summary>
public record DeleteScheduleTemplateCommand : ICommand
{
    public Guid ScheduleTemplateId { get; init; }
}
