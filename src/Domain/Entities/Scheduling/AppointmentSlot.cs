using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// AppointmentSlot - A specific time slot for a specific date, generated from a ScheduleTemplate.
/// Tracks bookings, reservations, and availability.
/// 
/// Slot ownership is determined by the parent ScheduleTemplate:
/// - If template has OphthalId → Online consultation slot (capacity = 1)
/// - If template has OrgId only → Clinic visit slot (capacity >= 1)
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

    /// <summary>Maximum number of patients that can book this slot (copied from template).</summary>
    public int MaxCapacity { get; private set; }

    /// <summary>Number of patients currently booked in this slot.</summary>
    public int BookedCount { get; private set; }

    /// <summary>Patient who has reserved this slot (pending payment) - for online consultations.</summary>
    public Guid? ReservedBy { get; private set; }

    /// <summary>When the reservation expires (auto-release after this time).</summary>
    public DateTime? ReservationExpireAt { get; private set; }

    /// <summary>Navigation property to the template.</summary>
    public ScheduleTemplate? ScheduleTemplate { get; private set; }

    // Navigation to appointments
    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    private AppointmentSlot() { } // EF Core

    public AppointmentSlot(
        Guid scheduleTemplateId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        int maxCapacity = 1,
        decimal? cost = null)
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
        Cost = cost;
        Status = ScheduleStatus.Available;
        BookedCount = 0;
    }

    /// <summary>
    /// Reserve the slot for a patient (pending payment).
    /// </summary>
    public void Reserve(Guid patientId, DateTime expirationTime)
    {
        if (Status != ScheduleStatus.Available)
            throw new InvalidOperationException($"Slot is not available for reservation. Current status: {Status}");

        if (expirationTime <= DateTime.UtcNow)
            throw new ArgumentException("Expiration time must be in the future", nameof(expirationTime));

        Status = ScheduleStatus.Reserved;
        ReservedBy = patientId;
        ReservationExpireAt = expirationTime;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Confirm the reservation after payment - transitions to Booked.
    /// </summary>
    public void ConfirmReservation(Guid patientId)
    {
        if (Status != ScheduleStatus.Reserved)
            throw new InvalidOperationException($"Slot is not in reserved state. Current status: {Status}");

        if (ReservedBy != patientId)
            throw new InvalidOperationException("Only the patient who reserved this slot can confirm it");

        if (ReservationExpireAt.HasValue && ReservationExpireAt.Value < DateTime.UtcNow)
            throw new InvalidOperationException("Reservation has expired");

        Status = ScheduleStatus.Booked;
        BookedCount = 1;
        ReservedBy = null;
        ReservationExpireAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Release the reservation (manual or timeout) - returns to Available.
    /// </summary>
    public void ReleaseReservation()
    {
        if (Status != ScheduleStatus.Reserved)
            throw new InvalidOperationException($"Slot is not in reserved state. Current status: {Status}");

        Status = ScheduleStatus.Available;
        ReservedBy = null;
        ReservationExpireAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Check if the reservation has expired.
    /// </summary>
    public bool IsReservationExpired()
    {
        return Status == ScheduleStatus.Reserved &&
               ReservationExpireAt.HasValue &&
               ReservationExpireAt.Value < DateTime.UtcNow;
    }

    /// <summary>
    /// Block the slot (doctor unavailable).
    /// </summary>
    public void Block()
    {
        if (Status == ScheduleStatus.Booked)
            throw new InvalidOperationException("Cannot block a slot that is already booked. Cancel the booking first.");

        if (Status == ScheduleStatus.Reserved)
            throw new InvalidOperationException("Cannot block a slot that is reserved. Wait for reservation to expire or release it first.");

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
    /// Book for capacity-based slots (organisation clinic appointments).
    /// Increments booked count if capacity available.
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
    /// Legacy book method for single-capacity slots (online consultation).
    /// </summary>
    public void Book()
    {
        if (Status != ScheduleStatus.Available)
            throw new InvalidOperationException("Slot is not available for booking");

        BookedCount++;
        if (MaxCapacity == 1)
        {
            Status = ScheduleStatus.Booked;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancel a booking and decrement the booked count.
    /// For capacity-based slots, slot remains available if count > 0.
    /// </summary>
    public void CancelBooking()
    {
        if (BookedCount <= 0)
            throw new InvalidOperationException("No bookings to cancel");

        BookedCount--;
        
        // For single-capacity slots (online consultation)
        if (MaxCapacity == 1 && BookedCount == 0 && Status == ScheduleStatus.Booked)
        {
            Status = ScheduleStatus.Available;
        }
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
        ReservedBy = null;
        ReservationExpireAt = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
