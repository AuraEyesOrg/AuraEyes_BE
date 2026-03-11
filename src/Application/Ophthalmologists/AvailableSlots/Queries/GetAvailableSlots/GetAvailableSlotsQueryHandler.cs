using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.AvailableSlots.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.AvailableSlots.Queries.GetAvailableSlots;

/// <summary>
/// Handler for GetAvailableSlotsQuery.
/// </summary>
public class GetAvailableSlotsQueryHandler : IQueryHandler<GetAvailableSlotsQuery, PagedResult<AvailableSlotListDto>>
{
    private readonly IAvailableSlotRepository _availableSlotRepository;

    public GetAvailableSlotsQueryHandler(IAvailableSlotRepository availableSlotRepository)
    {
        _availableSlotRepository = availableSlotRepository;
    }

    public async Task<Result<PagedResult<AvailableSlotListDto>>> Handle(
        GetAvailableSlotsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _availableSlotRepository.GetPagedAsync(
            request.OphthalmologistId,
            request.OrganisationId,
            request.FromDate,
            request.ToDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        // We need schedules to calculate booked count, so let's enhance this query
        // For list view, we'll get the slots with schedules
        var dtoList = new List<AvailableSlotListDto>();
        
        foreach (var slot in items)
        {
            var withSchedules = await _availableSlotRepository.GetByIdWithSchedulesAsync(slot.Id, cancellationToken);
            var bookedCount = withSchedules?.Schedules.Count(s => s.Status != ScheduleStatus.Cancelled) ?? 0;

            dtoList.Add(new AvailableSlotListDto
            {
                Id = slot.Id,
                OrganisationId = slot.OrganisationId,
                OphthalmologistId = slot.OphthalmologistId,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                MaxCapacity = slot.MaxCapacity,
                BookedCount = bookedCount,
                CreatedAt = slot.CreatedAt
            });
        }

        var pagedResult = new PagedResult<AvailableSlotListDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<AvailableSlotListDto>>.Success(pagedResult);
    }
}
