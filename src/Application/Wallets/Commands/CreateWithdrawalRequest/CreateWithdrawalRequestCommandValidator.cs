using FluentValidation;

namespace Application.Wallets.Commands.CreateWithdrawalRequest;

public class CreateWithdrawalRequestCommandValidator : AbstractValidator<CreateWithdrawalRequestCommand>
{
    private const decimal MinWithdrawal = 10000;

    public CreateWithdrawalRequestCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.AmountVnd)
            .GreaterThanOrEqualTo(MinWithdrawal)
            .WithMessage($"Minimum withdrawal amount is {MinWithdrawal:N0} VND.");

        RuleFor(x => x.BankName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.BankAccountNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.AccountHolderName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ContractNumber)
            .MaximumLength(100);

        RuleFor(x => x.Note)
            .MaximumLength(1000);
    }
}
