using System.Net;
using System.Text;
using System.Text.Json;
using Application.PatientRoadmaps.Common;
using FluentAssertions;
using Infrastructure.Services;
using Infrastructure.Settings;
using Infrastructure.UnitTests.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.UnitTests.Services;

public class PatientRoadmapGenerationServiceTests
{
    [Fact]
    public async Task GenerateFromDiagnosisAsync_WhenPatientIdEmpty_ShouldFail()
    {
        var service = CreateService("{}", hasApiKey: true);
        var input = CreateValidInput();
        input = new PatientRoadmapGenerationInput
        {
            PatientId = Guid.Empty,
            ScreeningId = input.ScreeningId,
            AiScreeningRawJson = input.AiScreeningRawJson
        };

        var result = await service.GenerateFromDiagnosisAsync(input);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Patient ID is required for roadmap generation.");
    }

    [Fact]
    public async Task GenerateFromDiagnosisAsync_WhenScreeningIdEmpty_ShouldFail()
    {
        var service = CreateService("{}", hasApiKey: true);
        var input = CreateValidInput();
        input = new PatientRoadmapGenerationInput
        {
            PatientId = input.PatientId,
            ScreeningId = Guid.Empty,
            AiScreeningRawJson = input.AiScreeningRawJson
        };

        var result = await service.GenerateFromDiagnosisAsync(input);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Screening ID is required for roadmap generation.");
    }

    [Fact]
    public async Task GenerateFromDiagnosisAsync_WhenAiScreeningMissing_ShouldFail()
    {
        var service = CreateService("{}", hasApiKey: true);
        var input = CreateValidInput();
        input = new PatientRoadmapGenerationInput
        {
            PatientId = input.PatientId,
            ScreeningId = input.ScreeningId,
            AiScreeningRawJson = " "
        };

        var result = await service.GenerateFromDiagnosisAsync(input);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("AI screening result is required for roadmap generation.");
    }

    [Fact]
    public async Task GenerateFromDiagnosisAsync_WhenApiKeyMissing_ShouldFail()
    {
        var service = CreateService("{}", hasApiKey: false);
        var input = CreateValidInput();

        var result = await service.GenerateFromDiagnosisAsync(input);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Google AI Studio API key is not configured.");
    }

    [Theory]
    [InlineData("LOW")]
    [InlineData("MEDIUM")]
    [InlineData("HIGH")]
    [InlineData("CRITICAL")]
    [InlineData("low")]
    [InlineData("medium")]
    [InlineData("high")]
    [InlineData("critical")]
    public async Task GenerateFromDiagnosisAsync_WithValidRiskLevels_ShouldNormalizeAndSucceed(string riskLevel)
    {
        var payload = BuildGeminiResponse("""
        {
          "risk_level":"RISK",
          "summary":"Summary",
          "next_steps":["A","B","A"],
          "lifestyle_advice":["L1","L2"],
          "follow_up":{"needed":true,"timeframe":"2 weeks"},
          "warning_signs":["W1","W2"]
        }
        """.Replace("RISK", riskLevel));
        var service = CreateService(payload, hasApiKey: true);

        var result = await service.GenerateFromDiagnosisAsync(CreateValidInput());

        result.IsSuccess.Should().BeTrue();
        result.Data!.RiskLevel.Should().Be(riskLevel.ToUpperInvariant());
        result.Data.NextSteps.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("{\"summary\":\"ok\"}")]
    [InlineData("{\"risk_level\":\"LOW\"}")]
    [InlineData("{\"risk_level\":\"LOW\",\"summary\":\"ok\",\"next_steps\":null,\"lifestyle_advice\":[],\"follow_up\":{\"needed\":false,\"timeframe\":\"\"},\"warning_signs\":[]}")]
    [InlineData("{\"risk_level\":\"LOW\",\"summary\":\"ok\",\"next_steps\":[],\"lifestyle_advice\":null,\"follow_up\":{\"needed\":false,\"timeframe\":\"\"},\"warning_signs\":[]}")]
    [InlineData("{\"risk_level\":\"LOW\",\"summary\":\"ok\",\"next_steps\":[],\"lifestyle_advice\":[],\"follow_up\":null,\"warning_signs\":[]}")]
    [InlineData("{\"risk_level\":\"INVALID\",\"summary\":\"ok\",\"next_steps\":[],\"lifestyle_advice\":[],\"follow_up\":{\"needed\":false,\"timeframe\":\"\"},\"warning_signs\":[]}")]
    public async Task GenerateFromDiagnosisAsync_WithInvalidModelPayload_ShouldFail(string aiJson)
    {
        var service = CreateService(BuildGeminiResponse(aiJson), hasApiKey: true);

        var result = await service.GenerateFromDiagnosisAsync(CreateValidInput());

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("AI returned an invalid roadmap format after retries.");
    }

    [Fact]
    public async Task GenerateFromDiagnosisAsync_WithHttpFailure_ShouldFailGracefully()
    {
        var service = CreateService("server down", hasApiKey: true, statusCode: HttpStatusCode.InternalServerError);

        var result = await service.GenerateFromDiagnosisAsync(CreateValidInput());

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Unable to generate patient roadmap from AI at this time.");
    }

    private static PatientRoadmapGenerationInput CreateValidInput()
        => new()
        {
            PatientId = Guid.NewGuid(),
            ScreeningId = Guid.NewGuid(),
            AiScreeningRawJson = "{\"x\":1}",
            DiagnosisCode = "H35",
            ClinicalFindings = "finding"
        };

    private static string BuildGeminiResponse(string innerJson)
    {
        var payload = new
        {
            candidates = new[]
            {
                new
                {
                    content = new
                    {
                        parts = new[]
                        {
                            new { text = innerJson }
                        }
                    }
                }
            }
        };
        return JsonSerializer.Serialize(payload);
    }

    private static PatientRoadmapGenerationService CreateService(
        string responseContent,
        bool hasApiKey,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var settings = new GoogleAiStudioSettings
        {
            ApiKey = hasApiKey ? "api-key" : "",
            MaxRetries = 0,
            InitialBackoffMs = 1,
            TimeoutSeconds = 5
        };
        var logger = new TestLogger<PatientRoadmapGenerationService>();
        var client = new HttpClient(new StubHandler(responseContent, statusCode));
        var factory = new StubHttpClientFactory(client);
        return new PatientRoadmapGenerationService(factory, Options.Create(settings), logger);
    }

    private sealed class StubHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class StubHandler(string body, HttpStatusCode statusCode) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
    }
}
