using Domain.Entities.Consultation;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class WebsiteFeedbackTests
{
    private readonly Guid _patientId = Guid.NewGuid();

    [Fact]
    public void Constructor_ValidInput_ShouldCreateFeedback()
    {
        var feedback = new WebsiteFeedback(
            _patientId, 4, WebsiteFeedbackCategory.UX, "Nice design");

        feedback.PatientId.Should().Be(_patientId);
        feedback.Rating.Should().Be(4);
        feedback.Category.Should().Be(WebsiteFeedbackCategory.UX);
        feedback.Comment.Should().Be("Nice design");
        feedback.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_NullComment_ShouldSetCommentToNull()
    {
        var feedback = new WebsiteFeedback(
            _patientId, 3, WebsiteFeedbackCategory.BUG, null);

        feedback.Comment.Should().BeNull();
    }

    [Fact]
    public void Constructor_WhitespaceComment_ShouldSetCommentToNull()
    {
        var feedback = new WebsiteFeedback(
            _patientId, 3, WebsiteFeedbackCategory.OTHER, "   ");

        feedback.Comment.Should().BeNull();
    }

    [Fact]
    public void Constructor_CommentWithSpaces_ShouldTrim()
    {
        var feedback = new WebsiteFeedback(
            _patientId, 5, WebsiteFeedbackCategory.SUGGESTION, "  Add dark mode  ");

        feedback.Comment.Should().Be("Add dark mode");
    }

    [Fact]
    public void Constructor_EmptyPatientId_ShouldThrow()
    {
        var act = () => new WebsiteFeedback(
            Guid.Empty, 4, WebsiteFeedbackCategory.UX, "Good");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("patientId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Constructor_RatingOutOfRange_ShouldThrow(int invalidRating)
    {
        var act = () => new WebsiteFeedback(
            _patientId, invalidRating, WebsiteFeedbackCategory.BUG, null);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("rating");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Constructor_ValidRatingBoundaries_ShouldCreate(int validRating)
    {
        var feedback = new WebsiteFeedback(
            _patientId, validRating, WebsiteFeedbackCategory.UX, null);

        feedback.Rating.Should().Be(validRating);
    }

    [Theory]
    [InlineData(WebsiteFeedbackCategory.BUG)]
    [InlineData(WebsiteFeedbackCategory.UX)]
    [InlineData(WebsiteFeedbackCategory.SUGGESTION)]
    [InlineData(WebsiteFeedbackCategory.OTHER)]
    public void Constructor_AllCategories_ShouldCreate(WebsiteFeedbackCategory category)
    {
        var feedback = new WebsiteFeedback(_patientId, 3, category, null);

        feedback.Category.Should().Be(category);
    }

    [Fact]
    public void Constructor_EmptyStringComment_ShouldSetNull()
    {
        var feedback = new WebsiteFeedback(
            _patientId, 4, WebsiteFeedbackCategory.OTHER, "");

        feedback.Comment.Should().BeNull();
    }
}
