using FluentValidation;

namespace Application.AiQuota.Commands.BuyAiQuota;

public class BuyAiQuotaCommandValidator : AbstractValidator<BuyAiQuotaCommand>
{
    public BuyAiQuotaCommandValidator()
    {
        RuleFor(x => x.QuotaAmount)
            .GreaterThan(0)
            .WithMessage("Quota amount must be greater than 0.");
    }
}
