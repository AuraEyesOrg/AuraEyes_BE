using FluentValidation;

namespace Application.SystemAdmin.Contracts.Commands.CreateContract;

public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        RuleFor(x => x.TemplateId).NotEmpty().WithMessage("Template ID is required.");
        RuleFor(x => x.ContractNumber)
            .NotEmpty().WithMessage("Contract number is required.")
            .MaximumLength(50).WithMessage("Contract number must not exceed 50 characters.");
        RuleFor(x => x.AiQuotaLimit)
            .GreaterThanOrEqualTo(0).WithMessage("AI quota limit cannot be negative.");
        RuleFor(x => x.PlatformCommissionRate)
            .InclusiveBetween(0m, 1m).WithMessage("Commission rate must be between 0 and 1 (0% – 100%).");
    }
}
