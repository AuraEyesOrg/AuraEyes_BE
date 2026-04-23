using FluentValidation;

namespace Application.ClinicStaffs.Commands.CreateClinicStaff;

public class CreateClinicStaffCommandValidator : AbstractValidator<CreateClinicStaffCommand>
{
    public CreateClinicStaffCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.SubRoles)
            .NotNull().WithMessage("SubRoles cannot be null.")
            .Must(r => r.Count > 0).WithMessage("At least one sub-role must be assigned.");

        RuleFor(x => x.Department)
            .MaximumLength(100).When(x => x.Department is not null);

        RuleFor(x => x.EmployeeCode)
            .MaximumLength(50).When(x => x.EmployeeCode is not null);

        RuleFor(x => x.Phone)
            .MaximumLength(20).When(x => x.Phone is not null);
    }
}
