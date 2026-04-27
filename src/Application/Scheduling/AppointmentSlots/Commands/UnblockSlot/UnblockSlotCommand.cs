using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.UnblockSlot;

public record UnblockSlotCommand : ICommand
{
    public Guid AppointmentSlotId { get; init; }
}
