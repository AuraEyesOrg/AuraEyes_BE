using FluentValidation;

namespace Application.AiQuota.Commands.BuyAiQuota;

public class BuyAiQuotaCommandValidator : AbstractValidator<BuyAiQuotaCommand>
{
    public BuyAiQuotaCommandValidator()
    {
        RuleFor(x => x.NumberOfBundles)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Must purchase at least 1 bundle.")
            .LessThanOrEqualTo(10)
            .WithMessage("Cannot purchase more than 10 bundles at once.");
    }
}
