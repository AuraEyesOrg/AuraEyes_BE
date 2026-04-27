namespace Application.Scheduling.AppointmentSlots.Common;

public record AppointmentSlotListDto
{
    public Guid Id { get; init; }
    public Guid OphthalId { get; init; }
    public Guid ScheduleTemplateId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public int MaxCapacity { get; init; }
    public int BookedCount { get; init; }
    public int AvailableCapacity { get; init; }
    public string? OphthalFullName { get; init; }
    public string? OphthalAvatarUrl { get; init; }
    public decimal? Cost { get; init; }
    public DateTime? ReservationExpireAt { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record AppointmentSlotDto
{
    public Guid Id { get; init; }
    public Guid OphthalId { get; init; }
    public Guid ScheduleTemplateId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public int MaxCapacity { get; init; }
    public int BookedCount { get; init; }
    public int AvailableCapacity { get; init; }
    public decimal? Cost { get; init; }
    public DateTime? ReservationExpireAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record AppointmentSlotStatsDto
{
    public int TotalCount { get; init; }
    public int AvailableCount { get; init; }
    public int BlockedCount { get; init; }
}

public record AllowedPriceRangeDto
{
    public Guid OphthalmologistId { get; init; }
    public int YearsOfExperience { get; init; }
    public decimal MinPrice { get; init; }
    public decimal MaxPrice { get; init; }
}

public record ClinicScheduleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Description { get; init; }
    public double RatingAverage { get; init; }
    public int RatingCount { get; init; }
    public List<AggregatedSlotDto> AggregatedSlots { get; init; } = new();
}

public record AggregatedSlotDto
{
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public List<DoctorSlotDetailDto> Doctors { get; init; } = new();
    public int TotalMaxCapacity { get; init; }
    public int TotalBookedCount { get; init; }
    public bool IsAvailable { get; init; }
}

public record DoctorSlotDetailDto
{
    public Guid SlotId { get; init; }
    public Guid DoctorId { get; init; }
    public string DoctorName { get; init; } = string.Empty;
    public string? DoctorAvatar { get; init; }
    public bool IsBooked { get; init; }
    public decimal Price { get; init; }
}
