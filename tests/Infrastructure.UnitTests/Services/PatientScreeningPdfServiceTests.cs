using Application.Screenings.Interfaces;
using FluentAssertions;
using Infrastructure.Services;
using QuestPDF.Infrastructure;

namespace Infrastructure.UnitTests.Services;

public class PatientScreeningPdfServiceTests
{
    private readonly PatientScreeningPdfService _service;

    public PatientScreeningPdfServiceTests()
    {
        _service = new PatientScreeningPdfService();
    }

    [Fact]
    public void GenerateScreeningReportPdf_WithValidModel_ShouldReturnBytes()
    {
        var model = CreateValidModel();
        var result = _service.GenerateScreeningReportPdf(model);

        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GenerateScreeningReportPdf_WithMissingImages_ShouldHandleGracefully()
    {
        var model = CreateValidModel() with { OriginalImageUrls = new List<string>(), AnnotatedImageUrl = null, HeatmapImageUrl = null };

        var result = _service.GenerateScreeningReportPdf(model);

        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData("High")]
    [InlineData("Critical")]
    [InlineData("Moderate")]
    [InlineData("Low")]
    [InlineData(null)]
    public void GenerateScreeningReportPdf_WithDifferentRiskLevels_ShouldSucceed(string? riskLevel)
    {
        var model = CreateValidModel() with { RiskLevel = riskLevel };

        var result = _service.GenerateScreeningReportPdf(model);

        result.Should().NotBeNull();
    }

    [Fact]
    public void GenerateScreeningReportPdf_WithHeatmapMatrix_ShouldSucceed()
    {
        var matrix = new float[10][];
        for (int i = 0; i < 10; i++)
        {
            matrix[i] = new float[10];
            for (int j = 0; j < 10; j++) matrix[i][j] = (i + j) / 20f;
        }

        var model = CreateValidModel() with { HeatmapMatrix = matrix };

        var result = _service.GenerateScreeningReportPdf(model);

        result.Should().NotBeNull();
    }

    [Fact]
    public void GenerateScreeningReportPdf_WithManyFindings_ShouldSucceed()
    {
        var model = CreateValidModel();
        for (int i = 0; i < 20; i++)
        {
            model.AiFindingDetails.Add(new PatientAiFindingDetail { Rank = i + 2, DiseaseName = $"Disease {i}", Status = "Detected" });
        }

        var result = _service.GenerateScreeningReportPdf(model);

        result.Should().NotBeNull();
    }

    private PatientScreeningReportPdfModel CreateValidModel()
    {
        return new PatientScreeningReportPdfModel
        {
            PatientName = "Test Patient",
            PatientId = Guid.NewGuid(),
            ScreeningId = Guid.NewGuid(),
            RiskLevel = "High",
            Summary = "Test Summary",
            DiagnosisCode = "H35.30",
            CodingSystem = "ICD-10",
            SeverityLevel = "Severe",
            ConfidenceLevel = 95.5m,
            IsUrgent = true,
            IsReferralNeeded = true,
            OphthamologistFindings = "Finding notes",
            TreatmentPlan = "Plan notes",
            Recommendations = "Rec notes",
            CreatedAt = DateTime.UtcNow,
            AssessedAt = DateTime.UtcNow,
            ModelVersion = "v1.0",
            ReportedByDoctorName = "Dr. Smith",
            OriginalImageUrls = new List<string> { "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==" },
            AiFindingDetails = new List<PatientAiFindingDetail>
            {
                new() { Rank = 1, DiseaseName = "Glaucoma", Status = "Detected", ConfidencePercentage = 95.5m }
            }
        };
    }
}
