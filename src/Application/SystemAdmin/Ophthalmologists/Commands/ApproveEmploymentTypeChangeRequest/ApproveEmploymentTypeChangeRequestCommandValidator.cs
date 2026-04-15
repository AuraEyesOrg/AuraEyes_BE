using FluentValidation;

namespace Application.SystemAdmin.Ophthalmologists.Commands.ApproveEmploymentTypeChangeRequest;

public class ApproveEmploymentTypeChangeRequestCommandValidator : AbstractValidator<ApproveEmploymentTypeChangeRequestCommand>
{
    public ApproveEmploymentTypeChangeRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty().WithMessage("Request ID is required.");

        RuleFor(x => x.ReviewedByAdminUserId)
            .NotEmpty().WithMessage("Admin user ID is required.");

        RuleFor(x => x.AdminNote)
            .MaximumLength(1000).WithMessage("Admin note must be 1000 characters or fewer.");
    }
}
