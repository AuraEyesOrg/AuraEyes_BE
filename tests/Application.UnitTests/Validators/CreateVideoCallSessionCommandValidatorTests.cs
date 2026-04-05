using Application.ConsultationSessions.Commands.CreateVideoCallSession;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateVideoCallSessionCommandValidatorTests
{
    private readonly CreateVideoCallSessionCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateVideoCallSessionCommand
        {
            PatientId = Guid.NewGuid(),
            Price = 100_000m,
            AppointmentTime = DateTime.UtcNow.AddDays(1)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyPatientId_ShouldFail()
    {
        var command = new CreateVideoCallSessionCommand
        {
            PatientId = Guid.Empty,
            Price = 100_000m,
            AppointmentTime = DateTime.UtcNow.AddDays(1)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PatientId)
            .WithErrorMessage("Patient ID is required.");
    }

    [Fact]
    public void Validate_NegativePrice_ShouldFail()
    {
        var command = new CreateVideoCallSessionCommand
        {
            PatientId = Guid.NewGuid(),
            Price = -1m,
            AppointmentTime = DateTime.UtcNow.AddDays(1)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("Price cannot be negative.");
    }

    [Fact]
    public void Validate_ZeroPrice_ShouldPass()
    {
        var command = new CreateVideoCallSessionCommand
        {
            PatientId = Guid.NewGuid(),
            Price = 0m,
            AppointmentTime = DateTime.UtcNow.AddDays(1)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_PastAppointmentTime_ShouldFail()
    {
        var command = new CreateVideoCallSessionCommand
        {
            PatientId = Guid.NewGuid(),
            Price = 100_000m,
            AppointmentTime = DateTime.UtcNow.AddHours(-1)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AppointmentTime)
            .WithErrorMessage("Appointment time must be in the future.");
    }

    [Fact]
    public void Validate_WithOptionalOphthalmologistId_ShouldPass()
    {
        var command = new CreateVideoCallSessionCommand
        {
            PatientId = Guid.NewGuid(),
            Price = 50_000m,
            AppointmentTime = DateTime.UtcNow.AddDays(7),
            OphthalmologistId = Guid.NewGuid()
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
