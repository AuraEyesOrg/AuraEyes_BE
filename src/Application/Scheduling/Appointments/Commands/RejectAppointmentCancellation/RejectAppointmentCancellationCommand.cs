using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Scheduling.Appointments.Commands.RejectAppointmentCancellation;

public record RejectAppointmentCancellationCommand(Guid AppointmentId, string? AdminNote = null) : ICommand;
