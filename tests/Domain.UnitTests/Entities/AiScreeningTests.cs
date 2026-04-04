using Domain.Entities.Screening;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class AiScreeningTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldCreateActiveScreening()
    {
        var screening = new AiScreening(Guid.NewGuid(), "v1.0.0");

        screening.ModelVersion.Should().Be("v1.0.0");
        screening.IsActive.Should().BeTrue();
        screening.ProcessedAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_EmptyModelVersion_ShouldThrow()
    {
        var act = () => new AiScreening(Guid.NewGuid(), string.Empty);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Model version cannot be empty*");
    }

    [Fact]
    public void Process_ShouldStoreJsonAndTimestamp()
    {
        var screening = new AiScreening(Guid.NewGuid(), "v1");

        screening.Process("{\"risk\":\"high\"}");

        screening.RawJsonOutput.Should().Be("{\"risk\":\"high\"}");
        screening.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldSetInactive()
    {
        var screening = new AiScreening(Guid.NewGuid(), "v1");

        screening.Deactivate();

        screening.IsActive.Should().BeFalse();
    }

    [Fact]
    public void AddRetinalImage_ShouldAppendToCollection()
    {
        var screening = new AiScreening(Guid.NewGuid(), "v1");
        var image = new RetinalImage(Guid.NewGuid(), "https://img.local/1.png", EyeSide.Left, DateTime.UtcNow);

        screening.AddRetinalImage(image);

        screening.RetinalImages.Should().ContainSingle().Which.Should().BeSameAs(image);
    }

    [Fact]
    public void AddScreeningResult_ShouldAppendToCollection()
    {
        var screening = new AiScreening(Guid.NewGuid(), "v1");
        var result = new ScreeningResult(Guid.NewGuid(), RiskLevel.High, 95m);

        screening.AddScreeningResult(result);

        screening.ScreeningResults.Should().ContainSingle().Which.Should().BeSameAs(result);
    }

    [Fact]
    public void RecordConsent_WhenMatchingPatient_ShouldCreateAgreedConsent()
    {
        var patientId = Guid.NewGuid();
        var screening = new AiScreening(patientId, "v1");

        screening.RecordConsent(patientId, "  Share screening data  ");

        screening.Consent.Should().NotBeNull();
        screening.HasAgreedConsent(patientId).Should().BeTrue();
        screening.Consent!.Content.Should().Be("Share screening data");
    }

    [Fact]
    public void RecordConsent_WithDifferentPatient_ShouldThrow()
    {
        var screening = new AiScreening(Guid.NewGuid(), "v1");

        var act = () => screening.RecordConsent(Guid.NewGuid(), "Share screening data");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Consent can only be recorded by the screening owner.");
    }
}