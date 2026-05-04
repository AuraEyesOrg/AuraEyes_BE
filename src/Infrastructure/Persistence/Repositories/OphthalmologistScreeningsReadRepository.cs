using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Read-only repository for ophthalmologist screening queries.
/// </summary>
public sealed class OphthalmologistScreeningsReadRepository : IOphthalmologistScreeningsReadRepository
{
    private readonly ApplicationDbContext _context;

    public OphthalmologistScreeningsReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OphthalmologistScreeningListReadModel>> ListForOphthalmologistAsync(
        Guid ophthalmologistProfileId,
        CancellationToken cancellationToken = default)
    {
        // 1. Get screening IDs for this ophthalmologist
        var screeningIds = await _context.Set<ConsultationSession>()
            .AsNoTracking()
            .Where(cs => cs.OphthalmologistId == ophthalmologistProfileId && cs.AiScreeningId != null)
            .Select(cs => cs.AiScreeningId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (screeningIds.Count == 0)
            return Array.Empty<OphthalmologistScreeningListReadModel>();

        // Parallelize independent queries
        var baseRowsTask = (
            from s in _context.Set<AiScreening>().AsNoTracking()
            join p in _context.Set<Patient>().AsNoTracking() on s.PatientId equals p.Id
            join u in _context.Set<ApplicationUser>().AsNoTracking() on p.UserId equals u.Id into users
            from u in users.DefaultIfEmpty()
            where screeningIds.Contains(s.Id)
            orderby s.CreatedAt descending
            select new
            {
                Screening = s,
                PatientName =
                    p.UserId == null
                        ? p.FullName
                        : (u != null && u.FullName != null && u.FullName.Length > 0
                            ? u.FullName
                            : (u != null ? u.Email : null)) ?? "Patient"
            }
        ).ToListAsync(cancellationToken);

        var latestResultByScreeningTask = (
            from r in _context.Set<ScreeningResult>().AsNoTracking()
            where screeningIds.Contains(r.AiScreeningId)
            group r by r.AiScreeningId into grp
            select grp.OrderByDescending(x => x.CreatedAt).First()
        ).ToListAsync(cancellationToken);

        var imgMetadataTask = (
            from img in _context.Set<RetinalImage>().AsNoTracking()
            where img.AiScreeningId != null && screeningIds.Contains(img.AiScreeningId.Value)
            group img by img.AiScreeningId into grp
            select new
            {
                ScreeningId = grp.Key!.Value,
                Count = grp.Count(),
                Thumb = grp.OrderBy(i => i.CreatedAt).Select(i => i.ImageUrl).First()
            }
        ).ToListAsync(cancellationToken);

        var diagnosesTask = _context.Set<MedicalDiagnosis>()
            .AsNoTracking()
            .Where(d => d.DoctorId == ophthalmologistProfileId && screeningIds.Contains(d.AiScreeningId))
            .ToListAsync(cancellationToken);

        await Task.WhenAll(baseRowsTask, latestResultByScreeningTask, imgMetadataTask, diagnosesTask);

        var baseRows = await baseRowsTask;
        var latestResultByScreening = await latestResultByScreeningTask;
        var imgMetadata = await imgMetadataTask;
        var diagnoses = await diagnosesTask;

        var resultDict = latestResultByScreening.ToDictionary(r => r.AiScreeningId);
        var imgDict = imgMetadata.ToDictionary(x => x.ScreeningId);
        var diagByScreening = diagnoses
            .GroupBy(d => d.AiScreeningId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CreatedAt).First());

        // 6. Project read models using pre-aggregated data
        var list = new List<OphthalmologistScreeningListReadModel>(baseRows.Count);
        foreach (var row in baseRows)
        {
            var s = row.Screening;
            resultDict.TryGetValue(s.Id, out var latest);
            imgDict.TryGetValue(s.Id, out var imgInfo);
            diagByScreening.TryGetValue(s.Id, out var diag);

            list.Add(new OphthalmologistScreeningListReadModel
            {
                ScreeningId = s.Id,
                PatientId = s.PatientId,
                PatientName = row.PatientName,
                CreatedAt = s.CreatedAt,
                ProcessedAt = s.ProcessedAt,
                ModelVersion = s.ModelVersion,
                ImagesCount = imgInfo?.Count ?? 0,
                ThumbnailUrl = imgInfo?.Thumb,
                LatestRiskLevel = latest?.RiskLevel.ToString(),
                ConfidenceScore = latest?.ConfidenceScore,
                AiPrimaryLabel = TryGetPrimaryClassName(s.RawJsonOutput),
                SummarySnippet = Truncate(latest?.Summary ?? latest?.Findings, 120),
                ReviewStatus = MapReviewStatus(diag)
            });
        }

        return list;
    }

