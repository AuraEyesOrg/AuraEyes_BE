using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.PatientRoadmaps.Common;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class PatientRoadmapGenerationService : IPatientRoadmapGenerationService
{
    private static readonly HashSet<string> AllowedRiskLevels =
    [
        "LOW",
        "MEDIUM",
        "HIGH",
        "CRITICAL"
    ];

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleAiStudioSettings _settings;
    private readonly ILogger<PatientRoadmapGenerationService> _logger;

    public PatientRoadmapGenerationService(
        IHttpClientFactory httpClientFactory,
        IOptions<GoogleAiStudioSettings> settings,
        ILogger<PatientRoadmapGenerationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<Result<GeneratedPatientRoadmap>> GenerateFromDiagnosisAsync(
        PatientRoadmapGenerationInput input,
        CancellationToken cancellationToken = default)
    {
        if (input.PatientId == Guid.Empty)
            return Result<GeneratedPatientRoadmap>.Failure("Patient ID is required for roadmap generation.");

        if (input.ScreeningId == Guid.Empty)
            return Result<GeneratedPatientRoadmap>.Failure("Screening ID is required for roadmap generation.");

        if (string.IsNullOrWhiteSpace(input.AiScreeningRawJson))
            return Result<GeneratedPatientRoadmap>.Failure("AI screening result is required for roadmap generation.");

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            return Result<GeneratedPatientRoadmap>.Failure("Google AI Studio API key is not configured.");

        var prompt = BuildPrompt(input);
        var maxAttempts = Math.Max(1, _settings.MaxRetries + 1);
        var delayMs = Math.Max(200, _settings.InitialBackoffMs);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var modelResponse = await CallGoogleAiAsync(prompt, cancellationToken);
                var normalizedJson = NormalizeJsonPayload(modelResponse);

                if (!TryParseAndValidateRoadmap(normalizedJson, out var roadmap, out var validationError))
                {
                    _logger.LogWarning(
                        "Invalid patient roadmap AI response at attempt {Attempt}/{MaxAttempts}: {ValidationError}",
                        attempt,
                        maxAttempts,
                        validationError);

                    if (attempt == maxAttempts)
                    {
                        return Result<GeneratedPatientRoadmap>.Failure(
                            "AI returned an invalid roadmap format after retries.");
                    }
                }
                else
                {
                    _logger.LogInformation(
                        "Generated patient roadmap for patient {PatientId} from screening {ScreeningId}",
                        input.PatientId,
                        input.ScreeningId);

                    return Result<GeneratedPatientRoadmap>.Success(roadmap!);
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or InvalidOperationException)
            {
                _logger.LogWarning(
                    ex,
                    "Patient roadmap generation attempt {Attempt}/{MaxAttempts} failed",
                    attempt,
                    maxAttempts);

                if (attempt == maxAttempts)
                {
                    return Result<GeneratedPatientRoadmap>.Failure(
                        "Unable to generate patient roadmap from AI at this time.");
                }
            }

            await Task.Delay(delayMs, cancellationToken);
            delayMs *= 2;
        }

        return Result<GeneratedPatientRoadmap>.Failure("Unable to generate patient roadmap.");
    }

    private async Task<string> CallGoogleAiAsync(string prompt, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(Math.Max(5, _settings.TimeoutSeconds)));

        var endpoint =
            $"{_settings.BaseUrl.TrimEnd('/')}/{_settings.Model}:generateContent?key={Uri.EscapeDataString(_settings.ApiKey)}";

        var request = new GeminiGenerateRequest
        {
            Contents =
            [
                new GeminiContent
                {
                    Parts =
                    [
                        new GeminiPart
                        {
                            Text = prompt
                        }
                    ]
                }
            ],
            GenerationConfig = new GeminiGenerationConfig
            {
                Temperature = 0.2m,
                ResponseMimeType = "application/json"
            }
        };

        using var response = await client.PostAsJsonAsync(endpoint, request, timeoutCts.Token);
        var body = await response.Content.ReadAsStringAsync(timeoutCts.Token);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Google AI Studio request failed with status {(int)response.StatusCode}: {Truncate(body, 500)}");
        }

        var parsed = JsonSerializer.Deserialize<GeminiGenerateResponse>(body);
        var text = parsed?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("Google AI Studio returned an empty roadmap payload.");

        return text;
    }

    private static string BuildPrompt(PatientRoadmapGenerationInput input)
    {
        var diagnosisContext = new
        {
            diagnosis_code = input.DiagnosisCode,
            coding_system = input.CodingSystem,
            clinical_findings = input.ClinicalFindings,
            severity_level = input.SeverityLevel,
            confidence_level = input.ConfidenceLevel,
            treatment_plan = input.TreatmentPlan,
            recommendations = input.Recommendations,
            lifestyle_advice = input.LifestyleAdvice,
            is_urgent = input.IsUrgent,
            status = input.Status,
            follow_up_date = input.FollowUpDate,
            is_referral_needed = input.IsReferralNeeded
        };

        var diagnosisJson = JsonSerializer.Serialize(diagnosisContext);

        var builder = new StringBuilder();
        builder.AppendLine("You are a medical assistant AI.");
        builder.AppendLine();
        builder.AppendLine("Based on:");
        builder.AppendLine();
        builder.AppendLine("* AI screening result");
        builder.AppendLine("* Doctor diagnosis");
        builder.AppendLine();
        builder.AppendLine("Generate a patient-friendly roadmap.");
        builder.AppendLine();
        builder.AppendLine("STRICT RULES:");
        builder.AppendLine();
        builder.AppendLine("* Output MUST be valid JSON");
        builder.AppendLine("* No extra text outside JSON");
        builder.AppendLine("* Use EXACT field names");
        builder.AppendLine();
        builder.AppendLine("JSON FORMAT:");
        builder.AppendLine();
        builder.AppendLine("{");
        builder.AppendLine("\"risk_level\": \"LOW | MEDIUM | HIGH | CRITICAL\",");
        builder.AppendLine("\"summary\": \"short explanation\",");
        builder.AppendLine("\"next_steps\": [\"step 1\", \"step 2\"],");
        builder.AppendLine("\"lifestyle_advice\": [\"advice 1\", \"advice 2\"],");
        builder.AppendLine("\"follow_up\": {");
        builder.AppendLine("\"needed\": true,");
        builder.AppendLine("\"timeframe\": \"string\"");
        builder.AppendLine("},");
        builder.AppendLine("\"warning_signs\": [\"symptom 1\", \"symptom 2\"]");
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine("AI screening result:");
        builder.AppendLine(input.AiScreeningRawJson);
        builder.AppendLine();
        builder.AppendLine("Doctor diagnosis:");
        builder.AppendLine(diagnosisJson);

        return builder.ToString();
    }

    private static string NormalizeJsonPayload(string payload)
    {
        var trimmed = payload.Trim();

        if (!trimmed.StartsWith("```", StringComparison.Ordinal))
            return trimmed;

        var lines = trimmed.Split('\n');
        if (lines.Length < 3)
            return trimmed;

        var jsonLines = lines
            .Skip(1)
            .Take(lines.Length - 2)
            .ToArray();

        return string.Join("\n", jsonLines).Trim();
    }

    private static bool TryParseAndValidateRoadmap(
        string json,
        out GeneratedPatientRoadmap? roadmap,
        out string error)
    {
        roadmap = null;

        AiRoadmapResponse? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<AiRoadmapResponse>(json);
        }
        catch (JsonException ex)
        {
            error = $"Invalid JSON from AI: {ex.Message}";
            return false;
        }

        if (parsed is null)
        {
            error = "AI response is empty.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(parsed.RiskLevel))
        {
            error = "Missing required field 'risk_level'.";
            return false;
        }

        var normalizedRiskLevel = parsed.RiskLevel.Trim().ToUpperInvariant();
        if (!AllowedRiskLevels.Contains(normalizedRiskLevel))
        {
            error = $"Invalid risk_level '{parsed.RiskLevel}'.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(parsed.Summary))
        {
            error = "Missing required field 'summary'.";
            return false;
        }

        if (parsed.NextSteps is null)
        {
            error = "Field 'next_steps' cannot be null.";
            return false;
        }

        if (parsed.LifestyleAdvice is null)
        {
            error = "Field 'lifestyle_advice' cannot be null.";
            return false;
        }

        if (parsed.WarningSigns is null)
        {
            error = "Field 'warning_signs' cannot be null.";
            return false;
        }

        if (parsed.FollowUp is null)
        {
            error = "Field 'follow_up' is required.";
            return false;
        }

        var followUpTimeframe = parsed.FollowUp.Timeframe?.Trim() ?? string.Empty;
        if (parsed.FollowUp.Needed && string.IsNullOrWhiteSpace(followUpTimeframe))
        {
            error = "Field 'follow_up.timeframe' is required when follow-up is needed.";
            return false;
        }

        roadmap = new GeneratedPatientRoadmap
        {
            RiskLevel = normalizedRiskLevel,
            Summary = parsed.Summary.Trim(),
            NextSteps = NormalizeStringList(parsed.NextSteps),
            LifestyleAdvice = NormalizeStringList(parsed.LifestyleAdvice),
            WarningSigns = NormalizeStringList(parsed.WarningSigns),
            FollowUpNeeded = parsed.FollowUp.Needed,
            FollowUpTimeframe = followUpTimeframe,
            RawAiResponse = json
        };

        error = string.Empty;
        return true;
    }

    private static IReadOnlyList<string> NormalizeStringList(IEnumerable<string> values)
    {
        return values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string Truncate(string value, int maxLength)
    {
        if (value.Length <= maxLength)
            return value;

        return value[..maxLength];
    }

    private sealed class GeminiGenerateRequest
    {
        [JsonPropertyName("contents")]
        public List<GeminiContent> Contents { get; set; } = [];

        [JsonPropertyName("generationConfig")]
        public GeminiGenerationConfig GenerationConfig { get; set; } = new();
    }

    private sealed class GeminiContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiPart> Parts { get; set; } = [];
    }

    private sealed class GeminiPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    private sealed class GeminiGenerationConfig
    {
        [JsonPropertyName("temperature")]
        public decimal Temperature { get; set; } = 0.2m;

        [JsonPropertyName("responseMimeType")]
        public string ResponseMimeType { get; set; } = "application/json";
    }

    private sealed class GeminiGenerateResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate>? Candidates { get; set; }
    }

    private sealed class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }
    }

    private sealed class AiRoadmapResponse
    {
        [JsonPropertyName("risk_level")]
        public string? RiskLevel { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("next_steps")]
        public List<string>? NextSteps { get; set; }

        [JsonPropertyName("lifestyle_advice")]
        public List<string>? LifestyleAdvice { get; set; }

        [JsonPropertyName("follow_up")]
        public AiFollowUp? FollowUp { get; set; }

        [JsonPropertyName("warning_signs")]
        public List<string>? WarningSigns { get; set; }
    }

    private sealed class AiFollowUp
    {
        [JsonPropertyName("needed")]
        public bool Needed { get; set; }

        [JsonPropertyName("timeframe")]
        public string? Timeframe { get; set; }
    }
}
