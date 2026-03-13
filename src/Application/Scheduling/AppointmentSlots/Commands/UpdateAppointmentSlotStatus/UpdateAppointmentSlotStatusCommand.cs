using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Scheduling.AppointmentSlots.Commands.UpdateAppointmentSlotStatus;

/// <summary>
/// Command to update the status of an appointment slot.
/// </summary>
public record UpdateAppointmentSlotStatusCommand : ICommand<bool>
{
    /// <summary>
    /// Appointment slot ID to update.
    /// </summary>
    public Guid AppointmentSlotId { get; init; }

    /// <summary>
    /// New status for the appointment slot.
    /// </summary>
    public ScheduleStatus NewStatus { get; init; }
}
