using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Scheduling.Appointments.Commands.ConfirmAppointmentCancellation;

public record ConfirmAppointmentCancellationCommand(Guid AppointmentId) : ICommand;
