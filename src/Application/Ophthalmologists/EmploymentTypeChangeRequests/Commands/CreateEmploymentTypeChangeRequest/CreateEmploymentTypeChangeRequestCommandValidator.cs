using Domain.Enums;
using FluentValidation;

namespace Application.Ophthalmologists.EmploymentTypeChangeRequests.Commands.CreateEmploymentTypeChangeRequest;

public class CreateEmploymentTypeChangeRequestCommandValidator : AbstractValidator<CreateEmploymentTypeChangeRequestCommand>
{
    public CreateEmploymentTypeChangeRequestCommandValidator()
    {
        RuleFor(x => x.OphthalmologistId)
            .NotEmpty().WithMessage("Ophthalmologist ID is required.");

        RuleFor(x => x.TargetEmploymentType)
            .Must(type => type == OphthalmologistEmploymentType.FullTime || type == OphthalmologistEmploymentType.PartTime)
            .WithMessage("Target employment type is invalid.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(1000).WithMessage("Reason must be 1000 characters or fewer.");
    }
}
