using Domain.Common;

namespace Domain.Entities.Scheduling;

/// <summary>
/// Availability - a working slot opened by a clinic or doctor before any booking.
/// Bệnh nhân đặt lịch vào một khung giờ rảnh; từ đó truy vết được bác sĩ / phòng khám.
/// </summary>
public class Availability : BaseEntity, IAggregateRoot
{
    /// <summary>FK to Organisation (nullable - can be a solo doctor slot).</summary>
    public Guid? OrganisationId { get; private set; }

    /// <summary>FK to Ophthalmologist (nullable - can be a clinic-only slot).</summary>
    public Guid? OphthalmologistId { get; private set; }

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    /// <summary>Max concurrent patients in this time window.</summary>
    public int MaxCapacity { get; private set; }

    // Navigation
    private readonly List<Schedule> _schedules = new();
    public IReadOnlyCollection<Schedule> Schedules => _schedules.AsReadOnly();

    private Availability() { } // EF Core

    public Availability(DateTime startTime, DateTime endTime, int maxCapacity,
        Guid? organisationId = null, Guid? ophthalmologistId = null)
    {
        if (endTime <= startTime)
            throw new ArgumentException("EndTime must be after StartTime");
        if (maxCapacity < 1)
            throw new ArgumentException("MaxCapacity must be at least 1", nameof(maxCapacity));
        if (organisationId is null && ophthalmologistId is null)
            throw new ArgumentException("At least one of OrganisationId or OphthalmologistId must be provided");

        StartTime = startTime;
        EndTime = endTime;
        MaxCapacity = maxCapacity;
        OrganisationId = organisationId;
        OphthalmologistId = ophthalmologistId;
    }

    public void Update(DateTime startTime, DateTime endTime, int maxCapacity)
    {
        if (endTime <= startTime)
            throw new ArgumentException("EndTime must be after StartTime");
        if (maxCapacity < 1)
            throw new ArgumentException("MaxCapacity must be at least 1", nameof(maxCapacity));

        StartTime = startTime;
        EndTime = endTime;
        MaxCapacity = maxCapacity;
        UpdatedAt = DateTime.UtcNow;
    }
}
