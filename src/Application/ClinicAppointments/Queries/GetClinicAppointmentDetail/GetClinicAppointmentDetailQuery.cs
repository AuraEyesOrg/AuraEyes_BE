using Application.ClinicAppointments.Common;
using Application.Common.Interfaces;

namespace Application.ClinicAppointments.Queries.GetClinicAppointmentDetail;

/// <summary>
/// Query to get detailed information about a clinic appointment.
/// </summary>
public record GetClinicAppointmentDetailQuery : IQuery<ClinicAppointmentDetailDto>
{
    /// <summary>Appointment ID to get details for.</summary>
    public Guid AppointmentId { get; init; }
}
