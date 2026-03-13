using FluentValidation;

namespace Application.Feedback.Commands.CreateWebsiteFeedback;

public class CreateWebsiteFeedbackCommandValidator : AbstractValidator<CreateWebsiteFeedbackCommand>
{
    public CreateWebsiteFeedbackCommandValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage("Comment cannot exceed 2000 characters.");
    }
}
