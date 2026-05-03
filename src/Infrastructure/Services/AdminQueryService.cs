using Application.SystemAdmin.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.AuditLogs.Queries.GetAuditLogs;
using Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;
using Application.SystemAdmin.Patients.Queries.GetPatientMetrics;
using Application.SystemAdmin.Patients.Queries.GetPatients;
using Domain.Enums;
using Infrastructure.Persistence;
using Application.Common.Constants;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

/// <summary>
/// Implementation of IAdminQueryService using ApplicationDbContext
/// for queries that require joining Domain entities with Identity data.
/// </summary>
public class AdminQueryService : IAdminQueryService
{
    private readonly ApplicationDbContext _context;

    public AdminQueryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<OphthalmologistListDto>> GetOphthalmologistsAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = from o in _context.Ophthalmologists.AsNoTracking()
                    join u in _context.Users.AsNoTracking() on o.UserId equals u.Id
                    select new { Ophthalmologist = o, User = u };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.User.FullName, term) ||
                (x.User.Email != null && EF.Functions.ILike(x.User.Email, term)) ||
                (x.Ophthalmologist.Phone != null && EF.Functions.ILike(x.Ophthalmologist.Phone, term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pageRows = await query
            .OrderByDescending(x => x.Ophthalmologist.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                OphthalmologistId = x.Ophthalmologist.Id,
                UserId = x.User.Id,
                FullName = x.User.FullName,
                Email = x.User.Email!,
                Phone = x.Ophthalmologist.Phone,
                Bio = x.Ophthalmologist.Bio,
                EmploymentType = x.Ophthalmologist.EmploymentType.ToString(),
                LicenseUrl = x.Ophthalmologist.LicenseUrl,
                DegreeUrl = x.Ophthalmologist.DegreeUrl,
                IsActive = x.User.IsActive,
                AvailableLeaveDays = x.Ophthalmologist.AvailableLeaveDays,
                ConsultationFee = x.Ophthalmologist.ConsultationFee,
                CreatedAt = x.Ophthalmologist.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var ophthalmologistIds = pageRows
            .Select(x => x.OphthalmologistId)
            .ToList();

        var credentialRows = await _context.Certificates
            .AsNoTracking()
            .Where(c => ophthalmologistIds.Contains(c.OphthalmologistId))
            .Select(c => new OphthalmologistCredentialProjection
            {
                OphthalmologistId = c.OphthalmologistId,
                Type = c.Type,
                Credential = new OphthalmologistCredentialDto
                {
                    Id = c.Id,
                    DegreeLevel = c.DegreeLevel.HasValue ? c.DegreeLevel.Value.ToString() : null,
                    Name = c.Name,
                    IssuingAuthority = c.IssuingAuthority,
                    IssuedDate = c.IssuedDate,
                    ExpiryDate = c.ExpiryDate,
                    CertificateUrl = c.CertificateUrl
                }
            })
            .ToListAsync(cancellationToken);

        var credentialsByOphthalmologist = credentialRows
            .GroupBy(x => x.OphthalmologistId)
            .ToDictionary(
                g => g.Key,
                g => g.ToList());

        var items = pageRows
            .Select(row => MapToOphthalmologistListDto(row, credentialsByOphthalmologist))
            .ToList();

        return new PagedResult<OphthalmologistListDto>(
            items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResult<PatientListDto>> GetPatientsAsync(
        string? searchTerm,
        string? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = from p in _context.Patients.AsNoTracking()
                    join u in _context.Users.AsNoTracking() on p.UserId equals (Guid?)u.Id into userJoin
                    from u in userJoin.DefaultIfEmpty()
                    where u == null || !u.IsDeleted
                    select new PatientProjection
                    {
                        Patient = p,
                        User = u
                    };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.User != null ? x.User.FullName : (x.Patient.FullName ?? string.Empty), term) ||
                (x.User != null && x.User.Email != null && EF.Functions.ILike(x.User.Email, term)) ||
                EF.Functions.ILike(x.User != null ? (x.User.PhoneNumber ?? string.Empty) : (x.Patient.PhoneNumber ?? string.Empty), term));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = ApplyPatientStatusFilter(query, status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Patient.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PatientListDto
            {
                Id = x.Patient.Id,
                UserId = x.Patient.UserId,
                FullName = x.User != null ? x.User.FullName : (x.Patient.FullName ?? "Walk-in Patient"),
                Email = x.User != null ? x.User.Email : null,
                Phone = x.User != null ? x.User.PhoneNumber : x.Patient.PhoneNumber,
                BMI = x.Patient.BMI,
                DiseaseHistory = x.Patient.DiseaseHistory,
                IsActive = x.User == null || x.User.IsActive,
                EmailConfirmed = x.User != null && x.User.EmailConfirmed,
                IsWalkIn = x.Patient.UserId == null,
                PatientType = x.Patient.UserId == null ? "WalkIn" : "Registered",
                CreatedAt = x.Patient.CreatedAt,
                LastLoginAt = x.User != null ? x.User.LastLoginAt : null
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PatientListDto>(items, totalCount, pageNumber, pageSize);
    }

    private static IQueryable<PatientProjection> ApplyPatientStatusFilter(IQueryable<PatientProjection> query, string status)
    {
        return status.ToLower() switch
        {
            "active" => query.Where(x => x.User != null && x.User.IsActive),
            "pending" => query.Where(x => x.User != null && !x.User.EmailConfirmed),
            "locked" or "inactive" or "suspended" => query.Where(x => x.User != null && !x.User.IsActive),
            "walkin" => query.Where(x => x.Patient.UserId == null),
            "registered" => query.Where(x => x.Patient.UserId != null),
            _ => query
        };
    }

    private sealed class PatientProjection
    {
        public Domain.Entities.Users.Patient Patient { get; init; } = null!;
        public Infrastructure.Identity.ApplicationUser? User { get; init; }
    }


    public async Task<PatientMetricsDto> GetPatientMetricsAsync(
        CancellationToken cancellationToken = default)
    {
        var totalPatients = await _context.Patients
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var walkInPatients = await _context.Patients
            .AsNoTracking()
            .CountAsync(x => x.UserId == null, cancellationToken);

        var activeRegisteredPatients = await (
            from patient in _context.Patients.AsNoTracking()
            join user in _context.Users.AsNoTracking() on patient.UserId equals (Guid?)user.Id
            where !user.IsDeleted && user.IsActive
            select patient.Id)
            .CountAsync(cancellationToken);

        var lockedRegisteredPatients = await (
            from patient in _context.Patients.AsNoTracking()
            join user in _context.Users.AsNoTracking() on patient.UserId equals (Guid?)user.Id
            where !user.IsDeleted && !user.IsActive
            select patient.Id)
            .CountAsync(cancellationToken);

        return new PatientMetricsDto
        {
            TotalPatients = totalPatients,
            WalkInPatients = walkInPatients,
            RegisteredPatients = Math.Max(totalPatients - walkInPatients, 0),
            ActiveRegisteredPatients = activeRegisteredPatients,
            LockedRegisteredPatients = lockedRegisteredPatients
        };
    }

    public async Task<PagedResult<AuditLogDto>> GetAuditLogsAsync(
        string? searchTerm,
        string? action,
        string? entityName,
        Guid? userId,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = from log in _context.AuditLogs.AsNoTracking()
                    join u in _context.Users.AsNoTracking() on log.UserId equals u.Id into userJoin
                    from u in userJoin.DefaultIfEmpty()
                    select new { Log = log, User = u };

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.Log.Action, term) ||
                EF.Functions.ILike(x.Log.EntityName, term) ||
                (x.Log.EntityId != null && EF.Functions.ILike(x.Log.EntityId, term)) ||
                (x.User != null && x.User.Email != null && EF.Functions.ILike(x.User.Email, term)));
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(x => x.Log.Action == action);
        }

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            query = query.Where(x => x.Log.EntityName == entityName);
        }

        if (userId.HasValue)
        {
            query = query.Where(x => x.Log.UserId == userId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.Log.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.Log.CreatedAt <= toDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Log.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AuditLogDto
            {
                Id = x.Log.Id,
                UserId = x.Log.UserId,
                UserName = x.User != null ? x.User.Email : null,
                Action = x.Log.Action,
                EntityName = x.Log.EntityName,
                EntityId = x.Log.EntityId,
                OldValue = x.Log.OldValue,
                NewValue = x.Log.NewValue,
                IpAddress = x.Log.IpAddress,
                CreatedAt = x.Log.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLogDto>(items, totalCount, pageNumber, pageSize);
    }

    private static OphthalmologistListDto MapToOphthalmologistListDto(
        dynamic row,
        Dictionary<Guid, List<OphthalmologistCredentialProjection>> credentialsByOphthalmologist)
    {
        var credentials = credentialsByOphthalmologist.TryGetValue((Guid)row.OphthalmologistId, out List<OphthalmologistCredentialProjection>? mapped)
            ? mapped
            : new List<OphthalmologistCredentialProjection>();

        var licenses = credentials
            .Where(c => c.Type == CertificateType.License)
            .Select(c => c.Credential)
            .ToList();

        var degrees = credentials
            .Where(c => c.Type == CertificateType.Degree)
            .Select(c => c.Credential)
            .ToList();

        return new OphthalmologistListDto
        {
            Id = row.OphthalmologistId,
            UserId = row.UserId,
            FullName = row.FullName,
            Email = row.Email,
            Phone = row.Phone,
            Bio = row.Bio,
            EmploymentType = row.EmploymentType,
            LicenseUrl = row.LicenseUrl,
            DegreeUrl = row.DegreeUrl,
            Licenses = licenses,
            Degrees = degrees,
            OrganisationName = null,
            IsActive = row.IsActive,
            AvailableLeaveDays = row.AvailableLeaveDays,
            ConsultationFee = row.ConsultationFee,
            CreatedAt = row.CreatedAt
        };
    }

    private sealed class OphthalmologistCredentialProjection
    {
        public Guid OphthalmologistId { get; init; }
        public CertificateType Type { get; init; }
        public OphthalmologistCredentialDto Credential { get; init; } = new();
    }
}
