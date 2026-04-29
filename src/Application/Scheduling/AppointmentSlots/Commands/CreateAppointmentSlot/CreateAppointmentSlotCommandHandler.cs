using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;

public class CreateAppointmentSlotCommandHandler : ICommandHandler<CreateAppointmentSlotCommand, Guid>
{
    private readonly IAppointmentSlotRepository _repository;
    private readonly IScheduleTemplateRepository _templateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAppointmentSlotCommandHandler> _logger;

    public CreateAppointmentSlotCommandHandler(
        IAppointmentSlotRepository repository,
        IScheduleTemplateRepository templateRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateAppointmentSlotCommandHandler> logger)
    {
        _repository = repository;
        _templateRepository = templateRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var template = await _templateRepository.GetByIdAsync(request.ScheduleTemplateId, cancellationToken);
            if (template is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.NotFound($"Schedule template with ID '{request.ScheduleTemplateId}' was not found.");
            }

            var hasOverlap = await _repository.HasOverlappingSlotAsync(
                request.Date,
                request.StartTime,
                request.EndTime,
                cancellationToken: cancellationToken);

            if (hasOverlap)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.Conflict("An overlapping appointment slot already exists for this date and time.");
            }

            var slot = new AppointmentSlot(
                request.ScheduleTemplateId,
                request.Date,
                request.StartTime,
                request.EndTime,
                template.MaxCapacity);

            await _repository.AddAsync(slot, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<Guid>.Success(slot.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(
                ex,
                "Error creating slot for template {ScheduleTemplateId} on {Date}",
                request.ScheduleTemplateId,
                request.Date);
            throw;
        }
    }
}
