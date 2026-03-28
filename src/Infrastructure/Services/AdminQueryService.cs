using Application.SystemAdmin.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.AuditLogs.Queries.GetAuditLogs;
using Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;
using Application.SystemAdmin.Patients.Queries.GetPatients;
using Domain.Enums;
using Infrastructure.Persistence;
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
        string? verificationStatus,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = from o in _context.Ophthalmologists.AsNoTracking()
                    join u in _context.Users.AsNoTracking() on o.UserId equals u.Id
                    join org in _context.Organisations.AsNoTracking() on u.OrganizationId equals org.Id into orgJoin
                    from org in orgJoin.DefaultIfEmpty()
                    select new { Ophthalmologist = o, User = u, Organisation = org };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.User.FullName, term) ||
                (x.User.Email != null && EF.Functions.ILike(x.User.Email, term)) ||
                (x.Ophthalmologist.Phone != null && EF.Functions.ILike(x.Ophthalmologist.Phone, term)));
        }

        if (!string.IsNullOrWhiteSpace(verificationStatus))
        {
            if (Enum.TryParse<VerificationStatus>(verificationStatus, true, out var status))
            {
                query = query.Where(x => x.Ophthalmologist.VerificationStatus == status);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Ophthalmologist.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new OphthalmologistListDto
            {
                Id = x.Ophthalmologist.Id,
                UserId = x.User.Id,
                FullName = x.User.FullName,
                Email = x.User.Email!,
                Phone = x.Ophthalmologist.Phone,
                Bio = x.Ophthalmologist.Bio,
                YearsOfExperience = x.Ophthalmologist.YearsOfExperience,
                EmploymentType = x.Ophthalmologist.EmploymentType.ToString(),
                WorkingHoursPerWeek = x.Ophthalmologist.WorkingHoursPerWeek,
                ExpectedMonthlySalary = x.Ophthalmologist.ExpectedMonthlySalary,
                CommissionRate = x.Ophthalmologist.CommissionRate,
                ActualMonthlySalary = x.Ophthalmologist.ActualMonthlySalary,
                VerificationStatus = x.Ophthalmologist.VerificationStatus.ToString(),
                IsVerified = x.Ophthalmologist.IsVerified,
                LicenseUrl = x.Ophthalmologist.LicenseUrl,
                DegreeUrl = x.Ophthalmologist.DegreeUrl,
                RejectionReason = x.Ophthalmologist.RejectionReason,
                OrganisationName = x.Organisation != null ? x.Organisation.Name : null,
                IsActive = x.User.IsActive,
                CreatedAt = x.Ophthalmologist.CreatedAt
            })
            .ToListAsync(cancellationToken);

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
                    join u in _context.Users.AsNoTracking() on p.UserId equals u.Id
                    where !u.IsDeleted
                    select new { Patient = p, User = u };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.User.FullName, term) ||
                (x.User.Email != null && EF.Functions.ILike(x.User.Email, term)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = status.ToLower() switch
            {
                "active" => query.Where(x => x.User.IsActive && x.User.EmailConfirmed),
                "pending" => query.Where(x => !x.User.EmailConfirmed),
                "suspended" => query.Where(x => !x.User.IsActive),
                _ => query
            };
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Patient.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PatientListDto
            {
                Id = x.Patient.Id,
                UserId = x.User.Id,
                FullName = x.User.FullName,
                Email = x.User.Email!,
                Phone = x.User.PhoneNumber,
                BMI = x.Patient.BMI,
                DiseaseHistory = x.Patient.DiseaseHistory,
                IsActive = x.User.IsActive,
                EmailConfirmed = x.User.EmailConfirmed,
                CreatedAt = x.Patient.CreatedAt,
                LastLoginAt = x.User.LastLoginAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PatientListDto>(
            items, totalCount, pageNumber, pageSize);
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
}
