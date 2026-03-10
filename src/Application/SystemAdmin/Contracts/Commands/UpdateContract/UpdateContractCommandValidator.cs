using FluentValidation;

namespace Application.SystemAdmin.Contracts.Commands.UpdateContract;

public class UpdateContractCommandValidator : AbstractValidator<UpdateContractCommand>
{
    public UpdateContractCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TemplateId).NotEmpty().WithMessage("Template ID is required.");
        RuleFor(x => x.AiQuotaLimit).GreaterThanOrEqualTo(0).WithMessage("AI quota limit cannot be negative.");
        RuleFor(x => x.PlatformCommissionRate)
            .InclusiveBetween(0m, 1m).WithMessage("Commission rate must be between 0 and 1.");
    }
}
