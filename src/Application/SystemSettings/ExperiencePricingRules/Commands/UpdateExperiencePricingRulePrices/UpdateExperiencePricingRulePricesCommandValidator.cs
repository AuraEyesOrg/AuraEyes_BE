using FluentValidation;

namespace Application.SystemSettings.ExperiencePricingRules.Commands.UpdateExperiencePricingRulePrices;

public class UpdateExperiencePricingRulePricesCommandValidator : AbstractValidator<UpdateExperiencePricingRulePricesCommand>
{
    public UpdateExperiencePricingRulePricesCommandValidator()
    {
        RuleFor(x => x.Rules)
            .NotEmpty()
            .WithMessage("At least one pricing rule must be provided.");

        RuleForEach(x => x.Rules)
            .SetValidator(new UpdateExperiencePricingRulePriceItemValidator());
    }
}

public class UpdateExperiencePricingRulePriceItemValidator : AbstractValidator<UpdateExperiencePricingRulePriceItem>
{
    public UpdateExperiencePricingRulePriceItemValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Rule ID is required.");

        RuleFor(x => x.MinPrice)
            .GreaterThan(0)
            .WithMessage("Minimum price must be greater than zero.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice)
            .WithMessage("Maximum price must be greater than or equal to minimum price.");

        RuleFor(x => x.MinPrice)
            .Must(price => decimal.Truncate(price) == price)
            .WithMessage("Minimum price must be an integer value.");

        RuleFor(x => x.MaxPrice)
            .Must(price => decimal.Truncate(price) == price)
            .WithMessage("Maximum price must be an integer value.");
    }
}