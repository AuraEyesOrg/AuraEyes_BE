using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Scheduling.Appointments.Commands.RequestAppointmentCancellation;

public record RequestAppointmentCancellationCommand(
    Guid AppointmentId,
    string? BankNumber,
    string? AccountName,
    string? BankName,
    string? Reason) : ICommand;
