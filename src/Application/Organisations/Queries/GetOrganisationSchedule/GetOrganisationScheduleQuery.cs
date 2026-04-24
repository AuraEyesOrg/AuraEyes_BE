using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using MediatR;

namespace Application.Organisations.Queries.GetOrganisationSchedule;

public class GetOrganisationScheduleQuery : IRequest<Result<OrganisationScheduleDto>>
{
    public Guid OrganisationId { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
}

public class OrganisationScheduleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Description { get; set; }
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }
    public List<AggregatedSlotDto> AggregatedSlots { get; set; } = new();
}

public class AggregatedSlotDto
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public List<DoctorSlotDetailDto> Doctors { get; set; } = new();

    /// <summary>
    /// Total capacity across all doctors for this time window.
    /// </summary>
    public int TotalMaxCapacity => Doctors.Count;

    /// <summary>
    /// Total number of doctors already booked for this time window.
    /// </summary>
    public int TotalBookedCount => Doctors.Count(d => d.IsBooked);

    /// <summary>
    /// Whether at least one doctor is available for booking.
    /// </summary>
    public bool IsAvailable => Doctors.Any(d => !d.IsBooked);
}

public class DoctorSlotDetailDto
{
    public Guid SlotId { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string? DoctorAvatar { get; set; }
    public bool IsBooked { get; set; }
    public decimal Price { get; set; }
}
