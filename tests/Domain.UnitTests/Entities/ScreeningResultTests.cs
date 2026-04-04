using Domain.Entities.Screening;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ScreeningResultTests
{
    [Fact]
    public void Constructor_ValidInput_ShouldCreateResult()
    {
        var result = new ScreeningResult(Guid.NewGuid(), RiskLevel.Moderate, 87.5m, "summary", "findings");

        result.RiskLevel.Should().Be(RiskLevel.Moderate);
        result.ConfidenceScore.Should().Be(87.5m);
        result.Summary.Should().Be("summary");
        result.Findings.Should().Be("findings");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_InvalidConfidenceScore_ShouldThrow(decimal score)
    {
        var act = () => new ScreeningResult(Guid.NewGuid(), RiskLevel.Low, score);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Confidence score must be between 0 and 100*");
    }

    [Fact]
    public void UpdateFindings_ShouldSetText()
    {
        var result = new ScreeningResult(Guid.NewGuid(), RiskLevel.Low, 50m);

        result.UpdateFindings("new summary", "new findings");

        result.Summary.Should().Be("new summary");
        result.Findings.Should().Be("new findings");
    }

    [Fact]
    public void UpdateRiskLevel_ShouldUpdateLevelAndConfidence()
    {
        var result = new ScreeningResult(Guid.NewGuid(), RiskLevel.Low, 50m);

        result.UpdateRiskLevel(RiskLevel.Critical, 99m);

        result.RiskLevel.Should().Be(RiskLevel.Critical);
        result.ConfidenceScore.Should().Be(99m);
    }
}