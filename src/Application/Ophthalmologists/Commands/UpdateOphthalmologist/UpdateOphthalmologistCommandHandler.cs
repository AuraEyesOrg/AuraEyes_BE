using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.ScheduleTemplates.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

/// <summary>
/// Handler for UpdateOphthalmologistCommand.
/// </summary>
public class UpdateOphthalmologistCommandHandler : ICommandHandler<UpdateOphthalmologistCommand>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;
    private readonly IFullTimeTemplateProvisioningService _fullTimeTemplateProvisioningService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOphthalmologistCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService,
        IFullTimeTemplateProvisioningService fullTimeTemplateProvisioningService,
        IUnitOfWork unitOfWork)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
        _fullTimeTemplateProvisioningService = fullTimeTemplateProvisioningService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateOphthalmologistCommand request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.Id, cancellationToken);

        if (ophthalmologist is null)
        {
            return Result.NotFound($"Ophthalmologist with ID '{request.Id}' was not found.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            ophthalmologist.UpdateProfile(request.Bio, request.YearsOfExperience);

            var targetEmploymentType = request.EmploymentType ?? ophthalmologist.EmploymentType;
            var targetWorkingHours = request.WorkingHoursPerWeek ?? ophthalmologist.WorkingHoursPerWeek;
            var targetExpectedSalary = request.ExpectedMonthlySalary ?? ophthalmologist.ExpectedMonthlySalary;

            ophthalmologist.UpdateEmploymentPreferences(
                targetEmploymentType,
                targetWorkingHours,
                targetExpectedSalary);

            if (request.UserId.HasValue)
            {
                var (succeeded, errors) = await _identityService.UpdateUserProfileAsync(
                    request.UserId.Value,
                    request.FullName ?? string.Empty,
                    request.Phone,
                    null,
                    null,
                    request.Address,
                    cancellationToken);

                if (!succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Failure(errors);
                }
            }

            await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Idempotent self-healing: ensure missing system-generated templates are provisioned
            // whenever the doctor is currently full-time, even if no employment transition happened.
            if (targetEmploymentType == OphthalmologistEmploymentType.FullTime)
            {
                await _fullTimeTemplateProvisioningService.EnsureSystemGeneratedTemplatesAsync(
                    ophthalmologist,
                    cancellationToken);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result.Success();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
