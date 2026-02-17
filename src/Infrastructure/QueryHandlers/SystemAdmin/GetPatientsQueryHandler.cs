using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Patients.Queries.GetPatients;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers.SystemAdmin;

/// <summary>
/// Handler for GetPatientsQuery - queries real patient data joined with ApplicationUser.
/// </summary>
public class GetPatientsQueryHandler : IQueryHandler<GetPatientsQuery, PagedResult<PatientListDto>>
{
    private readonly ApplicationDbContext _context;

    public GetPatientsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<PatientListDto>>> Handle(
        GetPatientsQuery request,
        CancellationToken cancellationToken)
    {
        var query = from p in _context.Patients.AsNoTracking()
                    join u in _context.Users.AsNoTracking() on p.UserId equals u.Id
                    where !u.IsDeleted
                    select new { Patient = p, User = u };

        // Search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(x =>
                x.User.FullName.ToLower().Contains(term) ||
                x.User.Email!.ToLower().Contains(term));
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = request.Status.ToLower() switch
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
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new PatientListDto
            {
                Id = x.Patient.Id,
                UserId = x.User.Id,
                FullName = x.User.FullName,
                Email = x.User.Email!,
                Phone = x.User.PhoneNumber,
                MedicalHistorySummary = x.Patient.MedicalHistorySummary,
                IsActive = x.User.IsActive,
                EmailConfirmed = x.User.EmailConfirmed,
                CreatedAt = x.Patient.CreatedAt,
                LastLoginAt = x.User.LastLoginAt
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<PatientListDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<PatientListDto>>.Success(pagedResult);
    }
}
