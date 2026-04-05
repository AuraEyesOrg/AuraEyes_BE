using Application.Wallets.Commands.CreateDeposit;
using Domain.Enums;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateDepositCommandValidatorTests
{
    private readonly CreateDepositCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateDepositCommand
        {
            UserId = Guid.NewGuid(),
            AmountVnd = 100_000m,
            PaymentMethod = PaymentMethod.PayOS,
            Description = "Top up",
            ReturnUrl = "https://return.local",
            CancelUrl = "https://cancel.local"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldFail()
    {
        var command = new CreateDepositCommand { UserId = Guid.Empty, AmountVnd = 100_000m };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("UserId is required.");
    }

    [Theory]
    [InlineData(9999)]
    [InlineData(50000001)]
    public void Validate_AmountOutOfRange_ShouldFail(decimal amount)
    {
        var command = new CreateDepositCommand { UserId = Guid.NewGuid(), AmountVnd = amount };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AmountVnd);
    }

    [Fact]
    public void Validate_TooLongDescription_ShouldFail()
    {
        var command = new CreateDepositCommand
        {
            UserId = Guid.NewGuid(),
            AmountVnd = 100_000m,
            Description = new string('A', 501)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description cannot exceed 500 characters.");
    }
}
