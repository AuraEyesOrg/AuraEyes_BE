namespace Domain.Enums;

public enum ScheduleStatus
{
    Available = 1,
    Booked = 2,
    Cancelled = 3,
    Completed = 4,
    NoShow = 5,
    /// <summary>Slot is temporarily reserved by a patient (pending payment).</summary>
    Reserved = 6,
    /// <summary>Slot is blocked by doctor (not available for booking).</summary>
    Blocked = 7
}
