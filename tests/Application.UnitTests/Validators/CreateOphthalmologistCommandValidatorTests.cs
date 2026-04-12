using Application.Ophthalmologists.Commands.CreateOphthalmologist;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateOphthalmologistCommandValidatorTests
{
    private readonly CreateOphthalmologistCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateOphthalmologistCommand
        {
            UserId = Guid.NewGuid(),
            YearsOfExperience = 10,
            Bio = "Experienced ophthalmologist"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldFail()
    {
        var command = new CreateOphthalmologistCommand { UserId = Guid.Empty, YearsOfExperience = 5 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("UserId is required.");
    }

    [Fact]
    public void Validate_NegativeYearsOfExperience_ShouldFail()
    {
        var command = new CreateOphthalmologistCommand { UserId = Guid.NewGuid(), YearsOfExperience = -1 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.YearsOfExperience)
            .WithErrorMessage("Years of experience cannot be negative.");
    }

    [Fact]
    public void Validate_YearsOfExperienceExceeds70_ShouldFail()
    {
        var command = new CreateOphthalmologistCommand { UserId = Guid.NewGuid(), YearsOfExperience = 71 };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.YearsOfExperience)
            .WithErrorMessage("Years of experience cannot exceed 70 years.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(35)]
    [InlineData(70)]
    public void Validate_YearsOfExperienceBoundaryValues_ShouldPass(int years)
    {
        var command = new CreateOphthalmologistCommand { UserId = Guid.NewGuid(), YearsOfExperience = years };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.YearsOfExperience);
    }

    [Fact]
    public void Validate_BioExceeds2000Characters_ShouldFail()
    {
        var command = new CreateOphthalmologistCommand
        {
            UserId = Guid.NewGuid(),
            YearsOfExperience = 5,
            Bio = new string('A', 2001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Bio)
            .WithErrorMessage("Bio cannot exceed 2000 characters.");
    }

    [Fact]
    public void Validate_BioExactly2000Characters_ShouldPass()
    {
        var command = new CreateOphthalmologistCommand
        {
            UserId = Guid.NewGuid(),
            YearsOfExperience = 5,
            Bio = new string('A', 2000)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Bio);
    }

    [Fact]
    public void Validate_NullBio_ShouldPass()
    {
        var command = new CreateOphthalmologistCommand
        {
            UserId = Guid.NewGuid(),
            YearsOfExperience = 5,
            Bio = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Bio);
    }
}
