using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.ScheduleTemplates.Commands.DeleteScheduleTemplate;

/// <summary>
/// Handler for DeleteScheduleTemplateCommand.
/// Soft-deletes the template (sets IsDeleted flag).
/// </summary>
public class DeleteScheduleTemplateCommandHandler : ICommandHandler<DeleteScheduleTemplateCommand>
{
    private readonly IScheduleTemplateRepository _scheduleTemplateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteScheduleTemplateCommandHandler> _logger;

    public DeleteScheduleTemplateCommandHandler(
        IScheduleTemplateRepository scheduleTemplateRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteScheduleTemplateCommandHandler> logger)
    {
        _scheduleTemplateRepository = scheduleTemplateRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteScheduleTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _scheduleTemplateRepository.GetByIdWithSlotsAsync(
            request.ScheduleTemplateId, cancellationToken);

        if (template is null)
        {
            return Result.NotFound($"Schedule template with ID '{request.ScheduleTemplateId}' was not found.");
        }

        // Handle associated slots
        if (template.AppointmentSlots != null)
        {
            foreach (var slot in template.AppointmentSlots)
            {
                if (slot.BookedCount == 0)
                {
                    // No bookings: safe to soft-delete the slot
                    slot.SoftDelete();
                }
                else
                {
                    // Has bookings: keep the slot but block it from further changes/bookings
                    slot.ForceBlock();
                }
            }
        }

        template.SoftDelete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Schedule template {TemplateId} deleted", request.ScheduleTemplateId);

        return Result.Success();
    }
}
