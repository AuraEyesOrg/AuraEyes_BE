using Application.Wallets.Commands.CreateWithdrawalRequest;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateWithdrawalRequestCommandValidatorTests
{
    private readonly CreateWithdrawalRequestCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateWithdrawalRequestCommand
        {
            UserId = Guid.NewGuid(),
            AmountVnd = 200_000m,
            BankName = "Vietcombank",
            BankAccountNumber = "1234567890",
            AccountHolderName = "Nguyen Van A",
            ContractNumber = "HD-001",
            Note = "Monthly payout"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyRequiredFields_ShouldFail()
    {
        var command = new CreateWithdrawalRequestCommand
        {
            UserId = Guid.Empty,
            AmountVnd = 1,
            BankName = string.Empty,
            BankAccountNumber = string.Empty,
            AccountHolderName = string.Empty
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
        result.ShouldHaveValidationErrorFor(x => x.AmountVnd);
        result.ShouldHaveValidationErrorFor(x => x.BankName);
        result.ShouldHaveValidationErrorFor(x => x.BankAccountNumber);
        result.ShouldHaveValidationErrorFor(x => x.AccountHolderName);
    }

    [Fact]
    public void Validate_AmountBelowMinimum_ShouldFail()
    {
        var command = new CreateWithdrawalRequestCommand
        {
            UserId = Guid.NewGuid(),
            AmountVnd = 9_999,
            BankName = "VCB",
            BankAccountNumber = "123",
            AccountHolderName = "Owner"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AmountVnd)
            .WithErrorMessage("Minimum withdrawal amount is 10,000 VND.");
    }

    [Fact]
    public void Validate_NoteTooLong_ShouldFail()
    {
        var command = new CreateWithdrawalRequestCommand
        {
            UserId = Guid.NewGuid(),
            AmountVnd = 20_000,
            BankName = "VCB",
            BankAccountNumber = "123",
            AccountHolderName = "Owner",
            Note = new string('N', 1001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Note);
    }
}
