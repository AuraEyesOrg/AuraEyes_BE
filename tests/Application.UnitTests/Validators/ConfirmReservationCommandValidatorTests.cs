using Application.Scheduling.AppointmentSlots.Commands.ConfirmReservation;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class ConfirmReservationCommandValidatorTests
{
    private readonly ConfirmReservationCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new ConfirmReservationCommand
        {
            AppointmentSlotId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            ShareRetinalImages = true,
            ShareAiResults = false
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ValidCommandWithScreeningId_ShouldPass()
    {
        var command = new ConfirmReservationCommand
        {
            AppointmentSlotId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            AiScreeningId = Guid.NewGuid(),
            ShareRetinalImages = true,
            ShareAiResults = true
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyAppointmentSlotId_ShouldFail()
    {
        var command = new ConfirmReservationCommand
        {
            AppointmentSlotId = Guid.Empty,
            PatientId = Guid.NewGuid()
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AppointmentSlotId)
            .WithErrorMessage("Appointment slot ID is required.");
    }

    [Fact]
    public void Validate_EmptyPatientId_ShouldFail()
    {
        var command = new ConfirmReservationCommand
        {
            AppointmentSlotId = Guid.NewGuid(),
            PatientId = Guid.Empty
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PatientId)
            .WithErrorMessage("Patient ID is required.");
    }

    [Fact]
    public void Validate_BothIdsEmpty_ShouldFailForBoth()
    {
        var command = new ConfirmReservationCommand
        {
            AppointmentSlotId = Guid.Empty,
            PatientId = Guid.Empty
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AppointmentSlotId);
        result.ShouldHaveValidationErrorFor(x => x.PatientId);
    }
}