    public async Task<OphthalmologistScreeningDetailReadModel?> GetDetailForOphthalmologistAsync(
        Guid ophthalmologistProfileId,
        Guid screeningId,
        CancellationToken cancellationToken = default)
    {
        // Check access and read sharing metadata for this doctor-session link.
        var consultation = await _context.Set<ConsultationSession>()
            .AsNoTracking()
            .Where(cs => cs.OphthalmologistId == ophthalmologistProfileId && cs.AiScreeningId == screeningId)
            .Select(cs => new
            {
                Id = cs.Id,
                Type = cs.Type,
                HasAssignedDoctor = cs.OphthalmologistId.HasValue
            })
            .FirstOrDefaultAsync(cancellationToken);

        // Access denied: no linked consultation
        if (consultation is null)
            return null;
        
        var bypassRedactionForAssignedDoctor =
            consultation.HasAssignedDoctor &&
            (consultation.Type == ConsultationSessionType.Verification ||
             consultation.Type == ConsultationSessionType.ClinicBooking);

        var canViewRetinalImages = bypassRedactionForAssignedDoctor;
        var canViewAiResults = bypassRedactionForAssignedDoctor;

        var row = await (
            from scr in _context.Set<AiScreening>().AsNoTracking()
            join p in _context.Set<Patient>().AsNoTracking() on scr.PatientId equals p.Id
            join u in _context.Set<ApplicationUser>().AsNoTracking() on p.UserId equals u.Id into users
            from u in users.DefaultIfEmpty()
            where scr.Id == screeningId
            select new
            {
                Screening = scr,
                PatientName =
                    p.UserId == null
                        ? p.FullName
                        : (u != null && u.FullName != null && u.FullName.Length > 0
                            ? u.FullName
                            : (u != null ? u.Email : null)) ?? "Patient"
            }
        ).FirstOrDefaultAsync(cancellationToken);

        if (row is null)
            return null;

        // Assigned doctors on verification/clinic sessions always receive full screening data.
        IReadOnlyList<OphthalmologistRetinalImageReadModel> images = Array.Empty<OphthalmologistRetinalImageReadModel>();
        if (canViewRetinalImages)
        {
            images = await _context.Set<RetinalImage>()
                .AsNoTracking()
                .Where(r => r.AiScreeningId == screeningId)
                .OrderBy(r => r.CreatedAt)
                .Select(r => new OphthalmologistRetinalImageReadModel
                {
                    Id = r.Id,
                    ImageUrl = r.ImageUrl,
                    EyeSide = r.EyeSide.ToString(),
                    DeviceName = r.DeviceName,
                    QualityScore = r.QualityScore,
                    CapturedAt = r.CapturedAt
                })
                .ToListAsync(cancellationToken);
        }

        // Assigned doctors on verification/clinic sessions always receive full screening data.
        OphthalmologistScreeningResultReadModel? latest = null;
        if (canViewAiResults)
        {
            latest = await _context.Set<ScreeningResult>()
                .AsNoTracking()
                .Where(r => r.AiScreeningId == screeningId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new OphthalmologistScreeningResultReadModel
                {
                    ScreeningResultId = r.Id,
                    RiskLevel = r.RiskLevel.ToString(),
                    ConfidenceScore = r.ConfidenceScore,
                    Summary = r.Summary,
                    Findings = r.Findings,
                    AssessedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        var latestDiagnosis = await _context.Set<MedicalDiagnosis>()
            .AsNoTracking()
            .Where(d => d.DoctorId == ophthalmologistProfileId && d.AiScreeningId == screeningId)
            .OrderByDescending(d => d.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var screening = row.Screening;
        return new OphthalmologistScreeningDetailReadModel
        {
            ScreeningId = screening.Id,
            PatientId = screening.PatientId,
            PatientFullName = row.PatientName,
            ModelVersion = screening.ModelVersion,
            CreatedAt = screening.CreatedAt,
            ProcessedAt = screening.ProcessedAt,
            RawJsonOutput = canViewAiResults ? screening.RawJsonOutput : null,
            Images = images,
            LatestResult = latest,
            ReviewStatus = MapReviewStatus(latestDiagnosis),
            MedicalRecordId = await _context.Set<Domain.Entities.MedicalRecords.MedicalRecord>()
                .AsNoTracking()
                .Where(mr => mr.ConsultationSessionId == consultation.Id)
                .Select(mr => (Guid?)mr.Id)
                .FirstOrDefaultAsync(cancellationToken)
        };
    }

    private static string MapReviewStatus(MedicalDiagnosis? d)
    {
        if (d is null)
            return "pending-review";
        if (d.ConfirmedAt.HasValue)
            return "approved";
        if (d.IsReferralNeeded)
            return "flagged";
        return "reviewed";
    }

    private static string? TryGetPrimaryClassName(string? rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
            return null;
        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;
            if (root.TryGetProperty("prediction", out var pred) &&
                pred.TryGetProperty("primary", out var primary) &&
                primary.TryGetProperty("class_name", out var cn))
            {
                return cn.GetString();
            }
        }
        catch
        {
            // ignore
        }

        return null;
    }

    private static string? Truncate(string? text, int maxLen)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        var t = text.Trim();
        return t.Length <= maxLen ? t : t[..maxLen] + "…";
    }
}
