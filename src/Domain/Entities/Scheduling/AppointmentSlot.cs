using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// AppointmentSlot - A specific time slot for a specific date, generated from a ScheduleTemplate.
/// Tracks capacity and availability. Simplified from reservation-based to capacity-based model.
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

    /// <summary>Status of this slot (Available or Blocked).</summary>
    public ScheduleStatus Status { get; private set; }

    /// <summary>FK to the primary Ophthalmologist assigned to this slot (if any).</summary>
    public Guid? OphthalId { get; private set; }

    /// <summary>The cost/fee for this specific slot.</summary>
    public decimal? Cost { get; private set; }

    /// <summary>Optional timestamp for when a temporary reservation expires.</summary>
    public DateTime? ReservationExpireAt { get; private set; }

    /// <summary>Slot creation source (staff or system).</summary>
    public int MaxCapacity { get; private set; }

    /// <summary>Number of patients currently booked in this slot.</summary>
    public int BookedCount { get; private set; }

    /// <summary>Slot creation source (staff or system).</summary>
    public SlotSource Source { get; private set; }

    /// <summary>Navigation property to the template.</summary>
    public ScheduleTemplate? ScheduleTemplate { get; private set; }

    // Navigation to appointments
    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    // Navigation to slot assignments
    private readonly List<SlotAssignment> _slotAssignments = new();
    public IReadOnlyCollection<SlotAssignment> SlotAssignments => _slotAssignments.AsReadOnly();

    private AppointmentSlot() { } // EF Core

    public AppointmentSlot(
        Guid scheduleTemplateId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        int maxCapacity = 1,
        SlotSource source = SlotSource.Doctor)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");
        if (maxCapacity < 1)
            throw new ArgumentException("Max capacity must be at least 1", nameof(maxCapacity));

        ScheduleTemplateId = scheduleTemplateId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        MaxCapacity = maxCapacity;
        Source = source;
        Status = ScheduleStatus.Available;
        BookedCount = 0;
    }

    /// <summary>
    /// Check if the slot has available capacity for more bookings.
    /// </summary>
    public bool HasCapacity()
    {
        return Status == ScheduleStatus.Available && BookedCount < MaxCapacity;
    }

    /// <summary>
    /// Get remaining capacity.
    /// </summary>
    public int RemainingCapacity => Math.Max(0, MaxCapacity - BookedCount);

    /// <summary>
    /// Book a patient into this slot. Increments booked count if capacity available.
    /// </summary>
    public void BookWithCapacity()
    {
        if (Status == ScheduleStatus.Blocked)
            throw new InvalidOperationException("Slot is blocked and not available for booking");

        if (Status != ScheduleStatus.Available)
            throw new InvalidOperationException($"Slot is not available for booking. Current status: {Status}");

        if (BookedCount >= MaxCapacity)
            throw new InvalidOperationException("Slot has reached maximum capacity");

        BookedCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancel a booking and decrement the booked count.
    /// </summary>
    public void CancelBooking()
    {
        if (BookedCount <= 0)
            throw new InvalidOperationException("No bookings to cancel");

        BookedCount--;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Block()
    {
        if (BookedCount > 0)
            throw new InvalidOperationException("Cannot block a slot that has bookings. Cancel the bookings first.");

        Status = ScheduleStatus.Blocked;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Force block the slot regardless of bookings (used for template deletion).
    /// </summary>
    public void ForceBlock()
    {
        Status = ScheduleStatus.Blocked;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Unblock the slot (make available again).
    /// </summary>
    public void Unblock()
    {
        if (Status != ScheduleStatus.Blocked)
            throw new InvalidOperationException($"Slot is not blocked. Current status: {Status}");

        Status = ScheduleStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Update the maximum capacity of this slot.
    /// Can only increase capacity (not below current booked count).
    /// </summary>
    public void UpdateCapacity(int newCapacity)
    {
        if (newCapacity < 1)
            throw new ArgumentException("Capacity must be at least 1", nameof(newCapacity));

        if (newCapacity < BookedCount)
            throw new InvalidOperationException($"Cannot reduce capacity below current booked count ({BookedCount})");

        MaxCapacity = newCapacity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOphthalId(Guid? ophthalId)
    {
        OphthalId = ophthalId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCost(decimal? cost)
    {
        if (cost.HasValue && cost < 0)
            throw new ArgumentException("Cost cannot be negative", nameof(cost));

        Cost = cost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateReservationExpireAt(DateTime? reservationExpireAt)
    {
        ReservationExpireAt = reservationExpireAt;
        UpdatedAt = DateTime.UtcNow;
    }
}
