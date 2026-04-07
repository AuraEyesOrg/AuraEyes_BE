using Domain.Common;
using Domain.Enums;

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

    /// <summary>Default cost for slots generated from this template (optional).</summary>
    public decimal? Cost { get; private set; }

    /// <summary>Template source (doctor-defined or system-generated).</summary>
    public ScheduleTemplateSource Source { get; private set; }

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
        Guid? ophthalId = null,
        decimal? cost = null,
        ScheduleTemplateSource source = ScheduleTemplateSource.Doctor)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");
        if (slotDuration < 1)
            throw new ArgumentException("Slot duration must be at least 1 minute", nameof(slotDuration));
        if (maxCapacity < 1)
            throw new ArgumentException("Max capacity must be at least 1", nameof(maxCapacity));
        if (orgId is null && ophthalId is null)
            throw new ArgumentException("At least one of OrgId or OphthalId must be provided");
        if (orgId.HasValue && ophthalId.HasValue)
            throw new ArgumentException("OrgId must be null when OphthalId is provided.");
        if (cost.HasValue && cost.Value < 0)
            throw new ArgumentException("Cost cannot be negative", nameof(cost));

        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        SlotDuration = slotDuration;
        MaxCapacity = maxCapacity;
        Cost = cost;
        OrgId = orgId;
        OphthalId = ophthalId;
        Source = source;
    }

    public void Update(
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int slotDuration,
        int maxCapacity,
        decimal? cost)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");
        if (slotDuration < 1)
            throw new ArgumentException("Slot duration must be at least 1 minute", nameof(slotDuration));
        if (maxCapacity < 1)
            throw new ArgumentException("Max capacity must be at least 1", nameof(maxCapacity));
        if (cost.HasValue && cost.Value < 0)
            throw new ArgumentException("Cost cannot be negative", nameof(cost));

        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        SlotDuration = slotDuration;
        MaxCapacity = maxCapacity;
        Cost = cost;
        UpdatedAt = DateTime.UtcNow;
    }
}
