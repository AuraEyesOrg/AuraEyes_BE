using Application.AiQuota.Commands.BuyAiQuota;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class BuyAiQuotaCommandValidatorTests
{
    private readonly BuyAiQuotaCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new BuyAiQuotaCommand { QuotaAmount = 5 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DefaultQuotaAmount_ShouldPass()
    {
        var command = new BuyAiQuotaCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_QuotaAmountOne_ShouldPass()
    {
        var command = new BuyAiQuotaCommand { QuotaAmount = 1 };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_QuotaAmountNotGreaterThanZero_ShouldFail(int quotaAmount)
    {
        var command = new BuyAiQuotaCommand { QuotaAmount = quotaAmount };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.QuotaAmount)
            .WithErrorMessage("Quota amount must be greater than 0.");
    }
}
