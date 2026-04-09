using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Repositories;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Persistence.Repositories;

public sealed class OrganisationPatientsRepository : IOrganisationPatientsRepository
{
    private readonly ApplicationDbContext _context;

    public OrganisationPatientsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OrganisationRecentPatientReadModel>> GetRecentPatientsForOrganisationAdminAsync(
        Guid orgAdminUserId,
        int take,
        CancellationToken cancellationToken = default)
    {
        var appUser = await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == orgAdminUserId, cancellationToken);

        if (appUser?.OrganizationId is null)
            return Array.Empty<OrganisationRecentPatientReadModel>();

        var organisationId = appUser.OrganizationId.Value;
        take = Math.Clamp(take, 1, 100);

        var organisationScreeningIds = _context.Set<ConsultationSession>()
            .AsNoTracking()
            .Where(cs => cs.OrganisationId == organisationId && cs.AiScreeningId != null)
            .Select(cs => cs.AiScreeningId!.Value)
            .Distinct();

        var topPatients = await (
            from u in _context.Set<ApplicationUser>().AsNoTracking()
            where u.OrganizationId == organisationId && !u.IsDeleted
            join p in _context.Set<Patient>().AsNoTracking() on u.Id equals p.UserId
            let latestScreeningId = (
                from scr in _context.Set<AiScreening>().AsNoTracking()
                join screeningId in organisationScreeningIds on scr.Id equals screeningId
                where scr.PatientId == p.Id
                orderby scr.CreatedAt descending
                select (Guid?)scr.Id
            ).FirstOrDefault()
            let latestScreeningCreatedAt = (
                from scr in _context.Set<AiScreening>().AsNoTracking()
                join screeningId in organisationScreeningIds on scr.Id equals screeningId
                where scr.PatientId == p.Id
                orderby scr.CreatedAt descending
                select (DateTime?)scr.CreatedAt
            ).FirstOrDefault()
            orderby latestScreeningCreatedAt ?? u.CreatedAt descending
            select new
            {
                PatientUser = u,
                Patient = p,
                LatestScreeningId = latestScreeningId
            }
        )
        .Take(take)
        .ToListAsync(cancellationToken);

        if (topPatients.Count == 0)
            return Array.Empty<OrganisationRecentPatientReadModel>();

        var screeningIdSet = topPatients
            .Where(x => x.LatestScreeningId.HasValue)
            .Select(x => x.LatestScreeningId!.Value)
            .ToHashSet();

        var screeningById = screeningIdSet.Count == 0
            ? new Dictionary<Guid, AiScreening>()
            : await _context.Set<AiScreening>()
                .AsNoTracking()
                .Where(s => screeningIdSet.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, cancellationToken);

        var latestResults = screeningIdSet.Count == 0
            ? new List<ScreeningResult>()
            : await _context.Set<ScreeningResult>()
                .AsNoTracking()
                .Where(r => screeningIdSet.Contains(r.AiScreeningId))
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

        var resultByScreeningId = latestResults
            .GroupBy(r => r.AiScreeningId)
            .ToDictionary(g => g.Key, g => g.First());

        var latestDiagnoses = screeningIdSet.Count == 0
            ? new List<MedicalDiagnosis>()
            : await _context.Set<MedicalDiagnosis>()
                .AsNoTracking()
                .Where(d => screeningIdSet.Contains(d.AiScreeningId))
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(cancellationToken);

        var diagByScreeningId = latestDiagnoses
            .GroupBy(d => d.AiScreeningId)
            .ToDictionary(g => g.Key, g => g.First());

        var list = new List<OrganisationRecentPatientReadModel>(topPatients.Count);

