using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;

public record CreateAppointmentSlotCommand : ICommand<Guid>
{
    public Guid ScheduleTemplateId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public SlotType SlotType { get; init; }
    public decimal? Cost { get; init; }
}
