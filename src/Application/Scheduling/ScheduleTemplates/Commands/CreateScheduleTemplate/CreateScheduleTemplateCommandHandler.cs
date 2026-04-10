using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.ScheduleTemplates.Commands.CreateScheduleTemplate;

public class CreateScheduleTemplateCommandHandler : ICommandHandler<CreateScheduleTemplateCommand, Guid>
{
    private readonly IScheduleTemplateRepository _repository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateScheduleTemplateCommandHandler(
        IScheduleTemplateRepository repository,
        IOphthalmologistRepository ophthalmologistRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateScheduleTemplateCommand request, CancellationToken cancellationToken)
    {
        var source = ScheduleTemplateSource.Doctor;

        if (request.OphthalId.HasValue)
        {
            var ophthal = await _ophthalmologistRepository.GetByIdAsync(request.OphthalId.Value, cancellationToken);
            if (ophthal is null)
            {
                return Result<Guid>.NotFound($"Ophthalmologist '{request.OphthalId.Value}' not found.");
            }

            if (ophthal.EmploymentType == OphthalmologistEmploymentType.FullTime)
            {
                return Result<Guid>.Forbidden("Full-time ophthalmologists cannot manually create schedule templates.");
            }

            source = ScheduleTemplateSource.Doctor;
        }

        // Check for overlapping templates
        var hasOverlap = await _repository.HasOverlappingTemplateAsync(
            request.OphthalId,
            request.OrgId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            cancellationToken: cancellationToken);

        if (hasOverlap)
        {
            return Result<Guid>.Conflict("An overlapping schedule template already exists for this time period.");
        }

        var template = new ScheduleTemplate(
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.SlotDuration,
            request.MaxCapacity,
            request.OrgId,
            request.OphthalId,
            request.Cost,
            source);

        await _repository.AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(template.Id);
    }
}
