using FluentValidation;

namespace Application.SystemAdmin.Ophthalmologists.Commands.ApproveLeaveRequest;

public class ApproveLeaveRequestCommandValidator : AbstractValidator<ApproveLeaveRequestCommand>
{
    public ApproveLeaveRequestCommandValidator()
    {
        RuleFor(x => x.LeaveRequestId)
            .NotEmpty().WithMessage("Leave request ID is required.");

        RuleFor(x => x.ReviewedByAdminUserId)
            .NotEmpty().WithMessage("Admin user ID is required.");

        RuleFor(x => x.AdminNote)
            .MaximumLength(1000).WithMessage("Admin note must be 1000 characters or fewer.");
    }
}
