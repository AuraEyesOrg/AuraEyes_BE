using System.Globalization;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Screenings.Interfaces;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Screenings.Queries.ExportPatientScreeningReportPdf;

public sealed class ExportPatientScreeningReportPdfQueryHandler
    : IQueryHandler<ExportPatientScreeningReportPdfQuery, PatientScreeningReportPdfFileDto>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<MedicalDiagnosis> _medicalDiagnosisRepository;
    private readonly IRepository<ConsultationSession> _consultationSessionRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;
    private readonly IPatientScreeningPdfService _patientScreeningPdfService;
    private readonly Uri _aiAssetBaseUri;

    public ExportPatientScreeningReportPdfQueryHandler(
        IRepository<Patient> patientRepository,
        IRepository<AiScreening> screeningRepository,
        IRepository<MedicalDiagnosis> medicalDiagnosisRepository,
        IRepository<ConsultationSession> consultationSessionRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService,
        IPatientScreeningPdfService patientScreeningPdfService,
        IAiAssetBaseUrlProvider aiAssetBaseUrlProvider)
    {
        _patientRepository = patientRepository;
        _screeningRepository = screeningRepository;
        _medicalDiagnosisRepository = medicalDiagnosisRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
        _patientScreeningPdfService = patientScreeningPdfService;
        _aiAssetBaseUri = aiAssetBaseUrlProvider.BaseUri;
    }

    public async Task<Result<PatientScreeningReportPdfFileDto>> Handle(
        ExportPatientScreeningReportPdfQuery request,
        CancellationToken cancellationToken)
    {
        var requesterPatient = await _patientRepository
            .Query()
            .Where(x => x.UserId == request.RequesterUserId && !x.IsDeleted)
            .Select(x => new
            {
                x.Id,
                x.FullName
            })
            .FirstOrDefaultAsync(cancellationToken);

        Guid? requesterPatientId = requesterPatient?.Id;
        if (requesterPatient is null && request.RequesterProfileId.HasValue)
        {
            var isDoctorReviewer = await _consultationSessionRepository
                .Query()
                .AnyAsync(
                    s => s.AiScreeningId == request.ScreeningId &&
                         s.OphthalmologistId == request.RequesterProfileId.Value &&
                         !s.IsDeleted,
                    cancellationToken);

            if (!isDoctorReviewer)
                return Result<PatientScreeningReportPdfFileDto>.Forbidden("You are not allowed to export this screening report.");
        }

        var detail = await _screeningRepository
            .Query()
            .Where(x => x.Id == request.ScreeningId && !x.IsDeleted)
            .Where(x => !requesterPatientId.HasValue || x.PatientId == requesterPatientId.Value)
            .Select(x => new
            {
                ScreeningId = x.Id,
                x.PatientId,
                x.CreatedAt,
                x.ModelVersion,
                x.RawJsonOutput,
                Images = x.RetinalImages
                    .Where(i => !i.IsDeleted)
                    .Select(i => i.ImageUrl)
                    .ToList(),
                LatestResult = x.ScreeningResults
                    .Where(r => !r.IsDeleted)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new
                    {
                        RiskLevel = r.RiskLevel.ToString(),
                        r.ConfidenceScore,
                        r.Summary,
                        r.Findings,
                        AssessedAt = r.CreatedAt
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (detail is null)
        {
            return Result<PatientScreeningReportPdfFileDto>.NotFound("Screening session not found.");
        }

        var patientSnapshot = await _patientRepository
            .Query()
            .Where(p => p.Id == detail.PatientId && !p.IsDeleted)
            .Select(p => new
            {
                p.FullName,
                p.UserId
            })
            .FirstOrDefaultAsync(cancellationToken);

        var diagnosisSnapshot = await _medicalDiagnosisRepository
            .Query()
            .Where(d => d.AiScreeningId == detail.ScreeningId && !d.IsDeleted)
            .OrderByDescending(d => d.FinalizedAt ?? d.CreatedAt)
            .Select(d => new
            {
                d.DoctorId,
                d.DiagnosisCode,
                d.CodingSystem,
                d.ClinicalFindings,
                d.SeverityLevel,
                d.ConfidenceLevel,
                d.TreatmentPlan,
                d.Recommendations,
                d.LifestyleAdvice,
                d.IsUrgent,
                d.Status,
                d.FollowUpDate,
                d.IsReferralNeeded,
                d.FinalizedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        var doctorName = "N/A";
        if (diagnosisSnapshot?.DoctorId is Guid doctorId)
        {
            var doctor = await _ophthalmologistRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor is not null)
            {
                var doctorUser = await _identityService.GetUserByIdAsync(doctor.UserId, cancellationToken);
                if (!string.IsNullOrWhiteSpace(doctorUser?.FullName))
                    doctorName = doctorUser.FullName.Trim();
            }
        }

        var aiFindingDetails = ParseAiFindingDetails(detail.RawJsonOutput);
        var localizationBoxes = ParseLocalizationBoxes(detail.RawJsonOutput);
        var visualAssets = ParseVisualAssets(detail.RawJsonOutput);
        var originalImageUrls = detail.Images
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var originalImageBaseUri = TryGetBaseUri(originalImageUrls);

        var patientName = ResolvePatientName(patientSnapshot?.FullName);

        var pdfModel = new PatientScreeningReportPdfModel
        {
            ScreeningId = detail.ScreeningId,
            PatientId = detail.PatientId,
            PatientName = patientName,
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
            ReportedByDoctorName = doctorName,
            DiagnosisCode = diagnosisSnapshot?.DiagnosisCode,
            CodingSystem = diagnosisSnapshot?.CodingSystem,
            OphthamologistFindings = diagnosisSnapshot?.ClinicalFindings,
            SeverityLevel = diagnosisSnapshot?.SeverityLevel,
            ConfidenceLevel = diagnosisSnapshot?.ConfidenceLevel,
            TreatmentPlan = diagnosisSnapshot?.TreatmentPlan,
            Recommendations = diagnosisSnapshot?.Recommendations,
            LifestyleAdvice = diagnosisSnapshot?.LifestyleAdvice,
            ClinicalStatus = diagnosisSnapshot?.Status,
            IsUrgent = diagnosisSnapshot?.IsUrgent ?? false,
            IsReferralNeeded = diagnosisSnapshot?.IsReferralNeeded ?? false,
            FollowUpDate = diagnosisSnapshot?.FollowUpDate,
            FinalizedAt = diagnosisSnapshot?.FinalizedAt,
            AiFindingDetails = aiFindingDetails,
            LocalizationBoxes = localizationBoxes
        };

        var pdfBytes = _patientScreeningPdfService.GenerateScreeningReportPdf(pdfModel);

        return Result<PatientScreeningReportPdfFileDto>.Success(new PatientScreeningReportPdfFileDto
        {
            Content = pdfBytes,
            ContentType = "application/pdf",
            FileName = BuildFileName(pdfModel.PatientName, pdfModel.ScreeningId, pdfModel.CreatedAt)
        });
    }

    private static string ResolvePatientName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return "Patient";

        return fullName.Trim();
    }

    private static string BuildFileName(string patientName, Guid screeningId, DateTime createdAt)
    {
        var safeName = SanitizeFileToken(patientName);
        var screeningShortId = screeningId.ToString("N")[..8];
        return $"patient-screening-report-{safeName}-{createdAt:yyyyMMdd}-{screeningShortId}.pdf";
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

    private static List<PatientAiFindingDetail> ParseAiFindingDetails(string? rawJsonOutput)
    {
        if (string.IsNullOrWhiteSpace(rawJsonOutput))
            return [];

        try
        {
            using var document = JsonDocument.Parse(rawJsonOutput);
            var root = document.RootElement;

            var findings = ParseAnomalies(root);
            if (findings.Count > 0)
                return findings;

            return ParseTopK(root);
        }
        catch
        {
            return [];
        }
    }

    private static List<PatientAiFindingDetail> ParseTopK(JsonElement root)
    {
        if (!root.TryGetProperty("prediction", out var prediction) ||
            !prediction.TryGetProperty("top_k", out var topK) ||
            topK.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var findings = new List<PatientAiFindingDetail>();
        var index = 0;

        foreach (var item in topK.EnumerateArray())
        {
            index++;

            var diseaseName = ResolveTopKFindingName(item);
            if (string.IsNullOrWhiteSpace(diseaseName))
                continue;

            var confidenceValue = TryReadDecimal(item, "confidence") ?? 0m;
            var rank = TryReadInt(item, "rank") ?? index;

            findings.Add(new PatientAiFindingDetail
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

    private static List<PatientAiFindingDetail> ParseAnomalies(JsonElement root)
    {
        if (!root.TryGetProperty("anomalies", out var anomalies) ||
            anomalies.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var findings = new List<PatientAiFindingDetail>();
        var index = 0;

        foreach (var item in anomalies.EnumerateArray())
        {
            index++;

            var diseaseName = TryReadString(item, "name");
            if (string.IsNullOrWhiteSpace(diseaseName))
                continue;

            var confidenceValue = TryReadDecimal(item, "confidence") ?? 0m;

            findings.Add(new PatientAiFindingDetail
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

    private static List<PatientAiLocalizationBox> ParseLocalizationBoxes(string? rawJsonOutput)
    {
        if (string.IsNullOrWhiteSpace(rawJsonOutput))
            return [];

        try
        {
            using var document = JsonDocument.Parse(rawJsonOutput);
            var root = document.RootElement;

            var anomalyBoxes = ParseLocalizationBoxesFromAnomalies(root);
            if (anomalyBoxes.Count > 0)
                return anomalyBoxes;

            if (!root.TryGetProperty("localization", out var localization) ||
                !localization.TryGetProperty("all_lesions", out var lesions) ||
                lesions.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            var boxes = new List<PatientAiLocalizationBox>();
            foreach (var lesion in lesions.EnumerateArray())
            {
                if (!lesion.TryGetProperty("bbox", out var bbox))
                    continue;

                var x = TryReadDecimal(bbox, "x");
                var y = TryReadDecimal(bbox, "y");
                var width = TryReadDecimal(bbox, "width");
                var height = TryReadDecimal(bbox, "height");

                if (!x.HasValue || !y.HasValue || !width.HasValue || !height.HasValue)
                    continue;
                if (width.Value <= 0 || height.Value <= 0)
                    continue;

                boxes.Add(new PatientAiLocalizationBox
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

    private static List<PatientAiLocalizationBox> ParseLocalizationBoxesFromAnomalies(JsonElement root)
    {
        if (!root.TryGetProperty("anomalies", out var anomalies) ||
            anomalies.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var boxes = new List<PatientAiLocalizationBox>();
        foreach (var anomaly in anomalies.EnumerateArray())
        {
            if (!anomaly.TryGetProperty("location", out var location) ||
                location.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            var x = TryReadDecimal(location, "x");
            var y = TryReadDecimal(location, "y");
            var width = TryReadDecimal(location, "width");
            var height = TryReadDecimal(location, "height");

            if (!x.HasValue || !y.HasValue || !width.HasValue || !height.HasValue)
                continue;
            if (width.Value <= 0 || height.Value <= 0)
                continue;

            boxes.Add(new PatientAiLocalizationBox
            {
                X = x.Value,
                Y = y.Value,
                Width = width.Value,
                Height = height.Value,
                Confidence = TryReadDecimal(anomaly, "confidence")
            });
        }

        return boxes;
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
