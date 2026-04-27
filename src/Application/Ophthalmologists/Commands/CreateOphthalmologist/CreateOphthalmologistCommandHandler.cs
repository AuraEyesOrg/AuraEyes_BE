using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.ScheduleTemplates.Interfaces;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.Commands.CreateOphthalmologist;

/// <summary>
/// Handler for CreateOphthalmologistCommand.
/// </summary>
public class CreateOphthalmologistCommandHandler : ICommandHandler<CreateOphthalmologistCommand, Guid>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    private readonly IUnitOfWork _unitOfWork;

    public CreateOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateOphthalmologistCommand request, CancellationToken cancellationToken)
    {
        // Verify that the user exists
        var user = await _identityService.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<Guid>.NotFound($"User with ID '{request.UserId}' was not found.");
        }

        // Check if ophthalmologist profile already exists for this user
        var existingProfile = await _ophthalmologistRepository.ExistsByUserIdAsync(request.UserId, cancellationToken);
        if (existingProfile)
        {
            return Result<Guid>.Conflict($"An ophthalmologist profile already exists for user '{request.UserId}'.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var ophthalmologist = new Ophthalmologist(
                request.UserId,
                request.Bio,
                request.YearsOfExperience,
                employmentType: request.EmploymentType,
                workingHoursPerWeek: request.WorkingHoursPerWeek,
                expectedMonthlySalary: request.ExpectedMonthlySalary);

            await _ophthalmologistRepository.AddAsync(ophthalmologist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // System generated templates are clinic-level now, not doctor-level.

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<Guid>.Success(ophthalmologist.Id);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
