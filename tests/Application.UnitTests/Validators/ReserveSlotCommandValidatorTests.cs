using Application.Scheduling.AppointmentSlots.Commands.ReserveSlot;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class ReserveSlotCommandValidatorTests
{
    private readonly ReserveSlotCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new ReserveSlotCommand
        {
            AppointmentSlotId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            ReservationMinutes = 5
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyAppointmentSlotId_ShouldFail()
    {
        var command = new ReserveSlotCommand
        {
            AppointmentSlotId = Guid.Empty,
            PatientId = Guid.NewGuid(),
            ReservationMinutes = 5
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AppointmentSlotId)
            .WithErrorMessage("Appointment slot ID is required.");
    }

    [Fact]
    public void Validate_EmptyPatientId_ShouldFail()
    {
        var command = new ReserveSlotCommand
        {
            AppointmentSlotId = Guid.NewGuid(),
            PatientId = Guid.Empty,
            ReservationMinutes = 5
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PatientId)
            .WithErrorMessage("Patient ID is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(16)]
    [InlineData(100)]
    public void Validate_ReservationMinutesOutOfRange_ShouldFail(int minutes)
    {
        var command = new ReserveSlotCommand
        {
            AppointmentSlotId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            ReservationMinutes = minutes
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ReservationMinutes)
            .WithErrorMessage("Reservation duration must be between 1 and 15 minutes.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(8)]
    [InlineData(15)]
    public void Validate_ReservationMinutesBoundaryValues_ShouldPass(int minutes)
    {
        var command = new ReserveSlotCommand
        {
            AppointmentSlotId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            ReservationMinutes = minutes
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ReservationMinutes);
    }
}
