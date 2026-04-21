using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;

namespace Application.Scheduling.Appointments.Queries.GetPatientClinicAppointments;

/// <summary>
/// Logical tab filter for the patient "My Appointments" page.
/// Keeping the grouping on the server ensures a single, cache-friendly
/// contract between BE and FE.
/// </summary>
public enum PatientAppointmentTab
{
    All = 0,
    Upcoming = 1,
    Completed = 2,
    Cancelled = 3
}

public record GetPatientClinicAppointmentsQuery(
    Guid PatientId,
    PatientAppointmentTab Tab = PatientAppointmentTab.All,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<PagedResult<ClinicAppointmentDto>>;
