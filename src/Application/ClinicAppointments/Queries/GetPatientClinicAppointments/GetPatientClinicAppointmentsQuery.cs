using Application.ClinicAppointments.Common;
using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.ClinicAppointments.Queries.GetPatientClinicAppointments;

/// <summary>
/// Query to get clinic appointments for a patient.
/// </summary>
public record GetPatientClinicAppointmentsQuery : IQuery<IReadOnlyList<ClinicAppointmentListDto>>
{
    /// <summary>Patient to get appointments for.</summary>
    public Guid PatientId { get; init; }

    /// <summary>Filter by status (optional).</summary>
    public AppointmentStatus? Status { get; init; }

    /// <summary>Whether to only include upcoming appointments.</summary>
    public bool UpcomingOnly { get; init; }
}
