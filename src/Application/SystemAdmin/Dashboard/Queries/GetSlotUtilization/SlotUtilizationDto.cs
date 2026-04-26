namespace Application.SystemAdmin.Dashboard.Queries.GetSlotUtilization;

public class SlotUtilizationDto
{
    public int TotalSlots { get; set; }
    public int BookedSlots { get; set; }
    public int RemainingCapacity { get; set; }
    public decimal UtilizationRate { get; set; }
}
