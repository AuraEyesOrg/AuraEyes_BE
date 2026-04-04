using Application.Screenings.Commands.CreateAiScreeningSession;
using Domain.Enums;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateAiScreeningSessionCommandValidatorTests
{
    private readonly CreateAiScreeningSessionCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateAiScreeningSessionCommand
        {
            RetinalImages = new List<RetinalImageData>
            {
                new() { ImageUrl = "https://storage.example.com/img1.jpg", EyeSide = EyeSide.Left }
            }
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ValidCommandWithMultipleImages_ShouldPass()
    {
        var command = new CreateAiScreeningSessionCommand
        {
            RetinalImages = new List<RetinalImageData>
            {
                new() { ImageUrl = "https://storage.example.com/left.jpg", EyeSide = EyeSide.Left },
                new() { ImageUrl = "https://storage.example.com/right.jpg", EyeSide = EyeSide.Right }
            }
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyRetinalImages_ShouldFail()
    {
        var command = new CreateAiScreeningSessionCommand
        {
            RetinalImages = new List<RetinalImageData>()
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RetinalImages)
            .WithErrorMessage("At least one retinal image is required.");
    }

    [Fact]
    public void Validate_ImageWithEmptyUrl_ShouldFail()
    {
        var command = new CreateAiScreeningSessionCommand
        {
            RetinalImages = new List<RetinalImageData>
            {
                new() { ImageUrl = "", EyeSide = EyeSide.Left }
            }
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("Image URL cannot be empty.");
    }

    [Fact]
    public void Validate_ImageWithWhitespaceUrl_ShouldFail()
    {
        var command = new CreateAiScreeningSessionCommand
        {
            RetinalImages = new List<RetinalImageData>
            {
                new() { ImageUrl = "   ", EyeSide = EyeSide.Left }
            }
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("Image URL cannot be empty.");
    }

    [Fact]
    public void Validate_ImageWithZeroEyeSide_ShouldFail()
    {
        var command = new CreateAiScreeningSessionCommand
        {
            RetinalImages = new List<RetinalImageData>
            {
                new() { ImageUrl = "https://storage.example.com/img.jpg", EyeSide = 0 }
            }
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("EyeSide must be specified.");
    }

    [Theory]
    [InlineData(EyeSide.Left)]
    [InlineData(EyeSide.Right)]
    [InlineData(EyeSide.Both)]
    public void Validate_ValidEyeSideValues_ShouldPass(EyeSide eyeSide)
    {
        var command = new CreateAiScreeningSessionCommand
        {
            RetinalImages = new List<RetinalImageData>
            {
                new() { ImageUrl = "https://storage.example.com/img.jpg", EyeSide = eyeSide }
            }
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
