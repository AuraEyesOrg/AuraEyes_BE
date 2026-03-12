using FluentValidation;

namespace Application.Screenings.Commands.CompleteAiScreening;

/// <summary>
/// Validator for CompleteAiScreeningCommand
/// </summary>
public class CompleteAiScreeningCommandValidator : AbstractValidator<CompleteAiScreeningCommand>
{
    public CompleteAiScreeningCommandValidator()
    {
        RuleFor(x => x.ScreeningId)
            .NotEmpty()
            .WithMessage("Screening ID is required");

        RuleFor(x => x.RawJsonOutput)
            .NotEmpty()
            .WithMessage("Raw JSON output is required");

        RuleFor(x => x.ResultStatus)
            .NotEmpty()
            .WithMessage("Result status is required")
            .Must(BeValidStatus)
            .WithMessage("Result status must be 'Normal', 'Abnormal', or 'RequiresReview'");
    }

    private static bool BeValidStatus(string status)
    {
        var validStatuses = new[] { "Normal", "Abnormal", "RequiresReview" };
        return validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase);
    }
}
