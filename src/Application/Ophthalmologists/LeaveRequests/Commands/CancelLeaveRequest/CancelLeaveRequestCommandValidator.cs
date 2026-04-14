using FluentValidation;

namespace Application.Ophthalmologists.LeaveRequests.Commands.CancelLeaveRequest;

public class CancelLeaveRequestCommandValidator : AbstractValidator<CancelLeaveRequestCommand>
{
    public CancelLeaveRequestCommandValidator()
    {
        RuleFor(x => x.LeaveRequestId)
            .NotEmpty().WithMessage("Leave request ID is required.");

        RuleFor(x => x.OphthalmologistId)
            .NotEmpty().WithMessage("Ophthalmologist ID is required.");
    }
}
