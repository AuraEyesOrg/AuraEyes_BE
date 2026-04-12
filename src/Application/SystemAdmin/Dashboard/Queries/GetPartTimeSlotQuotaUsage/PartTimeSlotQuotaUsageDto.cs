namespace Application.SystemAdmin.Dashboard.Queries.GetPartTimeSlotQuotaUsage;

public class PartTimeSlotQuotaUsageDto
{
    public DateOnly Date { get; init; }
    public int UsedSlots { get; init; }
    public int Quota { get; init; }
    public int RemainingSlots { get; init; }
}