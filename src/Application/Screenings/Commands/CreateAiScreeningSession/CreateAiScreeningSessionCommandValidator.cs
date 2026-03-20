using FluentValidation;

namespace Application.Screenings.Commands.CreateAiScreeningSession;

/// <summary>
/// Validator for CreateAiScreeningSessionCommand
/// </summary>
public class CreateAiScreeningSessionCommandValidator : AbstractValidator<CreateAiScreeningSessionCommand>
{
    public CreateAiScreeningSessionCommandValidator()
    {
        RuleFor(x => x.RetinalImages)
            .NotEmpty().WithMessage("At least one retinal image is required.");

        RuleForEach(x => x.RetinalImages)
            .Must(img => !string.IsNullOrWhiteSpace(img.ImageUrl))
            .WithMessage("Image URL cannot be empty.");

        RuleForEach(x => x.RetinalImages)
            .Must(img => img.EyeSide != 0)
            .WithMessage("EyeSide must be specified.");
    }
}
