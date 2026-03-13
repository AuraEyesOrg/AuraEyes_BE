using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.Schedules.Commands.UpdateScheduleCost;

public class UpdateScheduleCostCommandHandler : ICommandHandler<UpdateScheduleCostCommand>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleCostCommandHandler(
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateScheduleCostCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result.NotFound($"Schedule '{request.ScheduleId}' not found.");

        schedule.UpdateCost(request.Cost);

        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
