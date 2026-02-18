using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Schedules.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.Schedules.Queries.GetSchedule;

/// <summary>
/// Handler for GetScheduleQuery.
/// </summary>
public class GetScheduleQueryHandler : IQueryHandler<GetScheduleQuery, ScheduleDto>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public GetScheduleQueryHandler(
        IScheduleRepository scheduleRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _scheduleRepository = scheduleRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
    }

    public async Task<Result<ScheduleDto>> Handle(
        GetScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
        {
            return Result<ScheduleDto>.NotFound($"Schedule with ID '{request.ScheduleId}' was not found.");
        }

        // Get ophthalmologist details
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(schedule.OphthalmologistId, cancellationToken);
        string? ophthalmologistName = null;
        if (ophthalmologist is not null)
        {
            var user = await _identityService.GetUserByIdAsync(ophthalmologist.UserId, cancellationToken);
            ophthalmologistName = user?.FullName;
        }

        var dto = new ScheduleDto
        {
            Id = schedule.Id,
            OphthalmologistId = schedule.OphthalmologistId,
            OphthalmologistName = ophthalmologistName,
            OrganisationId = schedule.OrganisationId,
            OrganisationName = null, // Can be populated if Organisation repository is available
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
