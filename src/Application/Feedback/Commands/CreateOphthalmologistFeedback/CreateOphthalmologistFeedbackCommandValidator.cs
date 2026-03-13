using FluentValidation;

namespace Application.Feedback.Commands.CreateOphthalmologistFeedback;

public class CreateOphthalmologistFeedbackCommandValidator : AbstractValidator<CreateOphthalmologistFeedbackCommand>
{
    public CreateOphthalmologistFeedbackCommandValidator()
    {
        RuleFor(x => x.OphthalmologistId)
            .NotEmpty()
            .WithMessage("Ophthalmologist ID is required.");

        RuleFor(x => x.ConsultationSessionId)
            .NotEmpty()
            .WithMessage("Consultation session ID is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage("Comment cannot exceed 2000 characters.");
    }
}
