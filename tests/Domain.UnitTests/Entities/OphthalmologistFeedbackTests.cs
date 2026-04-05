using Domain.Entities.Consultation;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class OphthalmologistFeedbackTests
{
    private readonly Guid _patientId = Guid.NewGuid();
    private readonly Guid _ophthalmologistId = Guid.NewGuid();
    private readonly Guid _sessionId = Guid.NewGuid();

    [Fact]
    public void Constructor_ValidInput_ShouldCreateFeedback()
    {
        var feedback = new OphthalmologistFeedback(
            _patientId, _ophthalmologistId, _sessionId, 5, "Excellent doctor");

        feedback.PatientId.Should().Be(_patientId);
        feedback.OphthalmologistId.Should().Be(_ophthalmologistId);
        feedback.ConsultationSessionId.Should().Be(_sessionId);
        feedback.Rating.Should().Be(5);
        feedback.Comment.Should().Be("Excellent doctor");
        feedback.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_NullComment_ShouldSetCommentToNull()
    {
        var feedback = new OphthalmologistFeedback(
            _patientId, _ophthalmologistId, _sessionId, 4, null);

        feedback.Comment.Should().BeNull();
    }

    [Fact]
    public void Constructor_WhitespaceComment_ShouldSetCommentToNull()
    {
        var feedback = new OphthalmologistFeedback(
            _patientId, _ophthalmologistId, _sessionId, 3, "   ");

        feedback.Comment.Should().BeNull();
    }

    [Fact]
    public void Constructor_CommentWithSpaces_ShouldTrim()
    {
        var feedback = new OphthalmologistFeedback(
            _patientId, _ophthalmologistId, _sessionId, 4, "  Very helpful  ");

        feedback.Comment.Should().Be("Very helpful");
    }

    [Fact]
    public void Constructor_EmptyPatientId_ShouldThrow()
    {
        var act = () => new OphthalmologistFeedback(
            Guid.Empty, _ophthalmologistId, _sessionId, 4, "Good");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("patientId");
    }

    [Fact]
    public void Constructor_EmptyOphthalmologistId_ShouldThrow()
    {
        var act = () => new OphthalmologistFeedback(
            _patientId, Guid.Empty, _sessionId, 4, "Good");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("ophthalmologistId");
    }

    [Fact]
    public void Constructor_EmptyConsultationSessionId_ShouldThrow()
    {
        var act = () => new OphthalmologistFeedback(
            _patientId, _ophthalmologistId, Guid.Empty, 4, "Good");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("consultationSessionId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(99)]
    public void Constructor_RatingOutOfRange_ShouldThrow(int invalidRating)
    {
        var act = () => new OphthalmologistFeedback(
            _patientId, _ophthalmologistId, _sessionId, invalidRating, null);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("rating");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Constructor_ValidRatingBoundaries_ShouldCreate(int validRating)
    {
        var feedback = new OphthalmologistFeedback(
            _patientId, _ophthalmologistId, _sessionId, validRating, null);

        feedback.Rating.Should().Be(validRating);
    }

    [Fact]
    public void Constructor_EmptyStringComment_ShouldSetNull()
    {
        var feedback = new OphthalmologistFeedback(
            _patientId, _ophthalmologistId, _sessionId, 2, "");

        feedback.Comment.Should().BeNull();
    }
}
