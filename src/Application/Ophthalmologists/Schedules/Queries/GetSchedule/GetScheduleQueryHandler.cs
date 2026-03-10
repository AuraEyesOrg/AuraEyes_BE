using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Schedules.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.Schedules.Queries.GetSchedule;

public class GetScheduleQueryHandler : IQueryHandler<GetScheduleQuery, ScheduleDto>
{
    private readonly IScheduleRepository _scheduleRepository;

    public GetScheduleQueryHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<ScheduleDto>> Handle(
        GetScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result<ScheduleDto>.NotFound($"Schedule with ID '{request.ScheduleId}' was not found.");

        var dto = new ScheduleDto
        {
            Id = schedule.Id,
            AvailableSlotId = schedule.AvailableSlotId,
            PatientId = schedule.PatientId,
            Date = schedule.Date,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            Status = schedule.Status,
            SlotType = schedule.SlotType,
            Cost = schedule.Cost,
            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt
        };

        return Result<ScheduleDto>.Success(dto);
    }
}
