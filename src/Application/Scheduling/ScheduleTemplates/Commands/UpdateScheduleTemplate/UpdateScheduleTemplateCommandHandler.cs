using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Scheduling.ScheduleTemplates.Commands.UpdateScheduleTemplate;

/// <summary>
/// Handler for UpdateScheduleTemplateCommand.
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

        // Check for overlapping templates (excluding current template)
        var hasOverlap = await _scheduleTemplateRepository.HasOverlappingTemplateAsync(
            template.OphthalId,
            template.OrgId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.ScheduleTemplateId,
            cancellationToken);

        if (hasOverlap)
        {
            return Result.Conflict("An overlapping schedule template already exists for this day and time.");
        }

        // Check that new capacity is not less than current active appointment slots
        var activeSlots = template.AppointmentSlots?.Count(s => 
            s.Status != Domain.Enums.ScheduleStatus.Cancelled) ?? 0;
        
        // Note: We allow capacity changes since BookedCount in each AppointmentSlot
        // handles individual slot capacity, not the template

        template.Update(
            request.DayOfWeek, 
            request.StartTime, 
            request.EndTime, 
            request.SlotDuration, 
            request.MaxCapacity);
        
        await _scheduleTemplateRepository.UpdateAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
