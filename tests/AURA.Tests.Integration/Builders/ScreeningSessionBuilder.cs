using Domain.Entities.Screening;
using Domain.Enums;

namespace AURA.Tests.Integration.Builders;

public sealed class ScreeningSessionBuilder
{
    private Guid _patientId = Guid.NewGuid();
    private string _modelVersion = "integration-v1";
    private string _imageUrl = "https://storage.mock/retinal-image.jpg";
    private EyeSide _eyeSide = EyeSide.Left;
    private RiskLevel _riskLevel = RiskLevel.Moderate;
    private decimal _confidence = 87m;

    public ScreeningSessionBuilder WithPatientId(Guid patientId)
    {
        _patientId = patientId;
        return this;
    }

    public ScreeningSessionBuilder WithModelVersion(string modelVersion)
    {
        _modelVersion = modelVersion;
        return this;
    }

    public ScreeningSessionBuilder WithImage(string imageUrl, EyeSide eyeSide)
    {
        _imageUrl = imageUrl;
        _eyeSide = eyeSide;
        return this;
    }

    public ScreeningSessionBuilder WithRisk(RiskLevel riskLevel, decimal confidence)
    {
        _riskLevel = riskLevel;
        _confidence = confidence;
        return this;
    }

    public (AiScreening Screening, RetinalImage Image, ScreeningResult Result) Build()
    {
        var screening = new AiScreening(_patientId, _modelVersion);
        var image = new RetinalImage(_patientId, _imageUrl, _eyeSide, DateTime.UtcNow, "FundusCam-X", 90m);
        image.AssignToScreening(screening.Id);

        var result = new ScreeningResult(
            screening.Id,
            _riskLevel,
            _confidence,
            "Automated AI finding",
            "No acute hemorrhage");

        screening.AddRetinalImage(image);
        screening.AddScreeningResult(result);
        screening.Process("{\"riskLevel\":\"medium\"}");

        return (screening, image, result);
    }
}
