using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Schedules.Commands.BookSchedule;

/// <summary>
/// Command to book (confirm) an available schedule slot for a patient.
/// Validates AvailableSlot capacity before transitioning status to Booked.
/// </summary>
public record BookScheduleCommand : ICommand
{
    /// <summary>The schedule to book.</summary>
    public Guid ScheduleId { get; init; }

    /// <summary>Patient confirming the booking (used for authorization check).</summary>
    public Guid PatientId { get; init; }
}
