using Application.Screenings.Commands.CompleteAiScreening;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CompleteAiScreeningCommandValidatorTests
{
    private readonly CompleteAiScreeningCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CompleteAiScreeningCommand
        {
            ScreeningId = Guid.NewGuid(),
            RawJsonOutput = "{\"result\":\"ok\"}",
            ResultStatus = "Normal"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyScreeningId_ShouldFail()
    {
        var command = new CompleteAiScreeningCommand
        {
            ScreeningId = Guid.Empty,
            RawJsonOutput = "{\"result\":\"ok\"}",
            ResultStatus = "Normal"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ScreeningId)
            .WithErrorMessage("Screening ID is required");
    }

    [Fact]
    public void Validate_EmptyRawJsonOutput_ShouldFail()
    {
        var command = new CompleteAiScreeningCommand
        {
            ScreeningId = Guid.NewGuid(),
            RawJsonOutput = "",
            ResultStatus = "Normal"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RawJsonOutput)
            .WithErrorMessage("Raw JSON output is required");
    }

    [Fact]
    public void Validate_EmptyResultStatus_ShouldFail()
    {
        var command = new CompleteAiScreeningCommand
        {
            ScreeningId = Guid.NewGuid(),
            RawJsonOutput = "{\"result\":\"ok\"}",
            ResultStatus = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ResultStatus)
            .WithErrorMessage("Result status is required");
    }

    [Fact]
    public void Validate_InvalidResultStatus_ShouldFail()
    {
        var command = new CompleteAiScreeningCommand
        {
            ScreeningId = Guid.NewGuid(),
            RawJsonOutput = "{\"result\":\"ok\"}",
            ResultStatus = "Invalid"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ResultStatus)
            .WithErrorMessage("Result status must be 'Normal', 'Abnormal', or 'RequiresReview'");
    }

    [Theory]
    [InlineData("Normal")]
    [InlineData("Abnormal")]
    [InlineData("RequiresReview")]
    public void Validate_ValidResultStatuses_ShouldPass(string status)
    {
        var command = new CompleteAiScreeningCommand
        {
            ScreeningId = Guid.NewGuid(),
            RawJsonOutput = "{\"result\":\"ok\"}",
            ResultStatus = status
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("normal")]
    [InlineData("ABNORMAL")]
    [InlineData("requiresreview")]
    public void Validate_ResultStatusCaseInsensitive_ShouldPass(string status)
    {
        var command = new CompleteAiScreeningCommand
        {
            ScreeningId = Guid.NewGuid(),
            RawJsonOutput = "{\"result\":\"ok\"}",
            ResultStatus = status
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
