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
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateScheduleCommandHandler(
        IScheduleRepository scheduleRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        // Verify ophthalmologist exists
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result<Guid>.NotFound($"Ophthalmologist with ID '{request.OphthalmologistId}' was not found.");
        }

        // Check for overlapping schedules
        var hasOverlap = await _scheduleRepository.HasOverlappingScheduleAsync(
            request.OphthalmologistId,
            request.Date,
            request.StartTime,
            request.EndTime,
            cancellationToken: cancellationToken);

        if (hasOverlap)
        {
            return Result<Guid>.Conflict("This time slot overlaps with an existing schedule.");
        }

        // Create the schedule
        var schedule = new Schedule(
            request.OphthalmologistId,
            request.Date,
            request.StartTime,
            request.EndTime,
            request.SlotType,
            request.OrganisationId,
            request.Cost);

        await _scheduleRepository.AddAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(schedule.Id);
    }
}