        foreach (var row in topPatients)
        {
            var p = row.Patient;
            var u = row.PatientUser;
            AiScreening? scr = null;

            if (row.LatestScreeningId is Guid screeningId)
            {
                screeningById.TryGetValue(screeningId, out scr);
            }

            ScreeningResult? latest = null;
            MedicalDiagnosis? diag = null;

            if (scr != null)
            {
                resultByScreeningId.TryGetValue(scr.Id, out latest);
                diagByScreeningId.TryGetValue(scr.Id, out diag);
            }

            var status = scr == null ? "pending-review" : MapStatus(diag);
            var priority = scr == null ? "low" : MapPriority(latest?.RiskLevel, diag);

            list.Add(new OrganisationRecentPatientReadModel
            {
                Id = p.Id.ToString(),
                Name = string.IsNullOrWhiteSpace(u.FullName) ? (u.Email ?? "Patient") : u.FullName,
                Age = ComputeAge(u.DateOfBirth),
                Gender = MapGender(u.Gender),
                DateOfBirth = u.DateOfBirth,
                CitizenId = u.CitizenId,
                Address = u.Address,
                Email = u.Email ?? string.Empty,
                PhoneNumber = u.PhoneNumber ?? string.Empty,
                LastScreening = scr?.CreatedAt ?? u.CreatedAt,
                AiPrediction = scr != null ? (TryGetPrimaryClassName(scr.RawJsonOutput) ?? "AI prediction") : "No screening yet",
                Confidence = latest?.ConfidenceScore ?? 0,
                Status = status,
                Priority = priority
            });
        }

