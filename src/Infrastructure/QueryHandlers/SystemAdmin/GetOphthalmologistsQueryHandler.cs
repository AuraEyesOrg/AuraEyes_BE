using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;
using Domain.Enums;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers.SystemAdmin;

/// <summary>
/// Handler for GetOphthalmologistsQuery.
/// Queries Ophthalmologists joined with ApplicationUser for full details.
/// </summary>
public class GetOphthalmologistsQueryHandler
    : IQueryHandler<GetOphthalmologistsQuery, PagedResult<OphthalmologistListDto>>
{
    private readonly ApplicationDbContext _context;

    public GetOphthalmologistsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<OphthalmologistListDto>>> Handle(
        GetOphthalmologistsQuery request,
        CancellationToken cancellationToken)
    {
        var query = from o in _context.Ophthalmologists.AsNoTracking()
                    join u in _context.Users.AsNoTracking() on o.UserId equals u.Id
                    join org in _context.Organisations.AsNoTracking() on u.OrganizationId equals org.Id into orgJoin
                    from org in orgJoin.DefaultIfEmpty()
                    select new { Ophthalmologist = o, User = u, Organisation = org };

        // Search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(x =>
                x.User.FullName.ToLower().Contains(term) ||
                x.User.Email!.ToLower().Contains(term) ||
                (x.Ophthalmologist.Phone != null && x.Ophthalmologist.Phone.Contains(term)));
        }

        // Verification status filter
        if (!string.IsNullOrWhiteSpace(request.VerificationStatus))
        {
            if (Enum.TryParse<VerificationStatus>(request.VerificationStatus, true, out var status))
            {
                query = query.Where(x => x.Ophthalmologist.VerificationStatus == status);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.Ophthalmologist.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new OphthalmologistListDto
            {
                Id = x.Ophthalmologist.Id,
                UserId = x.User.Id,
                FullName = x.User.FullName,
                Email = x.User.Email!,
                Phone = x.Ophthalmologist.Phone,
                Bio = x.Ophthalmologist.Bio,
                YearsOfExperience = x.Ophthalmologist.YearsOfExperience,
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

        var pagedResult = new PagedResult<OphthalmologistListDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<OphthalmologistListDto>>.Success(pagedResult);
    }
}
