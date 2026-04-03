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

        var orgPatients = await (
            from u in _context.Set<ApplicationUser>().AsNoTracking()
            where u.OrganizationId == organisationId && !u.IsDeleted
            join p in _context.Set<Patient>().AsNoTracking() on u.Id equals p.UserId
            select new { PatientUser = u, Patient = p }
        ).ToListAsync(cancellationToken);

        var screeningRows = await (
            from scr in _context.Set<AiScreening>().AsNoTracking()
            join p in _context.Set<Patient>().AsNoTracking() on scr.PatientId equals p.Id
            join u in _context.Set<ApplicationUser>().AsNoTracking() on p.UserId equals u.Id
            join screeningId in organisationScreeningIds on scr.Id equals screeningId
            orderby scr.CreatedAt descending
            select new { Screening = scr, Patient = p, PatientUser = u }
        ).ToListAsync(cancellationToken);

        var uniquePatients = new Dictionary<Guid, (ApplicationUser User, Patient Patient, AiScreening? LatestScreening)>();

        foreach (var row in orgPatients)
        {
            uniquePatients[row.Patient.Id] = (row.PatientUser, row.Patient, null);
        }

        foreach (var row in screeningRows)
        {
            if (!uniquePatients.TryGetValue(row.Patient.Id, out var existing))
            {
                uniquePatients[row.Patient.Id] = (row.PatientUser, row.Patient, row.Screening);
            }
            else
            {
                if (existing.LatestScreening == null || row.Screening.CreatedAt > existing.LatestScreening.CreatedAt)
                {
                    uniquePatients[row.Patient.Id] = (row.PatientUser, row.Patient, row.Screening);
                }
            }
        }

        if (uniquePatients.Count == 0)
            return Array.Empty<OrganisationRecentPatientReadModel>();

        var orderedPatients = uniquePatients.Values
            .OrderByDescending(x => x.LatestScreening?.CreatedAt ?? x.User.CreatedAt)
            .Take(take)
            .ToList();

        var screeningIdSet = orderedPatients
            .Where(x => x.LatestScreening != null)
            .Select(x => x.LatestScreening!.Id)
            .ToHashSet();

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

        var diagByScreeningId = latestDiagnoses
            .GroupBy(d => d.AiScreeningId)
            .ToDictionary(g => g.Key, g => g.First());

        var list = new List<OrganisationRecentPatientReadModel>(orderedPatients.Count);

        foreach (var row in orderedPatients)
        {
            var p = row.Patient;
            var u = row.User;
            var scr = row.LatestScreening;

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
