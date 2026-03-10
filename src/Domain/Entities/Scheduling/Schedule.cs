using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// Schedule entity - a patient booking against an Availability slot.
/// Doctor and Clinic are now resolved via the parent Availability (3NF).
/// </summary>
public class Schedule : BaseEntity, IAggregateRoot
{
    /// <summary>FK to Availability — the opening slot this booking fills.</summary>
    public Guid AvailabilityId { get; private set; }
    public Guid PatientId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public ScheduleStatus Status { get; private set; }
    public SlotType SlotType { get; private set; }
    public decimal? Cost { get; private set; }

    /// <summary>Navigation property — resolves doctor/clinic without extra FKs.</summary>
    public Availability? Availability { get; private set; }

    private Schedule() { } // EF Core

    public Schedule(Guid availabilityId, Guid patientId, DateOnly date, TimeOnly startTime, TimeOnly endTime, SlotType slotType, decimal? cost = null)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");

        AvailabilityId = availabilityId;
        PatientId = patientId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        SlotType = slotType;
        Cost = cost;
        Status = ScheduleStatus.Available;
    }

    public void Book()
    {
        if (Status != ScheduleStatus.Available)
            throw new InvalidOperationException("Schedule is not available for booking");

        Status = ScheduleStatus.Booked;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = ScheduleStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != ScheduleStatus.Booked)
            throw new InvalidOperationException("Only booked schedules can be completed");

        Status = ScheduleStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkNoShow()
    {
        if (Status != ScheduleStatus.Booked)
            throw new InvalidOperationException("Only booked schedules can be marked as no-show");

        Status = ScheduleStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCost(decimal? cost)
    {
        Cost = cost;
        UpdatedAt = DateTime.UtcNow;
    }
}
