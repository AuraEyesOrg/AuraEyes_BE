using Application.Patients.Commands.UpdatePatientProfile;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class UpdatePatientProfileCommandValidatorTests
{
    private readonly UpdatePatientProfileCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new UpdatePatientProfileCommand
        {
            UserId = Guid.NewGuid(),
            FullName = "John Doe",
            Phone = "+84123456789",
            DateOfBirth = "1990-01-15",
            Gender = "male",
            Address = "123 Main St"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldFail()
    {
        var command = new UpdatePatientProfileCommand
        {
            UserId = Guid.Empty,
            FullName = "John Doe"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID is required.");
    }

    [Fact]
    public void Validate_EmptyFullName_ShouldFail()
    {
        var command = new UpdatePatientProfileCommand
        {
            UserId = Guid.NewGuid(),
            FullName = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name is required.");
    }

    [Fact]
    public void Validate_FullNameTooLong_ShouldFail()
    {
        var command = new UpdatePatientProfileCommand
        {
            UserId = Guid.NewGuid(),
            FullName = new string('A', 201)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_PhoneTooLong_ShouldFail()
    {
        var command = new UpdatePatientProfileCommand
        {
            UserId = Guid.NewGuid(),
            FullName = "John",
            Phone = new string('1', 21)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone number must not exceed 20 characters.");
    }

    [Fact]
    public void Validate_InvalidDateOfBirth_ShouldFail()
    {
        var command = new UpdatePatientProfileCommand
        {
            UserId = Guid.NewGuid(),
            FullName = "John",
            DateOfBirth = "not-a-date"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Invalid date of birth format.");
    }

    [Fact]
    public void Validate_InvalidGender_ShouldFail()
    {
        var command = new UpdatePatientProfileCommand
        {
            UserId = Guid.NewGuid(),
            FullName = "John",
            Gender = "invalid"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Gender)
            .WithErrorMessage("Gender must be 'male', 'female', or 'other'.");
    }

    [Theory]
    [InlineData("male")]
    [InlineData("female")]
    [InlineData("other")]
    [InlineData("prefernottotsay")]
    public void Validate_ValidGender_ShouldPass(string gender)
    {
        var command = new UpdatePatientProfileCommand
        {
            UserId = Guid.NewGuid(),
            FullName = "John",
            Gender = gender
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Gender);
    }

    [Fact]
    public void Validate_AddressTooLong_ShouldFail()
    {
        var command = new UpdatePatientProfileCommand
        {
            UserId = Guid.NewGuid(),
            FullName = "John",
            Address = new string('A', 501)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("Address must not exceed 500 characters.");
    }

    [Fact]
    public void Validate_NullOptionalFields_ShouldPass()
    {
        var command = new UpdatePatientProfileCommand
        {
            UserId = Guid.NewGuid(),
            FullName = "John Doe",
            Phone = null,
            DateOfBirth = null,
            Gender = null,
            Address = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
