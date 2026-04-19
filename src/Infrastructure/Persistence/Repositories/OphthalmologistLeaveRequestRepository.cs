using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OphthalmologistLeaveRequestRepository : Repository<OphthalmologistLeaveRequest>, IOphthalmologistLeaveRequestRepository
{
    public OphthalmologistLeaveRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<OphthalmologistLeaveRequest> Items, int TotalCount)> GetByOphthalmologistPagedAsync(
        Guid ophthalmologistId,
        OphthalmologistLeaveRequestStatus? status = null,
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

    public async Task<(IReadOnlyList<OphthalmologistLeaveRequest> Items, int TotalCount)> GetPagedAsync(
        OphthalmologistLeaveRequestStatus? status = null,
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

    public async Task<bool> HasOverlappingActiveRequestAsync(
        Guid ophthalmologistId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludeLeaveRequestId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(x => x.OphthalmologistId == ophthalmologistId)
            .Where(x => x.Status == OphthalmologistLeaveRequestStatus.Pending
                        || x.Status == OphthalmologistLeaveRequestStatus.Approved)
            .Where(x => x.StartDate <= endDate && x.EndDate >= startDate);

        if (excludeLeaveRequestId.HasValue)
        {
            query = query.Where(x => x.Id != excludeLeaveRequestId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OphthalmologistLeaveRequest>> GetApprovedOverlappingAsync(
        Guid ophthalmologistId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.OphthalmologistId == ophthalmologistId)
            .Where(x => x.Status == OphthalmologistLeaveRequestStatus.Approved)
            .Where(x => x.StartDate <= toDate && x.EndDate >= fromDate)
            .OrderBy(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }
}