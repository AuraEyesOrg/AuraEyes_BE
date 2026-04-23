using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;

public class GetAppointmentSlotsQueryHandler : IQueryHandler<GetAppointmentSlotsQuery, PagedResult<AppointmentSlotListDto>>
{
    private readonly IAppointmentSlotRepository _repository;

    public GetAppointmentSlotsQueryHandler(IAppointmentSlotRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<AppointmentSlotListDto>>> Handle(
        GetAppointmentSlotsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            request.ScheduleTemplateId,
            request.Status,
            request.FromDate,
            request.ToDate,
            request.ExcludePastSlots,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = items.Select(slot =>
        {
            var availableCapacity = slot.MaxCapacity - slot.BookedCount;

            return new AppointmentSlotListDto
            {
                Id = slot.Id,
                ScheduleTemplateId = slot.ScheduleTemplateId,
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Status = slot.Status.ToString(),
                MaxCapacity = slot.MaxCapacity,
                BookedCount = slot.BookedCount,
                AvailableCapacity = availableCapacity,
                CreatedAt = slot.CreatedAt
            };
        }).ToList();

        var resultPage = new PagedResult<AppointmentSlotListDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<AppointmentSlotListDto>>.Success(resultPage);
    }
}
