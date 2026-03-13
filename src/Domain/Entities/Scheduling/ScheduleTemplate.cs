using Domain.Common;

namespace Domain.Entities.Scheduling;

/// <summary>
/// ScheduleTemplate - A recurring availability pattern for a doctor or organization.
/// Defines when slots are available (e.g., "Every Monday 9am-5pm, 30min slots").
/// </summary>
public class ScheduleTemplate : BaseEntity, IAggregateRoot
{
    /// <summary>FK to Organisation (nullable - can be a solo doctor template).</summary>
    public Guid? OrgId { get; private set; }

    /// <summary>FK to Ophthalmologist (nullable - can be an org-only template).</summary>
    public Guid? OphthalId { get; private set; }

    /// <summary>Day of week (0=Sunday, 1=Monday, etc.).</summary>
    public DayOfWeek DayOfWeek { get; private set; }

    /// <summary>Start time of the availability window.</summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>End time of the availability window.</summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>Duration of each appointment slot in minutes.</summary>
    public int SlotDuration { get; private set; }

    /// <summary>Maximum concurrent patients per slot.</summary>
    public int MaxCapacity { get; private set; }

    // Navigation
    private readonly List<AppointmentSlot> _appointmentSlots = new();
    public IReadOnlyCollection<AppointmentSlot> AppointmentSlots => _appointmentSlots.AsReadOnly();

    private ScheduleTemplate() { } // EF Core

    public ScheduleTemplate(
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int slotDuration,
        int maxCapacity,
        Guid? orgId = null,
        Guid? ophthalId = null)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");
        if (slotDuration < 1)
            throw new ArgumentException("Slot duration must be at least 1 minute", nameof(slotDuration));
        if (maxCapacity < 1)
            throw new ArgumentException("Max capacity must be at least 1", nameof(maxCapacity));
        if (orgId is null && ophthalId is null)
            throw new ArgumentException("At least one of OrgId or OphthalId must be provided");

        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        SlotDuration = slotDuration;
        MaxCapacity = maxCapacity;
        OrgId = orgId;
        OphthalId = ophthalId;
    }

    public void Update(
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int slotDuration,
        int maxCapacity)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");
        if (slotDuration < 1)
            throw new ArgumentException("Slot duration must be at least 1 minute", nameof(slotDuration));
        if (maxCapacity < 1)
            throw new ArgumentException("Max capacity must be at least 1", nameof(maxCapacity));

        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        SlotDuration = slotDuration;
        MaxCapacity = maxCapacity;
        UpdatedAt = DateTime.UtcNow;
    }
}
