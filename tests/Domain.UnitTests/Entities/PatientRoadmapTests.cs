using Domain.Entities.Screening;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class PatientRoadmapTests
{
    private readonly Guid _patientId = Guid.NewGuid();
    private readonly Guid _diagnosisId = Guid.NewGuid();

    private PatientRoadmap CreateValidRoadmap(
        string riskLevel = "Moderate",
        string summary = "Summary text",
        string source = "AI")
    {
        return new PatientRoadmap(
            _patientId,
            _diagnosisId,
            riskLevel,
            summary,
            "[\"step1\"]",
            "[\"advice1\"]",
            "[\"sign1\"]",
            true,
            "3 months",
            "{\"raw\":true}",
            source,
            DateTime.UtcNow);
    }

    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateRoadmap()
    {
        var generatedAt = DateTime.UtcNow;

        var roadmap = new PatientRoadmap(
            _patientId,
            _diagnosisId,
            " High ",
            " Critical summary ",
            "[\"step\"]",
            "[\"advice\"]",
            "[\"sign\"]",
            true,
            " 1 month ",
            null,
            " AI ",
            generatedAt);

        roadmap.PatientId.Should().Be(_patientId);
        roadmap.MedicalDiagnosisId.Should().Be(_diagnosisId);
        roadmap.RiskLevel.Should().Be("High");
        roadmap.Summary.Should().Be("Critical summary");
        roadmap.NextStepsJson.Should().Be("[\"step\"]");
        roadmap.LifestyleAdviceJson.Should().Be("[\"advice\"]");
        roadmap.WarningSignsJson.Should().Be("[\"sign\"]");
        roadmap.FollowUpNeeded.Should().BeTrue();
        roadmap.FollowUpTimeframe.Should().Be("1 month");
        roadmap.RawAiResponse.Should().BeNull();
        roadmap.Source.Should().Be("AI");
        roadmap.GeneratedAt.Should().Be(generatedAt);
    }

    [Fact]
    public void Constructor_EmptyPatientId_ShouldThrow()
    {
        var act = () => new PatientRoadmap(
            Guid.Empty, _diagnosisId, "High", "Summary",
            "[\"s\"]", "[\"a\"]", "[\"w\"]", false, "", null, "AI", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("patientId");
    }

    [Fact]
    public void Constructor_EmptyDiagnosisId_ShouldThrow()
    {
        var act = () => new PatientRoadmap(
            _patientId, Guid.Empty, "High", "Summary",
            "[\"s\"]", "[\"a\"]", "[\"w\"]", false, "", null, "AI", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("medicalDiagnosisId");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyRiskLevel_ShouldThrow(string? riskLevel)
    {
        var act = () => new PatientRoadmap(
            _patientId, _diagnosisId, riskLevel!, "Summary",
            "[\"s\"]", "[\"a\"]", "[\"w\"]", false, "", null, "AI", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("riskLevel");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptySummary_ShouldThrow(string? summary)
    {
        var act = () => new PatientRoadmap(
            _patientId, _diagnosisId, "High", summary!,
            "[\"s\"]", "[\"a\"]", "[\"w\"]", false, "", null, "AI", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("summary");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyNextStepsJson_ShouldThrow(string? nextSteps)
    {
        var act = () => new PatientRoadmap(
            _patientId, _diagnosisId, "High", "Summary",
            nextSteps!, "[\"a\"]", "[\"w\"]", false, "", null, "AI", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("nextStepsJson");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyLifestyleAdviceJson_ShouldThrow(string? advice)
    {
        var act = () => new PatientRoadmap(
            _patientId, _diagnosisId, "High", "Summary",
            "[\"s\"]", advice!, "[\"w\"]", false, "", null, "AI", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("lifestyleAdviceJson");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyWarningSignsJson_ShouldThrow(string? signs)
    {
        var act = () => new PatientRoadmap(
            _patientId, _diagnosisId, "High", "Summary",
            "[\"s\"]", "[\"a\"]", signs!, false, "", null, "AI", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("warningSignsJson");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptySource_ShouldThrow(string? source)
    {
        var act = () => new PatientRoadmap(
            _patientId, _diagnosisId, "High", "Summary",
            "[\"s\"]", "[\"a\"]", "[\"w\"]", false, "", null, source!, DateTime.UtcNow);

        act.Should().Throw<ArgumentException>().WithParameterName("source");
    }

    #endregion

    #region OverrideByDoctor

    [Fact]
    public void OverrideByDoctor_ValidInput_ShouldUpdateFields()
    {
        var roadmap = CreateValidRoadmap();

        roadmap.OverrideByDoctor(
            " Low ",
            " Doctor summary ",
            "[\"new-step\"]",
            "[\"new-advice\"]",
            "[\"new-sign\"]",
            false,
            " 6 months ");

        roadmap.RiskLevel.Should().Be("Low");
        roadmap.Summary.Should().Be("Doctor summary");
        roadmap.NextStepsJson.Should().Be("[\"new-step\"]");
        roadmap.LifestyleAdviceJson.Should().Be("[\"new-advice\"]");
        roadmap.WarningSignsJson.Should().Be("[\"new-sign\"]");
        roadmap.FollowUpNeeded.Should().BeFalse();
        roadmap.FollowUpTimeframe.Should().Be("6 months");
        roadmap.Source.Should().Be("DOCTOR_OVERRIDE");
        roadmap.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void OverrideByDoctor_EmptyRiskLevel_ShouldThrow(string? riskLevel)
    {
        var roadmap = CreateValidRoadmap();

        var act = () => roadmap.OverrideByDoctor(riskLevel!, "S", "[\"s\"]", "[\"a\"]", "[\"w\"]", false, "");

        act.Should().Throw<ArgumentException>().WithParameterName("riskLevel");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void OverrideByDoctor_EmptySummary_ShouldThrow(string? summary)
    {
        var roadmap = CreateValidRoadmap();

        var act = () => roadmap.OverrideByDoctor("High", summary!, "[\"s\"]", "[\"a\"]", "[\"w\"]", false, "");

        act.Should().Throw<ArgumentException>().WithParameterName("summary");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void OverrideByDoctor_EmptyNextStepsJson_ShouldThrow(string? nextSteps)
    {
        var roadmap = CreateValidRoadmap();

        var act = () => roadmap.OverrideByDoctor("High", "S", nextSteps!, "[\"a\"]", "[\"w\"]", false, "");

        act.Should().Throw<ArgumentException>().WithParameterName("nextStepsJson");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void OverrideByDoctor_EmptyLifestyleAdviceJson_ShouldThrow(string? advice)
    {
        var roadmap = CreateValidRoadmap();

        var act = () => roadmap.OverrideByDoctor("High", "S", "[\"s\"]", advice!, "[\"w\"]", false, "");

        act.Should().Throw<ArgumentException>().WithParameterName("lifestyleAdviceJson");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void OverrideByDoctor_EmptyWarningSignsJson_ShouldThrow(string? signs)
    {
        var roadmap = CreateValidRoadmap();

        var act = () => roadmap.OverrideByDoctor("High", "S", "[\"s\"]", "[\"a\"]", signs!, false, "");

        act.Should().Throw<ArgumentException>().WithParameterName("warningSignsJson");
    }

    #endregion
}
