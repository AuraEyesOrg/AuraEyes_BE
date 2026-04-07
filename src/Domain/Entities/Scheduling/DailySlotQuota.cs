namespace Domain.Entities.Scheduling;

/// <summary>
/// Stores the globally reserved part-time slot count for a specific date.
/// </summary>
public class DailySlotQuota
{
    public DateOnly Date { get; private set; }
    public int PartTimeSlotCount { get; private set; }
    public int QuotaSnapshot { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private DailySlotQuota() { }

    public DailySlotQuota(DateOnly date, int quotaSnapshot)
    {
        if (quotaSnapshot < 1)
            throw new ArgumentOutOfRangeException(nameof(quotaSnapshot), "Quota must be at least 1.");

        Date = date;
        QuotaSnapshot = quotaSnapshot;
        PartTimeSlotCount = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public int Remaining => Math.Max(0, QuotaSnapshot - PartTimeSlotCount);

    public void RefreshQuotaSnapshot(int quotaSnapshot)
    {
        if (quotaSnapshot < 1)
            throw new ArgumentOutOfRangeException(nameof(quotaSnapshot), "Quota must be at least 1.");

        QuotaSnapshot = quotaSnapshot;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanReserve(int slots)
    {
        if (slots < 1)
            return false;

        return PartTimeSlotCount + slots <= QuotaSnapshot;
    }

    public void Reserve(int slots)
    {
        if (slots < 1)
            throw new ArgumentOutOfRangeException(nameof(slots), "Reserved slots must be at least 1.");

        if (!CanReserve(slots))
            throw new InvalidOperationException("Daily slot quota for part-time doctors has been reached");

        PartTimeSlotCount += slots;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Release(int slots)
    {
        if (slots < 1)
            throw new ArgumentOutOfRangeException(nameof(slots), "Released slots must be at least 1.");

        if (slots > PartTimeSlotCount)
            throw new InvalidOperationException("Cannot release more slots than currently reserved.");

        PartTimeSlotCount -= slots;
        UpdatedAt = DateTime.UtcNow;
    }
}