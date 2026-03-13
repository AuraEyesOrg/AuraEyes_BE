using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;

public class GetAppointmentSlotsQueryHandler : IQueryHandler<GetAppointmentSlotsQuery, PagedResult<AppointmentSlotListDto>>
{
    private readonly IAppointmentSlotRepository _repository;
    private readonly IScheduleTemplateRepository _templateRepository;

    public GetAppointmentSlotsQueryHandler(
        IAppointmentSlotRepository repository,
        IScheduleTemplateRepository templateRepository)
    {
        _repository = repository;
        _templateRepository = templateRepository;
    }

    public async Task<Result<PagedResult<AppointmentSlotListDto>>> Handle(
        GetAppointmentSlotsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Scheduling.AppointmentSlot> items;
        int totalCount;

        if (request.OphthalId.HasValue || request.OrgId.HasValue)
        {
            IReadOnlyList<Domain.Entities.Scheduling.AppointmentSlot> scopedItems;

            if (request.OphthalId.HasValue)
            {
                scopedItems = await _repository.GetByOphthalmologistAsync(
                    request.OphthalId.Value,
                    request.FromDate,
                    request.ToDate,
                    request.Status,
                    cancellationToken);

                if (request.OrgId.HasValue)
                {
                    scopedItems = scopedItems
                        .Where(slot => slot.ScheduleTemplate?.OrgId == request.OrgId.Value)
                        .ToList();
                }
            }
            else
            {
                scopedItems = await _repository.GetByOrganisationAsync(
                    request.OrgId!.Value,
                    request.FromDate,
                    request.ToDate,
                    request.Status,
                    cancellationToken);
            }

            if (request.Status == ScheduleStatus.Available)
            {
                scopedItems = scopedItems
                    .Where(slot => slot.BookedCount < slot.MaxCapacity)
                    .ToList();
            }

            totalCount = scopedItems.Count;
            items = scopedItems
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
        }
        else
        {
            var pagedResult = await _repository.GetPagedAsync(
                request.ScheduleTemplateId,
                request.Status,
                request.FromDate,
                request.ToDate,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            items = pagedResult.Items;
            totalCount = pagedResult.TotalCount;
        }

        // Get templates to calculate available capacity
        var dtoList = new List<AppointmentSlotListDto>();
        foreach (var slot in items)
        {
            var slotWithTemplate = await _repository.GetByIdWithTemplateAsync(slot.Id, cancellationToken);
            var availableCapacity = slotWithTemplate?.ScheduleTemplate != null
                ? slotWithTemplate.ScheduleTemplate.MaxCapacity - slot.BookedCount
                : 0;

            dtoList.Add(new AppointmentSlotListDto
            {
                Id = slot.Id,
                ScheduleTemplateId = slot.ScheduleTemplateId,
                OphthalId = slotWithTemplate?.ScheduleTemplate?.OphthalId,
                OrgId = slotWithTemplate?.ScheduleTemplate?.OrgId,
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Status = slot.Status.ToString(),
                Cost = slot.Cost,
                MaxCapacity = slot.MaxCapacity,
                BookedCount = slot.BookedCount,
                AvailableCapacity = availableCapacity,
                CreatedAt = slot.CreatedAt
            });
        }

        var resultPage = new PagedResult<AppointmentSlotListDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<AppointmentSlotListDto>>.Success(resultPage);
    }
}
