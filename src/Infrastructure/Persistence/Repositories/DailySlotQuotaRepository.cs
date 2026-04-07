using Domain.Entities.Scheduling;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class DailySlotQuotaRepository : IDailySlotQuotaRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<DailySlotQuota> _dbSet;

    public DailySlotQuotaRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<DailySlotQuota>();
    }

    public async Task<DailySlotQuota> GetOrCreateForUpdateAsync(
        DateOnly date,
        int quotaSnapshot,
        CancellationToken cancellationToken = default)
    {
        if (quotaSnapshot < 1)
            throw new ArgumentOutOfRangeException(nameof(quotaSnapshot), "Quota must be at least 1.");

        var utcNow = DateTime.UtcNow;
        const int initialCount = 0;

        await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"INSERT INTO ""DailySlotQuotas"" (""Date"", ""PartTimeSlotCount"", ""QuotaSnapshot"", ""UpdatedAt"")
               VALUES ({date}, {initialCount}, {quotaSnapshot}, {utcNow})
               ON CONFLICT (""Date"") DO NOTHING",
            cancellationToken);

        var quota = await _dbSet
            .FromSqlInterpolated($@"SELECT * FROM ""DailySlotQuotas"" WHERE ""Date"" = {date} FOR UPDATE")
            .AsTracking()
            .SingleAsync(cancellationToken);

        quota.RefreshQuotaSnapshot(quotaSnapshot);
        return quota;
    }

    public async Task<(bool Success, DailySlotQuota Quota)> TryReserveAsync(
        DateOnly date,
        int slotsToReserve,
        int quotaSnapshot,
        CancellationToken cancellationToken = default)
    {
        var quota = await GetOrCreateForUpdateAsync(date, quotaSnapshot, cancellationToken);
        if (!quota.CanReserve(slotsToReserve))
        {
            return (false, quota);
        }

        quota.Reserve(slotsToReserve);
        return (true, quota);
    }

    public async Task<IReadOnlyList<DailySlotQuota>> GetByDateRangeAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(q => q.Date >= fromDate && q.Date <= toDate)
            .OrderBy(q => q.Date)
            .ToListAsync(cancellationToken);
    }
}