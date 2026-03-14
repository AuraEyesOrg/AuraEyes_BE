using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.DeleteAppointmentSlot;

/// <summary>
/// Command to delete (cancel) an appointment slot.
/// </summary>
public record DeleteAppointmentSlotCommand : ICommand
{
    public Guid AppointmentSlotId { get; init; }
}
