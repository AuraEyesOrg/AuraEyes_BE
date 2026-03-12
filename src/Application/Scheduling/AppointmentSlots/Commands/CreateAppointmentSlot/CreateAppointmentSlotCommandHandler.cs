using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;

public class CreateAppointmentSlotCommandHandler : ICommandHandler<CreateAppointmentSlotCommand, Guid>
{
    private readonly IAppointmentSlotRepository _repository;
    private readonly IScheduleTemplateRepository _templateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAppointmentSlotCommandHandler(
        IAppointmentSlotRepository repository,
        IScheduleTemplateRepository templateRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _templateRepository = templateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        // Verify template exists
        var template = await _templateRepository.GetByIdAsync(request.ScheduleTemplateId, cancellationToken);
        if (template is null)
        {
            return Result<Guid>.NotFound($"Schedule template with ID '{request.ScheduleTemplateId}' was not found.");
        }

        var slot = new AppointmentSlot(
            request.ScheduleTemplateId,
            request.Date,
            request.StartTime,
            request.EndTime,
            request.SlotType,
            request.Cost);

        await _repository.AddAsync(slot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(slot.Id);
    }
}
