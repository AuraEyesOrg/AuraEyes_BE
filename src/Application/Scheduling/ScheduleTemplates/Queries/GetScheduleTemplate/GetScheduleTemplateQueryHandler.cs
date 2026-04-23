using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.ScheduleTemplates.Common;
using Domain.Repositories;

namespace Application.Scheduling.ScheduleTemplates.Queries.GetScheduleTemplate;

public class GetScheduleTemplateQueryHandler : IQueryHandler<GetScheduleTemplateQuery, ScheduleTemplateDto>
{
    private readonly IScheduleTemplateRepository _repository;

    public GetScheduleTemplateQueryHandler(IScheduleTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ScheduleTemplateDto>> Handle(GetScheduleTemplateQuery request, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(request.TemplateId, cancellationToken);
        if (template is null)
            return Result<ScheduleTemplateDto>.NotFound($"Schedule template with ID '{request.TemplateId}' was not found.");

        var dto = new ScheduleTemplateDto
        {
            Id = template.Id,
            DayOfWeek = template.DayOfWeek.ToString(),
            StartTime = template.StartTime,
            EndTime = template.EndTime,
            SlotDuration = template.SlotDuration,
            MaxCapacity = template.MaxCapacity,
            Cost = template.Cost,
            Source = template.Source.ToString(),
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };

        return Result<ScheduleTemplateDto>.Success(dto);
    }
}
