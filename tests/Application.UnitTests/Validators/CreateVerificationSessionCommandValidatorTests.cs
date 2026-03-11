using Application.ConsultationSessions.Commands.CreateVerificationSession;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateVerificationSessionCommandValidatorTests
{
    private readonly CreateVerificationSessionCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateVerificationSessionCommand
        {
            PatientId = Guid.NewGuid(),
            AiScreeningId = Guid.NewGuid(),
            Price = 50m
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyPatientId_ShouldFail()
    {
        var command = new CreateVerificationSessionCommand
        {
            PatientId = Guid.Empty,
            AiScreeningId = Guid.NewGuid(),
            Price = 50m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PatientId)
            .WithErrorMessage("Patient ID is required.");
    }

    [Fact]
    public void Validate_EmptyAiScreeningId_ShouldFail()
    {
        var command = new CreateVerificationSessionCommand
        {
            PatientId = Guid.NewGuid(),
            AiScreeningId = Guid.Empty,
            Price = 50m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AiScreeningId)
            .WithErrorMessage("AI Screening ID is required.");
    }

    [Fact]
    public void Validate_NegativePrice_ShouldFail()
    {
        var command = new CreateVerificationSessionCommand
        {
            PatientId = Guid.NewGuid(),
            AiScreeningId = Guid.NewGuid(),
            Price = -1m
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Price cannot be negative.");
    }

    [Fact]
    public void Validate_ZeroPrice_ShouldPass()
    {
        var command = new CreateVerificationSessionCommand
        {
            PatientId = Guid.NewGuid(),
            AiScreeningId = Guid.NewGuid(),
            Price = 0m
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_OptionalOphthalmologistId_ShouldPass()
    {
        var command = new CreateVerificationSessionCommand
        {
            PatientId = Guid.NewGuid(),
            AiScreeningId = Guid.NewGuid(),
            Price = 50m,
            OphthalmologistId = Guid.NewGuid()
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
