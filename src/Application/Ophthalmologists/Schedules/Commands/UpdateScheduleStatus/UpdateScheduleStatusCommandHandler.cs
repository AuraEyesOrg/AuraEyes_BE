using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.Schedules.Commands.UpdateScheduleStatus;

/// <summary>
/// Handler for UpdateScheduleStatusCommand.
/// </summary>
public class UpdateScheduleStatusCommandHandler : ICommandHandler<UpdateScheduleStatusCommand, bool>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleStatusCommandHandler(
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateScheduleStatusCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule is null)
        {
            return Result<bool>.NotFound($"Schedule with ID '{request.ScheduleId}' was not found.");
        }

        try
        {
            // Apply status change based on the requested new status
            switch (request.NewStatus)
            {
                case ScheduleStatus.Booked:
                    schedule.Book();
                    break;
                case ScheduleStatus.Cancelled:
                    schedule.Cancel();
                    break;
                case ScheduleStatus.Completed:
                    schedule.Complete();
                    break;
                case ScheduleStatus.NoShow:
                    schedule.MarkNoShow();
                    break;
                case ScheduleStatus.Available:
                    // Cannot revert to Available status after changes
                    return Result<bool>.Failure("Cannot revert schedule to Available status.");
                default:
                    return Result<bool>.Failure($"Unsupported status transition to '{request.NewStatus}'.");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
