using Application.ConsultationSessions.Commands.SendMessage;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class SendMessageCommandValidatorTests
{
    private readonly SendMessageCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new SendMessageCommand
        {
            SessionId = Guid.NewGuid(),
            Message = "Hello, doctor!"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptySessionId_ShouldFail()
    {
        var command = new SendMessageCommand
        {
            SessionId = Guid.Empty,
            Message = "Hello"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SessionId)
            .WithErrorMessage("Session ID is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyOrNullMessage_ShouldFail(string? message)
    {
        var command = new SendMessageCommand
        {
            SessionId = Guid.NewGuid(),
            Message = message!
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Message)
            .WithErrorMessage("Message cannot be empty.");
    }

    [Fact]
    public void Validate_MessageExceeds4000Characters_ShouldFail()
    {
        var command = new SendMessageCommand
        {
            SessionId = Guid.NewGuid(),
            Message = new string('A', 4001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Message)
            .WithErrorMessage("Message must not exceed 4000 characters.");
    }

    [Fact]
    public void Validate_MessageExactly4000Characters_ShouldPass()
    {
        var command = new SendMessageCommand
        {
            SessionId = Guid.NewGuid(),
            Message = new string('A', 4000)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
