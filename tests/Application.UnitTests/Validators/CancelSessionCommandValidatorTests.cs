using Application.ConsultationSessions.Commands.CancelSession;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CancelSessionCommandValidatorTests
{
    private readonly CancelSessionCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CancelSessionCommand
        {
            SessionId = Guid.NewGuid(),
            CancelledByUserId = Guid.NewGuid(),
            Reason = "Patient requested cancellation"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NullReason_ShouldPass()
    {
        var command = new CancelSessionCommand
        {
            SessionId = Guid.NewGuid(),
            CancelledByUserId = Guid.NewGuid(),
            Reason = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptySessionId_ShouldFail()
    {
        var command = new CancelSessionCommand
        {
            SessionId = Guid.Empty,
            CancelledByUserId = Guid.NewGuid()
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SessionId)
            .WithErrorMessage("Session ID is required.");
    }

    [Fact]
    public void Validate_EmptyCancelledByUserId_ShouldFail()
    {
        var command = new CancelSessionCommand
        {
            SessionId = Guid.NewGuid(),
            CancelledByUserId = Guid.Empty
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CancelledByUserId)
            .WithErrorMessage("Cancelled-by user ID is required.");
    }

    [Fact]
    public void Validate_ReasonExceeds200Characters_ShouldFail()
    {
        var command = new CancelSessionCommand
        {
            SessionId = Guid.NewGuid(),
            CancelledByUserId = Guid.NewGuid(),
            Reason = new string('A', 201)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Reason)
            .WithErrorMessage("Reason must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_ReasonExactly200Characters_ShouldPass()
    {
        var command = new CancelSessionCommand
        {
            SessionId = Guid.NewGuid(),
            CancelledByUserId = Guid.NewGuid(),
            Reason = new string('A', 200)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
