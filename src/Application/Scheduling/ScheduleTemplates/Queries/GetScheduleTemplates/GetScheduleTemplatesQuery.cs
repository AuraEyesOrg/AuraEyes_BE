using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.ScheduleTemplates.Common;

namespace Application.Scheduling.ScheduleTemplates.Queries.GetScheduleTemplates;

public record GetScheduleTemplatesQuery : IQuery<PagedResult<ScheduleTemplateListDto>>
{
    public DayOfWeek? DayOfWeek { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
