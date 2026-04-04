using Application.Feedback.Commands.CreateOrganisationFeedback;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Validators;

public class CreateOrganisationFeedbackCommandValidatorTests
{
    private readonly CreateOrganisationFeedbackCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateOrganisationFeedbackCommand
        {
            OrganisationId = Guid.NewGuid(),
            AppointmentId = Guid.NewGuid(),
            Rating = 4,
            Comment = "Good experience"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NullComment_ShouldPass()
    {
        var command = new CreateOrganisationFeedbackCommand
        {
            OrganisationId = Guid.NewGuid(),
            AppointmentId = Guid.NewGuid(),
            Rating = 3,
            Comment = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyOrganisationId_ShouldFail()
    {
        var command = new CreateOrganisationFeedbackCommand
        {
            OrganisationId = Guid.Empty,
            AppointmentId = Guid.NewGuid(),
            Rating = 4
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.OrganisationId)
            .WithErrorMessage("Organisation ID is required.");
    }

    [Fact]
    public void Validate_EmptyAppointmentId_ShouldFail()
    {
        var command = new CreateOrganisationFeedbackCommand
        {
            OrganisationId = Guid.NewGuid(),
            AppointmentId = Guid.Empty,
            Rating = 4
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AppointmentId)
            .WithErrorMessage("Appointment ID is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Validate_RatingOutOfRange_ShouldFail(int rating)
    {
        var command = new CreateOrganisationFeedbackCommand
        {
            OrganisationId = Guid.NewGuid(),
            AppointmentId = Guid.NewGuid(),
            Rating = rating
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
        var command = new CreateOrganisationFeedbackCommand
        {
            OrganisationId = Guid.NewGuid(),
            AppointmentId = Guid.NewGuid(),
            Rating = rating
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void Validate_CommentExceeds2000Characters_ShouldFail()
    {
        var command = new CreateOrganisationFeedbackCommand
        {
            OrganisationId = Guid.NewGuid(),
            AppointmentId = Guid.NewGuid(),
            Rating = 4,
            Comment = new string('A', 2001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Comment)
            .WithErrorMessage("Comment cannot exceed 2000 characters.");
    }

    [Fact]
    public void Validate_CommentExactly2000Characters_ShouldPass()
    {
        var command = new CreateOrganisationFeedbackCommand
        {
            OrganisationId = Guid.NewGuid(),
            AppointmentId = Guid.NewGuid(),
            Rating = 4,
            Comment = new string('A', 2000)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
