namespace Application.SystemSettings.Interfaces;

public interface ISystemSettingService
{
    Task<string?> GetSettingAsync(string key, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetAllSettingsAsync(CancellationToken cancellationToken = default);
    Task UpdateSettingsAsync(Dictionary<string, string> settings, CancellationToken cancellationToken = default);

    Task<(bool Success, int UsedSlots, int Quota, int RemainingSlots)> TryReservePartTimeSlotsAsync(
        DateOnly date,
        int slotsToReserve,
        int quotaSnapshot,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<DateOnly, int>> GetPartTimeReservedSlotsByDateRangeAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);
}
