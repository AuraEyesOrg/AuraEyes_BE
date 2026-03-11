using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Ophthalmologists.Schedules.Commands.DeleteSchedule;

/// <summary>
/// Handler for DeleteScheduleCommand.
/// Cancels the schedule (soft delete via status transition).
/// </summary>
public class DeleteScheduleCommandHandler : ICommandHandler<DeleteScheduleCommand>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteScheduleCommandHandler> _logger;

    public DeleteScheduleCommandHandler(
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteScheduleCommandHandler> logger)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result.NotFound($"Schedule '{request.ScheduleId}' not found.");

        if (schedule.Status == ScheduleStatus.Completed)
            return Result.Failure("Cannot delete a completed schedule.");

        if (schedule.Status == ScheduleStatus.Cancelled)
            return Result.Failure("Schedule is already cancelled.");

        schedule.Cancel();

        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Schedule {ScheduleId} cancelled (deleted)", request.ScheduleId);

        return Result.Success();
    }
}
