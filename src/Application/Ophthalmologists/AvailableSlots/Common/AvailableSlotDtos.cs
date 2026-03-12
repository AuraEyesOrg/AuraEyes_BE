namespace Application.Ophthalmologists.AvailableSlots.Common;

/// <summary>
/// DTO for available slot list items.
/// </summary>
public record AvailableSlotListDto
{
    public Guid Id { get; init; }
    public Guid? OrganisationId { get; init; }
    public Guid? OphthalmologistId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int MaxCapacity { get; init; }
    public int BookedCount { get; init; }
    public int AvailableCapacity => MaxCapacity - BookedCount;
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO for detailed available slot view.
/// </summary>
public record AvailableSlotDto
{
    public Guid Id { get; init; }
    public Guid? OrganisationId { get; init; }
    public Guid? OphthalmologistId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int MaxCapacity { get; init; }
    public int BookedCount { get; init; }
    public int AvailableCapacity => MaxCapacity - BookedCount;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public IReadOnlyList<AvailableSlotScheduleDto> Schedules { get; init; } = [];
}

/// <summary>
/// DTO for schedule items within an available slot.
/// </summary>
public record AvailableSlotScheduleDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string Status { get; init; } = string.Empty;
}
