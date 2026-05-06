using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Commands.CheckInClinicAppointment;

public record CheckInClinicAppointmentCommand(Guid AppointmentId, string? PatientName = null) : ICommand;