        return list;
    }

    public async Task<IReadOnlyList<OrganisationScreeningHistoryReadModel>> GetScreeningHistoryForOrganisationAdminAsync(
        Guid orgAdminUserId,
        int take,
        CancellationToken cancellationToken = default)
    {
        var appUser = await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == orgAdminUserId, cancellationToken);

        if (appUser?.OrganizationId is null)
            return Array.Empty<OrganisationScreeningHistoryReadModel>();

        var organisationId = appUser.OrganizationId.Value;
        take = Math.Clamp(take, 1, 100);

        var screenings = await (
            from scr in _context.Set<AiScreening>().AsNoTracking()
            join p in _context.Set<Patient>().AsNoTracking() on scr.PatientId equals p.Id
            join u in _context.Set<ApplicationUser>().AsNoTracking() on p.UserId equals u.Id
            where u.OrganizationId == organisationId && !u.IsDeleted && !scr.IsDeleted
            orderby scr.CreatedAt descending
            select new
            {
                scr.Id,
                scr.PatientId,
                PatientName = string.IsNullOrWhiteSpace(u.FullName)
                    ? (u.Email ?? "Patient")
                    : u.FullName,
                scr.CreatedAt,
                scr.ProcessedAt,
                ImagesCount = scr.RetinalImages.Count,
                scr.RawJsonOutput
            }
        )
        .Take(take)
        .ToListAsync(cancellationToken);

        if (screenings.Count == 0)
            return Array.Empty<OrganisationScreeningHistoryReadModel>();

        var screeningIdSet = screenings.Select(s => s.Id).ToHashSet();

        var latestResults = await _context.Set<ScreeningResult>()
            .AsNoTracking()
            .Where(r => screeningIdSet.Contains(r.AiScreeningId))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        var resultByScreeningId = latestResults
            .GroupBy(r => r.AiScreeningId)
            .ToDictionary(g => g.Key, g => g.First());

        var latestDiagnoses = await _context.Set<MedicalDiagnosis>()
            .AsNoTracking()
            .Where(d => screeningIdSet.Contains(d.AiScreeningId))
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);

        var diagnosisByScreeningId = latestDiagnoses
            .GroupBy(d => d.AiScreeningId)
            .ToDictionary(g => g.Key, g => g.First());

        var history = new List<OrganisationScreeningHistoryReadModel>(screenings.Count);

        foreach (var item in screenings)
        {
            resultByScreeningId.TryGetValue(item.Id, out var latestResult);
            diagnosisByScreeningId.TryGetValue(item.Id, out var latestDiagnosis);

            history.Add(new OrganisationScreeningHistoryReadModel
            {
                ScreeningId = item.Id,
                PatientId = item.PatientId,
                PatientName = item.PatientName,
                CreatedAt = item.CreatedAt,
                ProcessedAt = item.ProcessedAt,
                ImagesCount = item.ImagesCount,
                LatestRiskLevel = latestResult?.RiskLevel.ToString(),
                ConfidenceScore = latestResult?.ConfidenceScore,
                AiPrimaryLabel = TryGetPrimaryClassName(item.RawJsonOutput),
                Status = MapScreeningStatus(latestResult, latestDiagnosis)
            });
        }

        return history;
    }

    public async Task<OrganisationScreeningCountsReadModel> GetScreeningCountsForOrganisationAsync(
        Guid organisationId,
        DateTime fromUtc,
        CancellationToken cancellationToken = default)
    {
        var organisationScreeningIds = _context.Set<ConsultationSession>()
            .AsNoTracking()
            .Where(cs => cs.OrganisationId == organisationId && cs.AiScreeningId != null)
            .Select(cs => cs.AiScreeningId!.Value)
            .Distinct();

        var screeningsQuery = _context.Set<AiScreening>()
            .AsNoTracking()
            .Where(scr => organisationScreeningIds.Contains(scr.Id));

        var allTimeCountTask = screeningsQuery.CountAsync(cancellationToken);
        var fromDateCountTask = screeningsQuery.CountAsync(scr => scr.CreatedAt >= fromUtc, cancellationToken);

        await Task.WhenAll(allTimeCountTask, fromDateCountTask);

        return new OrganisationScreeningCountsReadModel
        {
            TotalScreeningsAllTime = allTimeCountTask.Result,
            TotalScreeningsFromDate = fromDateCountTask.Result
        };
    }

    public async Task<OrganisationScreeningReportReadModel> GetScreeningReportForOrganisationAdminAsync(
        Guid orgAdminUserId,
        CancellationToken cancellationToken = default)
    {
        var appUser = await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == orgAdminUserId, cancellationToken);

        if (appUser?.OrganizationId is null)
            return new OrganisationScreeningReportReadModel();

        var organisationId = appUser.OrganizationId.Value;

        var screeningEvents =
            from scr in _context.Set<AiScreening>().AsNoTracking()
            join p in _context.Set<Patient>().AsNoTracking() on scr.PatientId equals p.Id
            join u in _context.Set<ApplicationUser>().AsNoTracking() on p.UserId equals u.Id
            where u.OrganizationId == organisationId && !u.IsDeleted && !scr.IsDeleted
            select new
            {
                scr.CreatedAt,
                LatestRiskLevel = _context.Set<ScreeningResult>()
                    .AsNoTracking()
                    .Where(r => r.AiScreeningId == scr.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => (Domain.Enums.RiskLevel?)r.RiskLevel)
                    .FirstOrDefault(),
                LatestConfidence = _context.Set<ScreeningResult>()
                    .AsNoTracking()
                    .Where(r => r.AiScreeningId == scr.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => (decimal?)r.ConfidenceScore)
                    .FirstOrDefault(),
                IsReferralNeeded = _context.Set<MedicalDiagnosis>()
                    .AsNoTracking()
                    .Where(d => d.AiScreeningId == scr.Id)
                    .OrderByDescending(d => d.CreatedAt)
                    .Select(d => (bool?)d.IsReferralNeeded)
                    .FirstOrDefault() ?? false
            };

        var totals = await screeningEvents
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                HighRisk = g.Count(x =>
                    x.IsReferralNeeded
                    || x.LatestRiskLevel == Domain.Enums.RiskLevel.Critical
                    || x.LatestRiskLevel == Domain.Enums.RiskLevel.High),
                ModerateRisk = g.Count(x =>
                    !x.IsReferralNeeded
                    && x.LatestRiskLevel == Domain.Enums.RiskLevel.Moderate),
                AverageConfidence = g.Average(x => x.LatestConfidence ?? 0m)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (totals is null)
            return new OrganisationScreeningReportReadModel();

        var monthlyRows = await screeningEvents
            .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Count = g.Count(),
                HighRisk = g.Count(x =>
                    x.IsReferralNeeded
                    || x.LatestRiskLevel == Domain.Enums.RiskLevel.Critical
                    || x.LatestRiskLevel == Domain.Enums.RiskLevel.High),
                ModerateRisk = g.Count(x =>
                    !x.IsReferralNeeded
                    && x.LatestRiskLevel == Domain.Enums.RiskLevel.Moderate)
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .Take(6)
            .ToListAsync(cancellationToken);

        var monthly = monthlyRows
            .Select(x => new OrganisationMonthlyScreeningCountReadModel
            {
                Month = $"{x.Year:D4}-{x.Month:D2}",
                Count = x.Count,
                HighRisk = x.HighRisk,
                ModerateRisk = x.ModerateRisk,
                LowRisk = x.Count - x.HighRisk - x.ModerateRisk
            })
            .OrderBy(x => x.Month)
            .ToList();

        return new OrganisationScreeningReportReadModel
        {
            TotalScreenings = totals.Total,
            HighRiskCount = totals.HighRisk,
            ModerateRiskCount = totals.ModerateRisk,
            LowRiskCount = totals.Total - totals.HighRisk - totals.ModerateRisk,
            AverageConfidence = totals.AverageConfidence,
            MonthlyBreakdown = monthly
        };
    }

    public async Task<bool> IsPatientManagedByOrganisationAdminAsync(
        Guid orgAdminUserId,
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var organisationId = await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == orgAdminUserId && !u.IsDeleted)
            .Select(u => u.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (!organisationId.HasValue)
            return false;

        return await (
            from p in _context.Set<Patient>().AsNoTracking()
            join u in _context.Set<ApplicationUser>().AsNoTracking() on p.UserId equals u.Id
            where p.Id == patientId
                  && !u.IsDeleted
                  && u.OrganizationId == organisationId.Value
            select p.Id
        ).AnyAsync(cancellationToken);
    }

    public async Task<string?> GetPatientDisplayNameForOrganisationAdminAsync(
        Guid orgAdminUserId,
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var organisationId = await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == orgAdminUserId && !u.IsDeleted)
            .Select(u => u.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (!organisationId.HasValue)
            return null;

        return await (
            from p in _context.Set<Patient>().AsNoTracking()
            join u in _context.Set<ApplicationUser>().AsNoTracking() on p.UserId equals u.Id
            where p.Id == patientId
                  && !u.IsDeleted
                  && u.OrganizationId == organisationId.Value
            select string.IsNullOrWhiteSpace(u.FullName)
                ? (u.Email ?? "Patient")
                : u.FullName
        ).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string?> GetOrganisationNameForOrganisationAdminAsync(
        Guid orgAdminUserId,
        CancellationToken cancellationToken = default)
    {
        var organisationId = await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == orgAdminUserId && !u.IsDeleted)
            .Select(u => u.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (!organisationId.HasValue)
            return null;

        return await _context.Set<Organisation>()
            .AsNoTracking()
            .Where(o => o.Id == organisationId.Value && !o.IsDeleted)
            .Select(o => o.Name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static int ComputeAge(DateTime? dob)
    {
        if (dob is null) return 0;
        var now = DateTime.UtcNow.Date;
        var birth = dob.Value.Date;
        var age = now.Year - birth.Year;
        if (birth > now.AddYears(-age)) age--;
        return Math.Max(0, age);
    }

    private static string MapGender(Domain.Enums.Gender? gender)
    {
        return gender switch
        {
            Domain.Enums.Gender.Male => "M",
            Domain.Enums.Gender.Female => "F",
            _ => string.Empty
        };
    }

    private static string MapStatus(MedicalDiagnosis? d)
    {
        if (d is null)
            return "pending-review";

        if (d.ConfirmedAt.HasValue)
            return "reviewed";

        if (d.IsReferralNeeded)
            return "archived";

        return "reviewed";
    }

    private static string MapScreeningStatus(ScreeningResult? result, MedicalDiagnosis? diagnosis)
    {
        if (diagnosis?.ConfirmedAt.HasValue == true || diagnosis?.IsReferralNeeded == true)
            return "completed";

        if (result is not null)
            return "saved";

        return "pending";
    }

    private static string MapPriority(Domain.Enums.RiskLevel? risk, MedicalDiagnosis? d)
    {
        var riskLevel = risk?.ToString()?.ToLowerInvariant() ?? string.Empty;
        var referralNeeded = d?.IsReferralNeeded ?? false;

        if (referralNeeded || riskLevel.Contains("critical") || riskLevel.Contains("high"))
            return "high";

        if (riskLevel.Contains("moderate") || riskLevel.Contains("medium"))
            return "medium";

        return "low";
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
}
