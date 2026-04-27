using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.BlockSlot;

public record BlockSlotCommand : ICommand
{
    public Guid AppointmentSlotId { get; init; }
    public string? Reason { get; init; }
}
