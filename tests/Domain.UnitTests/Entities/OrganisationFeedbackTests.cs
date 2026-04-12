using Domain.Entities.Consultation;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class OrganisationFeedbackTests
{
    private readonly Guid _patientId = Guid.NewGuid();
    private readonly Guid _organisationId = Guid.NewGuid();
    private readonly Guid _appointmentId = Guid.NewGuid();

    [Fact]
    public void Constructor_ValidInput_ShouldCreateFeedback()
    {
        var feedback = new OrganisationFeedback(
            _patientId, _organisationId, _appointmentId, 4, "Great service");

        feedback.PatientId.Should().Be(_patientId);
        feedback.OrganisationId.Should().Be(_organisationId);
        feedback.AppointmentId.Should().Be(_appointmentId);
        feedback.Rating.Should().Be(4);
        feedback.Comment.Should().Be("Great service");
        feedback.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_NullComment_ShouldSetCommentToNull()
    {
        var feedback = new OrganisationFeedback(
            _patientId, _organisationId, _appointmentId, 5, null);

        feedback.Comment.Should().BeNull();
    }

    [Fact]
    public void Constructor_WhitespaceComment_ShouldSetCommentToNull()
    {
        var feedback = new OrganisationFeedback(
            _patientId, _organisationId, _appointmentId, 3, "   ");

        feedback.Comment.Should().BeNull();
    }

    [Fact]
    public void Constructor_CommentWithLeadingTrailingSpaces_ShouldTrim()
    {
        var feedback = new OrganisationFeedback(
            _patientId, _organisationId, _appointmentId, 5, "  Nice place  ");

        feedback.Comment.Should().Be("Nice place");
    }

    [Fact]
    public void Constructor_EmptyPatientId_ShouldThrow()
    {
        var act = () => new OrganisationFeedback(
            Guid.Empty, _organisationId, _appointmentId, 4, "Good");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("patientId");
    }

    [Fact]
    public void Constructor_EmptyOrganisationId_ShouldThrow()
    {
        var act = () => new OrganisationFeedback(
            _patientId, Guid.Empty, _appointmentId, 4, "Good");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("organisationId");
    }

    [Fact]
    public void Constructor_EmptyAppointmentId_ShouldThrow()
    {
        var act = () => new OrganisationFeedback(
            _patientId, _organisationId, Guid.Empty, 4, "Good");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("appointmentId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void Constructor_RatingOutOfRange_ShouldThrow(int invalidRating)
    {
        var act = () => new OrganisationFeedback(
            _patientId, _organisationId, _appointmentId, invalidRating, "Comment");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("rating");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Constructor_ValidRatingBoundaries_ShouldCreate(int validRating)
    {
        var feedback = new OrganisationFeedback(
            _patientId, _organisationId, _appointmentId, validRating, null);

        feedback.Rating.Should().Be(validRating);
    }

    [Fact]
    public void Constructor_EmptyStringComment_ShouldSetNull()
    {
        var feedback = new OrganisationFeedback(
            _patientId, _organisationId, _appointmentId, 3, "");

        feedback.Comment.Should().BeNull();
    }
}
