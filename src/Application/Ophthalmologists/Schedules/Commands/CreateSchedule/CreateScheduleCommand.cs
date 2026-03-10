using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Ophthalmologists.Schedules.Commands.CreateSchedule;

/// <summary>
/// Command to create a new schedule (booking) against an Availability slot.
/// </summary>
public record CreateScheduleCommand : ICommand<Guid>
{
    /// <summary>Availability slot the patient is booking into.</summary>
    public Guid AvailabilityId { get; init; }

    /// <summary>Patient making the booking.</summary>
    public Guid PatientId { get; init; }

    /// <summary>Date of the booking.</summary>
    public DateOnly Date { get; init; }

    /// <summary>Start time of the slot.</summary>
    public TimeOnly StartTime { get; init; }

    /// <summary>End time of the slot.</summary>
    public TimeOnly EndTime { get; init; }

    /// <summary>Type of appointment slot.</summary>
    public SlotType SlotType { get; init; }

    /// <summary>Cost of the appointment (optional).</summary>
    public decimal? Cost { get; init; }
}
