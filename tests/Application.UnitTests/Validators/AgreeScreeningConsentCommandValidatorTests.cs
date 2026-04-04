using Application.Consents.Commands.AgreeScreeningConsent;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class AgreeScreeningConsentCommandValidatorTests
{
    private readonly AgreeScreeningConsentCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new AgreeScreeningConsentCommand
        {
            ScreeningId = Guid.NewGuid(),
            Content = "I agree to the screening terms and conditions."
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyScreeningId_ShouldFail()
    {
        var command = new AgreeScreeningConsentCommand
        {
            ScreeningId = Guid.Empty,
            Content = "Valid content"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScreeningId)
            .WithErrorMessage("ScreeningId is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyOrNullContent_ShouldFail(string? content)
    {
        var command = new AgreeScreeningConsentCommand
        {
            ScreeningId = Guid.NewGuid(),
            Content = content!
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Consent content is required.");
    }

    [Fact]
    public void Validate_ContentExceeds4000Characters_ShouldFail()
    {
        var command = new AgreeScreeningConsentCommand
        {
            ScreeningId = Guid.NewGuid(),
            Content = new string('A', 4001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Consent content must not exceed 4000 characters.");
    }

    [Fact]
    public void Validate_ContentExactly4000Characters_ShouldPass()
    {
        var command = new AgreeScreeningConsentCommand
        {
            ScreeningId = Guid.NewGuid(),
            Content = new string('A', 4000)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
