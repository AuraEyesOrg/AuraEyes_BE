using FluentValidation;

namespace Application.Wallets.Commands.VerifyPayment;

/// <summary>
/// Validator for VerifyPaymentCommand.
/// </summary>
public class VerifyPaymentCommandValidator : AbstractValidator<VerifyPaymentCommand>
{
    public VerifyPaymentCommandValidator()
    {
        RuleFor(x => x.OrderCode)
            .NotEmpty()
            .WithMessage("Order code is required.")
            .MaximumLength(100)
            .WithMessage("Order code cannot exceed 100 characters.");
    }
}
