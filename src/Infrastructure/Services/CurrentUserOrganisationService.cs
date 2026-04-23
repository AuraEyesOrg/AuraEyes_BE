using Application.Common.Interfaces;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace Infrastructure.Services;

/// <summary>
/// Resolves the Organisation that the currently authenticated user belongs to.
/// Works for both OrgAdmin (ApplicationUser.OrganizationId) and ClinicStaff
/// (also stored in ApplicationUser.OrganizationId when assigned to a clinic).
/// </summary>
public sealed class CurrentUserOrganisationService : ICurrentUserOrganisationService
{
    private readonly ApplicationDbContext _context;

    public CurrentUserOrganisationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid?> GetOrganisationIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == userId && !u.IsDeleted)
            .Select(u => u.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
