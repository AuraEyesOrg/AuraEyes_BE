using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.ScheduleTemplates.Common;
using Domain.Repositories;

namespace Application.Scheduling.ScheduleTemplates.Queries.GetScheduleTemplates;

public class GetScheduleTemplatesQueryHandler : IQueryHandler<GetScheduleTemplatesQuery, PagedResult<ScheduleTemplateListDto>>
{
    private readonly IScheduleTemplateRepository _repository;

    public GetScheduleTemplatesQueryHandler(IScheduleTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<ScheduleTemplateListDto>>> Handle(
        GetScheduleTemplatesQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            request.DayOfWeek,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = items.Select(t => new ScheduleTemplateListDto
        {
            Id = t.Id,
            DayOfWeek = t.DayOfWeek.ToString(),
            StartTime = t.StartTime,
            EndTime = t.EndTime,
            SlotDuration = t.SlotDuration,
            MaxCapacity = t.MaxCapacity,
            Cost = t.Cost,
            Source = t.Source.ToString(),
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<ScheduleTemplateListDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<ScheduleTemplateListDto>>.Success(pagedResult);
    }
}
