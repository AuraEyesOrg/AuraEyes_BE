using Application.Common.Interfaces;
using Application.Scheduling.ScheduleTemplates.Common;

namespace Application.Scheduling.ScheduleTemplates.Queries.GetScheduleTemplate;

public record GetScheduleTemplateQuery(Guid TemplateId) : IQuery<ScheduleTemplateDto>;
