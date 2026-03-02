using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Ophthalmologists.Schedules.Commands.CreateSchedule;

/// <summary>
/// Command to create a new schedule (time slot) for an ophthalmologist.
/// </summary>
public record CreateScheduleCommand : ICommand<Guid>
{
    /// <summary>
    /// Ophthalmologist ID to create schedule for.
    /// </summary>
    public Guid OphthalmologistId { get; init; }

    /// <summary>
    /// Organisation ID (optional).
    /// </summary>
    public Guid? OrganisationId { get; init; }

    /// <summary>
    /// Date of the schedule.
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Start time of the slot.
    /// </summary>
    public TimeOnly StartTime { get; init; }

    /// <summary>
    /// End time of the slot.
    /// </summary>
    public TimeOnly EndTime { get; init; }

    /// <summary>
    /// Type of appointment slot.
    /// </summary>
    public SlotType SlotType { get; init; }

    /// <summary>
    /// Cost of the appointment (optional).
    /// </summary>
    public decimal? Cost { get; init; }
}
