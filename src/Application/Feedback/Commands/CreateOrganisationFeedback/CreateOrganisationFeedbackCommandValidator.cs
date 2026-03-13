using FluentValidation;

namespace Application.Feedback.Commands.CreateOrganisationFeedback;

public class CreateOrganisationFeedbackCommandValidator : AbstractValidator<CreateOrganisationFeedbackCommand>
{
    public CreateOrganisationFeedbackCommandValidator()
    {
        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WithMessage("Organisation ID is required.");

        RuleFor(x => x.AppointmentId)
            .NotEmpty()
            .WithMessage("Appointment ID is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage("Comment cannot exceed 2000 characters.");
    }
}
