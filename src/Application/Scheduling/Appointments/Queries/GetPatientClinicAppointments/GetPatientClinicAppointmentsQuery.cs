using Application.Common.Interfaces;
using Application.Scheduling.Appointments.Common;

namespace Application.Scheduling.Appointments.Queries.GetPatientClinicAppointments;

public record GetPatientClinicAppointmentsQuery(Guid PatientId) : IQuery<IReadOnlyList<ClinicAppointmentDto>>;
