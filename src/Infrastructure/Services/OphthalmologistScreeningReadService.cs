using Application.Common.Interfaces;
using Application.OphthalmologistScreenings;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Services;

public sealed class OphthalmologistScreeningReadService : IOphthalmologistScreeningReadService
{
    private readonly ApplicationDbContext _context;

    public OphthalmologistScreeningReadService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OphthalmologistScreeningListItemDto>> ListForOphthalmologistAsync(
        Guid ophthalmologistProfileId,
        CancellationToken cancellationToken = default)
    {
        var screeningIds = await _context.Set<ConsultationSession>()
            .AsNoTracking()
            .Where(cs => cs.OphthalmologistId == ophthalmologistProfileId && cs.AiScreeningId != null)
            .Select(cs => cs.AiScreeningId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (screeningIds.Count == 0)
            return Array.Empty<OphthalmologistScreeningListItemDto>();

        var diagnoses = await _context.Set<MedicalDiagnosis>()
            .AsNoTracking()
            .Where(d => d.DoctorId == ophthalmologistProfileId && screeningIds.Contains(d.AiScreeningId))
            .ToListAsync(cancellationToken);

        var diagByScreening = diagnoses
            .GroupBy(d => d.AiScreeningId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CreatedAt).First());

        var baseRows = await (
            from s in _context.Set<AiScreening>().AsNoTracking()
            join p in _context.Set<Patient>().AsNoTracking() on s.PatientId equals p.Id
            join u in _context.Set<ApplicationUser>().AsNoTracking() on p.UserId equals u.Id
            where screeningIds.Contains(s.Id)
            orderby s.CreatedAt descending
            select new
            {
                Screening = s,
                PatientName = u.FullName != null && u.FullName.Length > 0 ? u.FullName : (u.Email ?? "Patient")
            }
        ).ToListAsync(cancellationToken);

        var allResults = await _context.Set<ScreeningResult>()
            .AsNoTracking()
            .Where(r => screeningIds.Contains(r.AiScreeningId))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        var latestResultByScreening = allResults
            .GroupBy(r => r.AiScreeningId)
            .ToDictionary(g => g.Key, g => g.First());

        var retinalRows = await _context.Set<RetinalImage>()
            .AsNoTracking()
            .Where(r => r.AiScreeningId != null && screeningIds.Contains(r.AiScreeningId.Value))
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        var imgByScreening = retinalRows
            .GroupBy(r => r.AiScreeningId!.Value)
            .ToDictionary(g => g.Key, g => (Count: g.Count(), Thumb: g.First().ImageUrl));

        var list = new List<OphthalmologistScreeningListItemDto>(baseRows.Count);
        foreach (var row in baseRows)
        {
            var s = row.Screening;
            imgByScreening.TryGetValue(s.Id, out var imgInfo);
            latestResultByScreening.TryGetValue(s.Id, out var latest);
            diagByScreening.TryGetValue(s.Id, out var diag);

            list.Add(new OphthalmologistScreeningListItemDto
            {
                ScreeningId = s.Id,
                PatientId = s.PatientId,
                PatientName = row.PatientName,
                CreatedAt = s.CreatedAt,
                ProcessedAt = s.ProcessedAt,
                ModelVersion = s.ModelVersion,
                ImagesCount = imgInfo.Count,
                ThumbnailUrl = imgInfo.Thumb,
                LatestRiskLevel = latest?.RiskLevel.ToString(),
                ConfidenceScore = latest?.ConfidenceScore,
                AiPrimaryLabel = TryGetPrimaryClassName(s.RawJsonOutput),
                SummarySnippet = Truncate(latest?.Summary ?? latest?.Findings, 120),
                ReviewStatus = MapReviewStatus(diag)
            });
        }

        return list;
    }

    /// <inheritdoc />
    public async Task<OphthalmologistScreeningDetailDto?> GetDetailForOphthalmologistAsync(
        Guid ophthalmologistProfileId,
        Guid screeningId,
        CancellationToken cancellationToken = default)
    {
        var allowed = await _context.Set<ConsultationSession>()
            .AsNoTracking()
            .AnyAsync(
                cs => cs.OphthalmologistId == ophthalmologistProfileId && cs.AiScreeningId == screeningId,
                cancellationToken);

        if (!allowed)
            return null;

        var row = await (
            from scr in _context.Set<AiScreening>().AsNoTracking()
            join p in _context.Set<Patient>().AsNoTracking() on scr.PatientId equals p.Id
            join u in _context.Set<ApplicationUser>().AsNoTracking() on p.UserId equals u.Id
            where scr.Id == screeningId
            select new { Screening = scr, PatientName = u.FullName != null && u.FullName.Length > 0 ? u.FullName : (u.Email ?? "Patient") }
        ).FirstOrDefaultAsync(cancellationToken);

        if (row is null)
            return null;

        var images = await _context.Set<RetinalImage>()
            .AsNoTracking()
            .Where(r => r.AiScreeningId == screeningId)
            .OrderBy(r => r.CreatedAt)
            .Select(r => new OphthalmologistRetinalImageDto
            {
                Id = r.Id,
                ImageUrl = r.ImageUrl,
                EyeSide = r.EyeSide.ToString(),
                DeviceName = r.DeviceName,
                QualityScore = r.QualityScore,
                CapturedAt = r.CapturedAt
            })
            .ToListAsync(cancellationToken);

        var latest = await _context.Set<ScreeningResult>()
            .AsNoTracking()
            .Where(r => r.AiScreeningId == screeningId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new OphthalmologistScreeningResultDto
            {
                ScreeningResultId = r.Id,
                RiskLevel = r.RiskLevel.ToString(),
                ConfidenceScore = r.ConfidenceScore,
                Summary = r.Summary,
                Findings = r.Findings,
                AssessedAt = r.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        var screening = row.Screening;
        return new OphthalmologistScreeningDetailDto
        {
            ScreeningId = screening.Id,
            PatientId = screening.PatientId,
            PatientFullName = row.PatientName,
            ModelVersion = screening.ModelVersion,
            CreatedAt = screening.CreatedAt,
            ProcessedAt = screening.ProcessedAt,
            RawJsonOutput = screening.RawJsonOutput,
            Images = images,
            LatestResult = latest
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
