using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Scheduling.ScheduleTemplates.Commands.UpdateScheduleTemplate;

/// <summary>
/// Handler for UpdateScheduleTemplateCommand. Clinic-centric model.
/// </summary>
public class UpdateScheduleTemplateCommandHandler : ICommandHandler<UpdateScheduleTemplateCommand>
{
    private readonly IScheduleTemplateRepository _scheduleTemplateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleTemplateCommandHandler(
        IScheduleTemplateRepository scheduleTemplateRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleTemplateRepository = scheduleTemplateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateScheduleTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _scheduleTemplateRepository.GetByIdWithSlotsAsync(
            request.ScheduleTemplateId, cancellationToken);

        if (template is null)
        {
            return Result.NotFound($"Schedule template with ID '{request.ScheduleTemplateId}' was not found.");
        }

        // Check for overlapping templates only if we are keeping/setting this template as ACTIVE
        if (request.IsActive)
        {
            var hasOverlap = await _scheduleTemplateRepository.HasOverlappingTemplateAsync(
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request.ScheduleTemplateId,
                cancellationToken);

            if (hasOverlap)
            {
                return Result.Conflict("An overlapping schedule template already exists for this day and time.");
            }
        }

        template.Update(
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.SlotDuration,
            request.MaxCapacity,
            request.Cost,
            request.IsActive);

        await _scheduleTemplateRepository.UpdateAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
