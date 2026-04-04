using Application.Ophthalmologists.Commands.UpdateOphthalmologist;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class UpdateOphthalmologistCommandValidatorTests
{
    private readonly UpdateOphthalmologistCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new UpdateOphthalmologistCommand
        {
            Id = Guid.NewGuid(),
            YearsOfExperience = 10,
            Bio = "Updated bio"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ValidSelfProfileUpdate_ShouldPass()
    {
        var command = new UpdateOphthalmologistCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FullName = "Dr. John Doe",
            Phone = "0123456789",
            Address = "123 Main St",
            YearsOfExperience = 15,
            Bio = "Specialist"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyId_ShouldFail()
    {
        var command = new UpdateOphthalmologistCommand { Id = Guid.Empty, YearsOfExperience = 5 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Ophthalmologist ID is required.");
    }

    [Fact]
    public void Validate_NegativeYearsOfExperience_ShouldFail()
    {
        var command = new UpdateOphthalmologistCommand { Id = Guid.NewGuid(), YearsOfExperience = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.YearsOfExperience)
            .WithErrorMessage("Years of experience cannot be negative.");
    }

    [Fact]
    public void Validate_YearsOfExperienceExceeds70_ShouldFail()
    {
        var command = new UpdateOphthalmologistCommand { Id = Guid.NewGuid(), YearsOfExperience = 71 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.YearsOfExperience)
            .WithErrorMessage("Years of experience cannot exceed 70 years.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(70)]
    public void Validate_YearsOfExperienceBoundary_ShouldPass(int years)
    {
        var command = new UpdateOphthalmologistCommand { Id = Guid.NewGuid(), YearsOfExperience = years };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.YearsOfExperience);
    }

    [Fact]
    public void Validate_BioExceeds2000Characters_ShouldFail()
    {
        var command = new UpdateOphthalmologistCommand
        {
            Id = Guid.NewGuid(),
            YearsOfExperience = 5,
            Bio = new string('A', 2001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Bio)
            .WithErrorMessage("Bio cannot exceed 2000 characters.");
    }

    [Fact]
    public void Validate_SelfProfile_EmptyFullName_ShouldFail()
    {
        var command = new UpdateOphthalmologistCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FullName = "",
            YearsOfExperience = 5
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name is required.");
    }

    [Fact]
    public void Validate_SelfProfile_FullNameExceeds200_ShouldFail()
    {
        var command = new UpdateOphthalmologistCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FullName = new string('A', 201),
            YearsOfExperience = 5
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Full name must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_SelfProfile_PhoneExceeds20_ShouldFail()
    {
        var command = new UpdateOphthalmologistCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FullName = "Dr. Smith",
            Phone = new string('1', 21),
            YearsOfExperience = 5
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone number must not exceed 20 characters.");
    }

    [Fact]
    public void Validate_SelfProfile_AddressExceeds500_ShouldFail()
    {
        var command = new UpdateOphthalmologistCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FullName = "Dr. Smith",
            Address = new string('A', 501),
            YearsOfExperience = 5
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("Address must not exceed 500 characters.");
    }

    [Fact]
    public void Validate_NoUserIdSet_FullNameNotRequired()
    {
        var command = new UpdateOphthalmologistCommand
        {
            Id = Guid.NewGuid(),
            UserId = null,
            FullName = null,
            YearsOfExperience = 5
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.FullName);
    }
}
