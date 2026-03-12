using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlot;

/// <summary>
/// Command to update an existing appointment slot.
/// </summary>
public record UpdateAppointmentSlotCommand : ICommand
{
    public Guid AppointmentSlotId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public decimal? Cost { get; init; }
}
