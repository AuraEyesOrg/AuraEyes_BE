using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.CreateAppointmentSlot;

public record CreateAppointmentSlotCommand : ICommand<Guid>
{
    public Guid ScheduleTemplateId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
}
