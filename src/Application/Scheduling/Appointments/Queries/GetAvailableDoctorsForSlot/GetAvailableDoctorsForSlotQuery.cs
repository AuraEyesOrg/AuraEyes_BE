using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Queries.GetAvailableDoctorsForSlot;

public record GetAvailableDoctorsForSlotQuery(
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime) : IQuery<IReadOnlyList<AvailableDoctorDto>>;

public record AvailableDoctorDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
}
