namespace Domain.Enums;

/// <summary>
/// Slot availability status for the clinic-centric scheduling model.
/// </summary>
public enum ScheduleStatus
{
    /// <summary>Slot is open for booking.</summary>
    Available = 1,

    /// <summary>Slot is blocked by staff (not available for booking).</summary>
    Blocked = 7
}
