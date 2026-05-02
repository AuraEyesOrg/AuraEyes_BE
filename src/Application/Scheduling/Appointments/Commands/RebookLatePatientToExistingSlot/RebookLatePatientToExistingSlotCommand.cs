using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Commands.RebookLatePatientToExistingSlot;

public record RebookLatePatientToExistingSlotCommand(
    Guid AppointmentId,
    Guid NewSlotId) : ICommand<RebookLatePatientResult>;

public record RebookLatePatientResult
{
    public Guid NewAppointmentId { get; init; }
    public Guid NewSlotId { get; init; }
}
