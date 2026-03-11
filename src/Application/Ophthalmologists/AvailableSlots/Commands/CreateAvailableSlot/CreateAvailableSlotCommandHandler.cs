using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Repositories;

namespace Application.Ophthalmologists.AvailableSlots.Commands.CreateAvailableSlot;

/// <summary>
/// Handler for CreateAvailableSlotCommand.
/// </summary>
public class CreateAvailableSlotCommandHandler : ICommandHandler<CreateAvailableSlotCommand, Guid>
{
    private readonly IAvailableSlotRepository _availableSlotRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAvailableSlotCommandHandler(
        IAvailableSlotRepository availableSlotRepository,
        IUnitOfWork unitOfWork)
    {
        _availableSlotRepository = availableSlotRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAvailableSlotCommand request, CancellationToken cancellationToken)
    {
        // Check for overlapping slots
        var hasOverlap = await _availableSlotRepository.HasOverlappingSlotAsync(
            request.OphthalmologistId,
            request.OrganisationId,
            request.StartTime,
            request.EndTime,
            cancellationToken: cancellationToken);

        if (hasOverlap)
        {
            return Result<Guid>.Conflict("An overlapping available slot already exists for this time period.");
        }

        var availableSlot = new AvailableSlot(
            request.StartTime,
            request.EndTime,
            request.MaxCapacity,
            request.OrganisationId,
            request.OphthalmologistId);

        await _availableSlotRepository.AddAsync(availableSlot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(availableSlot.Id);
    }
}
