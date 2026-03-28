using FluentValidation;

namespace Application.SystemAdmin.Ophthalmologists.Commands.ConfirmWithdrawalRequest;

public class ConfirmWithdrawalRequestCommandValidator : AbstractValidator<ConfirmWithdrawalRequestCommand>
{
    public ConfirmWithdrawalRequestCommandValidator()
    {
        RuleFor(x => x.WithdrawalRequestId)
            .NotEmpty();

        RuleFor(x => x.AdminUserId)
            .NotEmpty();

        RuleFor(x => x.TransferReference)
            .MaximumLength(200);

        RuleFor(x => x.Note)
            .MaximumLength(1000);
    }
}
