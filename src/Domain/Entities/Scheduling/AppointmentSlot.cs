using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// AppointmentSlot - A specific time slot for a specific date, generated from a ScheduleTemplate.
/// Tracks bookings and availability.
/// </summary>
public class AppointmentSlot : BaseEntity, IAggregateRoot
{
    /// <summary>FK to ScheduleTemplate - the template this slot was generated from.</summary>
    public Guid ScheduleTemplateId { get; private set; }

    /// <summary>Date of this appointment slot.</summary>
    public DateOnly Date { get; private set; }

    /// <summary>Start time of this slot.</summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>End time of this slot.</summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>Status of this slot.</summary>
    public ScheduleStatus Status { get; private set; }

    /// <summary>Cost of the appointment (optional).</summary>
    public decimal? Cost { get; private set; }

    /// <summary>Type of appointment slot.</summary>
    public SlotType SlotType { get; private set; }

    /// <summary>Number of patients currently booked in this slot.</summary>
    public int BookedCount { get; private set; }

    /// <summary>Navigation property to the template.</summary>
    public ScheduleTemplate? ScheduleTemplate { get; private set; }

    private AppointmentSlot() { } // EF Core

    public AppointmentSlot(
        Guid scheduleTemplateId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        SlotType slotType,
        decimal? cost = null)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");

        ScheduleTemplateId = scheduleTemplateId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        SlotType = slotType;
        Cost = cost;
        Status = ScheduleStatus.Available;
        BookedCount = 0;
    }

    public void Book()
    {
        if (Status != ScheduleStatus.Available)
            throw new InvalidOperationException("Slot is not available for booking");

        BookedCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelBooking()
    {
        if (BookedCount <= 0)
            throw new InvalidOperationException("No bookings to cancel");

        BookedCount--;
        if (BookedCount == 0 && Status == ScheduleStatus.Booked)
        {
            Status = ScheduleStatus.Available;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ScheduleStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != ScheduleStatus.Booked)
            throw new InvalidOperationException("Only booked slots can be completed");

        Status = ScheduleStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkNoShow()
    {
        if (Status != ScheduleStatus.Booked)
            throw new InvalidOperationException("Only booked slots can be marked as no-show");

        Status = ScheduleStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCost(decimal? cost)
    {
        Cost = cost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = ScheduleStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
