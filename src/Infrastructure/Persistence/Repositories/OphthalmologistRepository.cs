using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Ophthalmologist aggregate root.
/// </summary>
public class OphthalmologistRepository : Repository<Ophthalmologist>, IOphthalmologistRepository
{
    public OphthalmologistRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Ophthalmologist?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Certificates)
            .FirstOrDefaultAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<Ophthalmologist?> GetByIdWithCertificatesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Certificates)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Ophthalmologist> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(o => o.Certificates)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lower = searchTerm.ToLower();
            query = query
                .Join(_context.Users, o => o.UserId, u => u.Id, (o, u) => new { o, u })
                .Where(x => x.u.FullName.ToLower().Contains(lower) ||
                            x.u.Email.ToLower().Contains(lower) ||
                            x.o.Phone.ToLower().Contains(lower))
                .Select(x => x.o);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, string>> GetDisplayNamesByIdsAsync(
        IReadOnlyCollection<Guid> ophthalmologistIds,
        CancellationToken cancellationToken = default)
    {
        if (ophthalmologistIds.Count == 0)
            return new Dictionary<Guid, string>();

        var uniqueIds = ophthalmologistIds.Distinct().ToArray();

        var rows = await (
            from ophthalmologist in _dbSet
            join user in _context.Users on ophthalmologist.UserId equals user.Id
            where uniqueIds.Contains(ophthalmologist.Id) && !user.IsDeleted
            select new
            {
                ophthalmologist.Id,
                user.FullName
            })
            .ToListAsync(cancellationToken);

        return rows
            .Where(x => !string.IsNullOrWhiteSpace(x.FullName))
            .ToDictionary(x => x.Id, x => x.FullName);
    }

    public async Task<IReadOnlyDictionary<Guid, (string FullName, string? AvatarUrl)>> GetDoctorDetailsByIdsAsync(
        IReadOnlyCollection<Guid> ophthalmologistIds,
        CancellationToken cancellationToken = default)
    {
        if (ophthalmologistIds.Count == 0)
            return new Dictionary<Guid, (string FullName, string? AvatarUrl)>();

        var uniqueIds = ophthalmologistIds.Distinct().ToArray();

        var rows = await (
            from ophthalmologist in _dbSet
            join user in _context.Users on ophthalmologist.UserId equals user.Id
            where uniqueIds.Contains(ophthalmologist.Id) && !user.IsDeleted
            select new
            {
                ophthalmologist.Id,
                user.FullName,
                user.AvatarUrl
            })
            .ToListAsync(cancellationToken);

        return rows
            .ToDictionary(
                x => x.Id,
                x => (FullName: x.FullName ?? "Unknown", AvatarUrl: (string?)x.AvatarUrl));
    }
    public async Task<IReadOnlyDictionary<Guid, EnhancedDoctorDetail>> GetEnhancedDoctorDetailsByIdsAsync(
        IReadOnlyCollection<Guid> ophthalmologistIds,
        CancellationToken cancellationToken = default)
    {
        if (ophthalmologistIds.Count == 0)
            return new Dictionary<Guid, EnhancedDoctorDetail>();

        var uniqueIds = ophthalmologistIds.Distinct().ToArray();

        var query = from ophthalmologist in _dbSet.Include(o => o.Certificates)
                    join user in _context.Users on ophthalmologist.UserId equals user.Id
                    where uniqueIds.Contains(ophthalmologist.Id) && !user.IsDeleted
                    select new
                    {
                        ophthalmologist.Id,
                        user.FullName,
                        user.AvatarUrl,
                        ophthalmologist.Bio,
                        ophthalmologist.RatingAverage,
                        ophthalmologist.RatingCount,
                        Certificates = ophthalmologist.Certificates.ToList()
                    };

        var rows = await query.ToListAsync(cancellationToken);

        return rows.ToDictionary(
            x => x.Id,
            x => new EnhancedDoctorDetail(
                x.FullName ?? "Unknown",
                x.AvatarUrl,
                x.Bio,
                x.RatingAverage,
                x.RatingCount,
                x.Certificates));
    }
}
