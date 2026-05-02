using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Commands.CreateAdHocSlotAndRebook;

public record CreateAdHocSlotAndRebookCommand(
    Guid AppointmentId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int MaxCapacity,
    decimal? Cost,
    Guid? DoctorId) : ICommand<CreateAdHocSlotAndRebookResult>;

public record CreateAdHocSlotAndRebookResult
{
    public Guid NewAppointmentId { get; init; }
    public Guid NewSlotId { get; init; }
}
