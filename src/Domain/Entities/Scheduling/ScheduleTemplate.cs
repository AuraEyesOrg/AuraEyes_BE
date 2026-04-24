using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// ScheduleTemplate - A clinic-level recurring availability pattern.
/// Defines when slots are available (e.g., "Every Monday 9am-5pm, 30min slots").
/// Single-clinic model: no OrgId/OphthalId ownership.
/// </summary>
public class ScheduleTemplate : BaseEntity, IAggregateRoot
{
    /// <summary>FK to the Ophthalmologist owner (for personal schedules).</summary>
    public Guid? OphthalId { get; private set; }

    /// <summary>FK to the Organisation (if it's an organisation-level template).</summary>
    public Guid? OrgId { get; private set; }

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

    /// <summary>The cost/fee for slots generated from this template.</summary>
    public decimal? Cost { get; private set; }

    /// <summary>Template source (staff-defined or system-generated).</summary>
    public ScheduleTemplateSource Source { get; private set; }

    /// <summary>Indicates whether this template is active and can be used for slot generation.</summary>
    public bool IsActive { get; private set; }

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
        decimal? cost = null,
        Guid? ophthalId = null,
        Guid? orgId = null,
        ScheduleTemplateSource source = ScheduleTemplateSource.Doctor)
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
        Cost = cost;
        OphthalId = ophthalId;
        OrgId = orgId;
        Source = source;
        IsActive = true;
    }

    public void Update(
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int slotDuration,
        int maxCapacity,
        decimal? cost = null,
        Guid? ophthalId = null,
        Guid? orgId = null,
        bool isActive = true)
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
        Cost = cost;
        OphthalId = ophthalId;
        OrgId = orgId;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
