using Application.Feedback.Commands.CreateWebsiteFeedback;
using Domain.Enums;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateWebsiteFeedbackCommandValidatorTests
{
    private readonly CreateWebsiteFeedbackCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateWebsiteFeedbackCommand
        {
            Rating = 5,
            Category = WebsiteFeedbackCategory.OTHER,
            Comment = "Love the platform!"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NullComment_ShouldPass()
    {
        var command = new CreateWebsiteFeedbackCommand
        {
            Rating = 3,
            Category = WebsiteFeedbackCategory.OTHER,
            Comment = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Validate_RatingOutOfRange_ShouldFail(int rating)
    {
        var command = new CreateWebsiteFeedbackCommand
        {
            Rating = rating,
            Category = WebsiteFeedbackCategory.OTHER
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Rating)
            .WithErrorMessage("Rating must be between 1 and 5.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Validate_RatingBoundaryValues_ShouldPass(int rating)
    {
        var command = new CreateWebsiteFeedbackCommand
        {
            Rating = rating,
            Category = WebsiteFeedbackCategory.OTHER
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void Validate_CommentExceeds2000Characters_ShouldFail()
    {
        var command = new CreateWebsiteFeedbackCommand
        {
            Rating = 4,
            Category = WebsiteFeedbackCategory.OTHER,
            Comment = new string('A', 2001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Comment)
            .WithErrorMessage("Comment cannot exceed 2000 characters.");
    }

    [Fact]
    public void Validate_CommentExactly2000Characters_ShouldPass()
    {
        var command = new CreateWebsiteFeedbackCommand
        {
            Rating = 4,
            Category = WebsiteFeedbackCategory.OTHER,
            Comment = new string('A', 2000)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
