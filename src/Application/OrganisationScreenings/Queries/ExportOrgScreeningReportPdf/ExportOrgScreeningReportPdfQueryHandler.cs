using Application.Common.Interfaces;
using Application.Common.Models;
using Application.OrganisationScreenings.Interfaces;
using Application.OrganisationScreenings.Queries.GetOrgScreeningSessionDetail;
using Domain.Repositories;
using MediatR;
using System.Globalization;
using System.Text.Json;

namespace Application.OrganisationScreenings.Queries.ExportOrgScreeningReportPdf;

public sealed class ExportOrgScreeningReportPdfQueryHandler
    : IQueryHandler<ExportOrgScreeningReportPdfQuery, OrgScreeningReportPdfFileDto>
{
    private readonly IMediator _mediator;
    private readonly IOrganisationPatientsRepository _organisationPatientsRepository;
    private readonly IOrganisationScreeningPdfService _organisationScreeningPdfService;
    private readonly Uri _aiAssetBaseUri;

    public ExportOrgScreeningReportPdfQueryHandler(
        IMediator mediator,
        IOrganisationPatientsRepository organisationPatientsRepository,
        IOrganisationScreeningPdfService organisationScreeningPdfService,
        IAiAssetBaseUrlProvider aiAssetBaseUrlProvider)
    {
        _mediator = mediator;
        _organisationPatientsRepository = organisationPatientsRepository;
        _organisationScreeningPdfService = organisationScreeningPdfService;
        _aiAssetBaseUri = aiAssetBaseUrlProvider.BaseUri;
    }

    public async Task<Result<OrgScreeningReportPdfFileDto>> Handle(
        ExportOrgScreeningReportPdfQuery request,
        CancellationToken cancellationToken)
    {
        var detailResult = await _mediator.Send(
            new GetOrgScreeningSessionDetailQuery(request.OrgAdminUserId, request.ScreeningId),
            cancellationToken);

        if (!detailResult.IsSuccess || detailResult.Data is null)
            return MapFailure(detailResult);

        var detail = detailResult.Data;

        var patientName = detail.PatientName;
        if (string.IsNullOrWhiteSpace(patientName))
        {
            patientName = await _organisationPatientsRepository.GetPatientDisplayNameForOrganisationAdminAsync(
                request.OrgAdminUserId,
                detail.PatientId,
                cancellationToken);
        }

        var organisationName = await _organisationPatientsRepository.GetOrganisationNameForOrganisationAdminAsync(
            request.OrgAdminUserId,
            cancellationToken);

        var aiFindingDetails = ParseAiFindingDetails(detail.RawJsonOutput);
        var localizationBoxes = ParseLocalizationBoxes(detail.RawJsonOutput);
        var visualAssets = ParseVisualAssets(detail.RawJsonOutput);
        var originalImageUrls = detail.Images
            .Select(x => x.ImageUrl)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var originalImageBaseUri = TryGetBaseUri(originalImageUrls);

        var pdfModel = new OrgScreeningReportPdfModel
        {
            ScreeningId = detail.ScreeningId,
            PatientId = detail.PatientId,
            OrganisationName = string.IsNullOrWhiteSpace(organisationName)
                ? "AuraEyes Partner Organisation"
                : organisationName.Trim(),
            PatientName = string.IsNullOrWhiteSpace(patientName) ? "Patient" : patientName,
            CreatedAt = detail.CreatedAt,
            ModelVersion = detail.ModelVersion,
            ImagesCount = detail.Images.Count,
            OriginalImageUrls = originalImageUrls,
            AnnotatedImageUrl = ResolveAssetUrl(visualAssets.AnnotatedImageUrl, originalImageBaseUri),
            HeatmapImageUrl = ResolveAiHeatmapUrl(visualAssets.HeatmapImageUrl),
            RiskLevel = detail.LatestResult?.RiskLevel,
            ConfidenceScore = detail.LatestResult?.ConfidenceScore,
            Summary = detail.LatestResult?.Summary,
            Findings = detail.LatestResult?.Findings,
            AssessedAt = detail.LatestResult?.AssessedAt,
            AiFindingDetails = aiFindingDetails,
            LocalizationBoxes = localizationBoxes,
        };

        var pdfBytes = _organisationScreeningPdfService.GenerateScreeningReportPdf(pdfModel);

        return Result<OrgScreeningReportPdfFileDto>.Success(new OrgScreeningReportPdfFileDto
        {
            Content = pdfBytes,
            ContentType = "application/pdf",
            FileName = BuildFileName(pdfModel.PatientName, pdfModel.ScreeningId, pdfModel.CreatedAt),
        });
    }

    private static Result<OrgScreeningReportPdfFileDto> MapFailure<T>(Result<T> source)
    {
        if (source.IsUnauthorized)
            return Result<OrgScreeningReportPdfFileDto>.Unauthorized(source.ErrorMessage);

        if (source.IsForbidden)
            return Result<OrgScreeningReportPdfFileDto>.Forbidden(source.ErrorMessage);

        if (source.IsNotFound)
            return Result<OrgScreeningReportPdfFileDto>.NotFound(source.ErrorMessage);

        if (source.IsConflict)
            return Result<OrgScreeningReportPdfFileDto>.Conflict(source.ErrorMessage);

        if (source.IsPaymentRequired)
            return Result<OrgScreeningReportPdfFileDto>.PaymentRequired(source.ErrorMessage);

        return Result<OrgScreeningReportPdfFileDto>.Failure(source.Errors);
    }

    private static string BuildFileName(string patientName, Guid screeningId, DateTime createdAt)
    {
        var safeName = SanitizeFileToken(patientName);
        var screeningShortId = screeningId.ToString("N")[..8];
        return $"screening-report-{safeName}-{createdAt:yyyyMMdd}-{screeningShortId}.pdf";
    }

    private static string SanitizeFileToken(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "patient";

        var cleanedChars = value.Trim().ToLowerInvariant().Select(ch =>
            char.IsLetterOrDigit(ch) ? ch : '-');

        var cleaned = new string(cleanedChars.ToArray());

        while (cleaned.Contains("--", StringComparison.Ordinal))
        {
            cleaned = cleaned.Replace("--", "-", StringComparison.Ordinal);
        }

        cleaned = cleaned.Trim('-');
        return string.IsNullOrWhiteSpace(cleaned) ? "patient" : cleaned;
    }

    private static List<AiFindingDetail> ParseAiFindingDetails(string? rawJsonOutput)
    {
        if (string.IsNullOrWhiteSpace(rawJsonOutput))
            return [];

        try
        {
            using var document = JsonDocument.Parse(rawJsonOutput);
            var root = document.RootElement;

            var findings = ParseTopK(root);
            if (findings.Count > 0)
                return findings;

            return ParseAnomalies(root);
        }
        catch
        {
            return [];
        }
    }

    private static List<AiFindingDetail> ParseTopK(JsonElement root)
    {
        if (!root.TryGetProperty("prediction", out var prediction) ||
            !prediction.TryGetProperty("top_k", out var topK) ||
            topK.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var findings = new List<AiFindingDetail>();
        var index = 0;

        foreach (var item in topK.EnumerateArray())
        {
            index++;

            var diseaseName = ResolveTopKFindingName(item);
            if (string.IsNullOrWhiteSpace(diseaseName))
                continue;

            var confidenceValue = TryReadDecimal(item, "confidence") ?? 0m;
            var rank = TryReadInt(item, "rank") ?? index;

            findings.Add(new AiFindingDetail
            {
                Rank = rank,
                DiseaseName = diseaseName.Trim(),
                ConfidencePercentage = NormalizeConfidencePercentage(confidenceValue),
                Status = TryReadString(item, "status")
            });
        }

        return findings
            .OrderBy(x => x.Rank)
            .ThenByDescending(x => x.ConfidencePercentage)
            .ToList();
    }

    private static List<AiFindingDetail> ParseAnomalies(JsonElement root)
    {
        if (!root.TryGetProperty("anomalies", out var anomalies) ||
            anomalies.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var findings = new List<AiFindingDetail>();
        var index = 0;

        foreach (var item in anomalies.EnumerateArray())
        {
            index++;

            var diseaseName = TryReadString(item, "name");
            if (string.IsNullOrWhiteSpace(diseaseName))
                continue;

            var confidenceValue = TryReadDecimal(item, "confidence") ?? 0m;

            findings.Add(new AiFindingDetail
            {
                Rank = index,
                DiseaseName = diseaseName.Trim(),
                ConfidencePercentage = NormalizeConfidencePercentage(confidenceValue),
                Status = TryReadString(item, "status")
            });
        }

        return findings
            .OrderBy(x => x.Rank)
            .ThenByDescending(x => x.ConfidencePercentage)
            .ToList();
    }

    private static decimal NormalizeConfidencePercentage(decimal value)
    {
        var normalized = value <= 1m ? value * 100m : value;
        return Math.Clamp(normalized, 0m, 100m);
    }

    private static VisualAssets ParseVisualAssets(string? rawJsonOutput)
    {
        if (string.IsNullOrWhiteSpace(rawJsonOutput))
            return VisualAssets.Empty;

        try
        {
            using var document = JsonDocument.Parse(rawJsonOutput);
            var root = document.RootElement;

            var annotatedImageUrl =
                TryReadString(root, "annotatedImageUrl") ??
                TryReadString(root, "annotated_image_url") ??
                TryReadString(root, "image_url");

            var heatmapImageUrl =
                TryReadString(root, "heatmap_url") ??
                TryReadString(root, "heatmap_colormap_url") ??
                TryReadString(root, "heatmapUrl");

            return new VisualAssets(annotatedImageUrl, heatmapImageUrl);
        }
        catch
        {
            return VisualAssets.Empty;
        }
    }

    private static Uri? TryGetBaseUri(IEnumerable<string> originalImageUrls)
    {
        foreach (var imageUrl in originalImageUrls)
        {
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
                continue;

            return new UriBuilder(uri.Scheme, uri.Host, uri.IsDefaultPort ? -1 : uri.Port).Uri;
        }

        return null;
    }

    private string? ResolveAssetUrl(string? candidate, Uri? baseUri)
    {
        if (string.IsNullOrWhiteSpace(candidate))
            return null;

        if (Uri.TryCreate(candidate, UriKind.Absolute, out var absolute))
            return absolute.ToString();

        if (candidate.StartsWith('/'))
        {
            if (Uri.TryCreate(_aiAssetBaseUri, candidate, out var aiAssetResolved))
                return aiAssetResolved.ToString();
        }

        if (baseUri is not null && Uri.TryCreate(baseUri, candidate, out var resolved))
            return resolved.ToString();

        return candidate;
    }

    private string? ResolveAiHeatmapUrl(string? candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate))
            return null;

        var normalizedCandidate = candidate.Trim();

        if (Uri.TryCreate(normalizedCandidate, UriKind.Absolute, out var absolute))
            return absolute.ToString();

        if (normalizedCandidate.StartsWith("api/", StringComparison.OrdinalIgnoreCase))
        {
            normalizedCandidate = "/" + normalizedCandidate;
        }

        if (Uri.TryCreate(_aiAssetBaseUri, normalizedCandidate, out var resolved))
            return resolved.ToString();

        return normalizedCandidate;
    }

    private static string? ResolveTopKFindingName(JsonElement item)
    {
        return
            TryReadString(item, "name_en") ??
            TryReadString(item, "code") ??
            TryReadString(item, "name_vi") ??
            TryReadString(item, "class_name");
    }

    private static List<AiLocalizationBox> ParseLocalizationBoxes(string? rawJsonOutput)
    {
        if (string.IsNullOrWhiteSpace(rawJsonOutput))
            return [];

        try
        {
            using var document = JsonDocument.Parse(rawJsonOutput);
            var root = document.RootElement;

            if (!root.TryGetProperty("localization", out var localization) ||
                !localization.TryGetProperty("all_lesions", out var lesions) ||
                lesions.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            var boxes = new List<AiLocalizationBox>();

            foreach (var lesion in lesions.EnumerateArray())
            {
                if (!lesion.TryGetProperty("bbox", out var bbox))
                    continue;

                var x = TryReadInt(bbox, "x");
                var y = TryReadInt(bbox, "y");
                var width = TryReadInt(bbox, "width");
                var height = TryReadInt(bbox, "height");

                if (!x.HasValue || !y.HasValue || !width.HasValue || !height.HasValue)
                    continue;

                if (width.Value <= 0 || height.Value <= 0)
                    continue;

                boxes.Add(new AiLocalizationBox
                {
                    X = x.Value,
                    Y = y.Value,
                    Width = width.Value,
                    Height = height.Value,
                    Confidence = TryReadDecimal(lesion, "confidence")
                });
            }

            return boxes;
        }
        catch
        {
            return [];
        }
    }

    private static string? TryReadString(JsonElement item, string propertyName)
    {
        if (!item.TryGetProperty(propertyName, out var property))
            return null;

        return property.ValueKind switch
        {
            JsonValueKind.String => property.GetString(),
            JsonValueKind.Number => property.ToString(),
            _ => null
        };
    }

    private static decimal? TryReadDecimal(JsonElement item, string propertyName)
    {
        if (!item.TryGetProperty(propertyName, out var property))
            return null;

        if (property.ValueKind == JsonValueKind.Number && property.TryGetDecimal(out var numberValue))
            return numberValue;

        if (property.ValueKind == JsonValueKind.String &&
            decimal.TryParse(property.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static int? TryReadInt(JsonElement item, string propertyName)
    {
        if (!item.TryGetProperty(propertyName, out var property))
            return null;

        if (property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var intValue))
            return intValue;

        if (property.ValueKind == JsonValueKind.String &&
            int.TryParse(property.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private sealed record VisualAssets(string? AnnotatedImageUrl, string? HeatmapImageUrl)
    {
        public static VisualAssets Empty { get; } = new(null, null);
    }
}
