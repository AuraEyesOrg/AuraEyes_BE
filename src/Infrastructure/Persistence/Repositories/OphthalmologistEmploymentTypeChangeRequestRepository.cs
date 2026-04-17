using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OphthalmologistEmploymentTypeChangeRequestRepository : Repository<OphthalmologistEmploymentTypeChangeRequest>, IOphthalmologistEmploymentTypeChangeRequestRepository
{
    public OphthalmologistEmploymentTypeChangeRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<OphthalmologistEmploymentTypeChangeRequest> Items, int TotalCount)> GetByOphthalmologistPagedAsync(
        Guid ophthalmologistId,
        OphthalmologistEmploymentTypeChangeRequestStatus? status = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(x => x.OphthalmologistId == ophthalmologistId)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<OphthalmologistEmploymentTypeChangeRequest> Items, int TotalCount)> GetPagedAsync(
        OphthalmologistEmploymentTypeChangeRequestStatus? status = null,
        Guid? ophthalmologistId = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (ophthalmologistId.HasValue)
        {
            query = query.Where(x => x.OphthalmologistId == ophthalmologistId.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> HasPendingRequestAsync(
        Guid ophthalmologistId,
        Guid? excludeRequestId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(x => x.OphthalmologistId == ophthalmologistId)
            .Where(x => x.Status == OphthalmologistEmploymentTypeChangeRequestStatus.Pending);

        if (excludeRequestId.HasValue)
        {
            query = query.Where(x => x.Id != excludeRequestId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
