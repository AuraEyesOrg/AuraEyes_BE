using Application.ConsultationSessions.Commands.EndSession;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class EndSessionCommandValidatorTests
{
    private readonly EndSessionCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new EndSessionCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid()
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptySessionId_ShouldFail()
    {
        var command = new EndSessionCommand
        {
            SessionId = Guid.Empty,
            DoctorId = Guid.NewGuid()
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SessionId)
            .WithErrorMessage("Session ID is required.");
    }

    [Fact]
    public void Validate_EmptyDoctorId_ShouldFail()
    {
        var command = new EndSessionCommand
        {
            SessionId = Guid.NewGuid(),
            DoctorId = Guid.Empty
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DoctorId)
            .WithErrorMessage("Doctor ID is required.");
    }

    [Fact]
    public void Validate_BothIdsEmpty_ShouldFailForBoth()
    {
        var command = new EndSessionCommand
        {
            SessionId = Guid.Empty,
            DoctorId = Guid.Empty
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.SessionId);
        result.ShouldHaveValidationErrorFor(x => x.DoctorId);
    }
}
