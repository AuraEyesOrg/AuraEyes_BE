using FluentValidation;

namespace Application.Wallets.Commands.CreateDeposit;

/// <summary>
/// Validator for CreateDepositCommand.
/// </summary>
public class CreateDepositCommandValidator : AbstractValidator<CreateDepositCommand>
{
    private const decimal MinDeposit = 10000; // 10,000 VND minimum
    private const decimal MaxDeposit = 50000000; // 50,000,000 VND maximum

    public CreateDepositCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.AmountVnd)
            .GreaterThanOrEqualTo(MinDeposit)
            .WithMessage($"Minimum deposit amount is {MinDeposit:N0} VND.")
            .LessThanOrEqualTo(MaxDeposit)
            .WithMessage($"Maximum deposit amount is {MaxDeposit:N0} VND.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.ReturnUrl)
            .MaximumLength(1000)
            .WithMessage("Return URL cannot exceed 1000 characters.");

        RuleFor(x => x.CancelUrl)
            .MaximumLength(1000)
            .WithMessage("Cancel URL cannot exceed 1000 characters.");
    }
}
