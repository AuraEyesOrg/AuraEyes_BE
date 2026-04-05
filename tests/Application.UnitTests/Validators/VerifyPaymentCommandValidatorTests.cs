using Application.Wallets.Commands.VerifyPayment;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class VerifyPaymentCommandValidatorTests
{
    private readonly VerifyPaymentCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidOrderCode_ShouldPass()
    {
        var command = new VerifyPaymentCommand { OrderCode = "ORDER-001" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyOrderCode_ShouldFail()
    {
        var command = new VerifyPaymentCommand { OrderCode = string.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.OrderCode)
            .WithErrorMessage("Order code is required.");
    }

    [Fact]
    public void Validate_OrderCodeTooLong_ShouldFail()
    {
        var command = new VerifyPaymentCommand { OrderCode = new string('X', 101) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.OrderCode)
            .WithErrorMessage("Order code cannot exceed 100 characters.");
    }
}
