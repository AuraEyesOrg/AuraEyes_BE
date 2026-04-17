using FluentValidation;

namespace Application.SystemAdmin.Ophthalmologists.Commands.RejectWithdrawalRequest;

public class RejectWithdrawalRequestCommandValidator : AbstractValidator<RejectWithdrawalRequestCommand>
{
    public RejectWithdrawalRequestCommandValidator()
    {
        RuleFor(x => x.WithdrawalRequestId)
            .NotEmpty();

        RuleFor(x => x.AdminUserId)
            .NotEmpty();

        RuleFor(x => x.Reason)
            .MaximumLength(1000);
    }
}
