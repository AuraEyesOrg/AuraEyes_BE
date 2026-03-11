using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Repositories;

namespace Application.Ophthalmologists.Schedules.Commands.CreateSchedule;

/// <summary>
/// Handler for CreateScheduleCommand.
/// </summary>
public class CreateScheduleCommandHandler : ICommandHandler<CreateScheduleCommand, Guid>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateScheduleCommandHandler(
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = new Schedule(
            request.AvailableSlotId,
            request.PatientId,
            request.Date,
            request.StartTime,
            request.EndTime,
            request.SlotType,
            request.Cost);

        await _scheduleRepository.AddAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(schedule.Id);
    }
}
