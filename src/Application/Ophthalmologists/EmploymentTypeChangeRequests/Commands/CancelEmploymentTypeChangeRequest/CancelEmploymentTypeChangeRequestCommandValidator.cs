using FluentValidation;

namespace Application.Ophthalmologists.EmploymentTypeChangeRequests.Commands.CancelEmploymentTypeChangeRequest;

public class CancelEmploymentTypeChangeRequestCommandValidator : AbstractValidator<CancelEmploymentTypeChangeRequestCommand>
{
    public CancelEmploymentTypeChangeRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty().WithMessage("Request ID is required.");

        RuleFor(x => x.OphthalmologistId)
            .NotEmpty().WithMessage("Ophthalmologist ID is required.");
    }
}
