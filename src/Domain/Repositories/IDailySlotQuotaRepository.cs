using Domain.Entities.Scheduling;

namespace Domain.Repositories;

public interface IDailySlotQuotaRepository
{
    Task<DailySlotQuota> GetOrCreateForUpdateAsync(
        DateOnly date,
        int quotaSnapshot,
        CancellationToken cancellationToken = default);

    Task<(bool Success, DailySlotQuota Quota)> TryReserveAsync(
        DateOnly date,
        int slotsToReserve,
        int quotaSnapshot,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DailySlotQuota>> GetByDateRangeAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);
}