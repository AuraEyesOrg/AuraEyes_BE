using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Schedules.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.Schedules.Queries.GetSchedules;

/// <summary>
/// Handler for GetSchedulesQuery.
/// </summary>
public class GetSchedulesQueryHandler : IQueryHandler<GetSchedulesQuery, PagedResult<ScheduleListDto>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public GetSchedulesQueryHandler(
        IScheduleRepository scheduleRepository,
        IOphthalmologistRepository ophthalmologistRepository)
    {
        _scheduleRepository = scheduleRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<PagedResult<ScheduleListDto>>> Handle(
        GetSchedulesQuery request,
        CancellationToken cancellationToken)
    {
        // Verify ophthalmologist exists
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result<PagedResult<ScheduleListDto>>.NotFound(
                $"Ophthalmologist with ID '{request.OphthalmologistId}' was not found.");
        }

        var (items, totalCount) = await _scheduleRepository.GetPagedAsync(
            request.OphthalmologistId,
            request.Status,
            request.SlotType,
            request.FromDate,
            request.ToDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = items.Select(schedule => new ScheduleListDto
        {
            Id = schedule.Id,
            AvailabilityId = schedule.AvailabilityId,
            PatientId = schedule.PatientId,
            Date = schedule.Date,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            Status = schedule.Status,
            SlotType = schedule.SlotType,
            Cost = schedule.Cost,
            CreatedAt = schedule.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<ScheduleListDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<ScheduleListDto>>.Success(pagedResult);
    }
}
