using Domain.Entities.Screening;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class RetinalImageTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldCreateImage()
    {
        var capturedAt = DateTime.UtcNow;
        var image = new RetinalImage(Guid.NewGuid(), "https://img.local/1.png", EyeSide.Left, capturedAt, "device", 80m);

        image.ImageUrl.Should().Be("https://img.local/1.png");
        image.EyeSide.Should().Be(EyeSide.Left);
        image.CapturedAt.Should().Be(capturedAt);
        image.DeviceName.Should().Be("device");
        image.QualityScore.Should().Be(80m);
    }

    [Fact]
    public void Constructor_EmptyImageUrl_ShouldThrow()
    {
        var act = () => new RetinalImage(Guid.NewGuid(), string.Empty, EyeSide.Right, DateTime.UtcNow);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Image URL cannot be empty*");
    }

    [Fact]
    public void AssignToScreening_ShouldSetScreeningId()
    {
        var image = new RetinalImage(Guid.NewGuid(), "https://img.local/1.png", EyeSide.Right, DateTime.UtcNow);
        var screeningId = Guid.NewGuid();

        image.AssignToScreening(screeningId);

        image.AiScreeningId.Should().Be(screeningId);
    }

    [Fact]
    public void UpdateQualityScore_ValidInput_ShouldUpdateScore()
    {
        var image = new RetinalImage(Guid.NewGuid(), "https://img.local/1.png", EyeSide.Right, DateTime.UtcNow);

        image.UpdateQualityScore(92m);

        image.QualityScore.Should().Be(92m);
    }
}