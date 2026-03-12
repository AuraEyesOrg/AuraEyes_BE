using Application.ClinicAppointments.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;

namespace Application.ClinicAppointments.Queries.GetOrganisationAppointments;

/// <summary>
/// Query to get appointments for an organisation (for staff management).
/// </summary>
public record GetOrganisationAppointmentsQuery : IQuery<PagedResult<ClinicAppointmentListDto>>
{
    /// <summary>Organisation to get appointments for.</summary>
    public Guid OrganisationId { get; init; }

    /// <summary>Filter by date (optional).</summary>
    public DateOnly? Date { get; init; }

    /// <summary>Filter from date (used if Date is null).</summary>
    public DateOnly? FromDate { get; init; }

    /// <summary>Filter to date (used if Date is null).</summary>
    public DateOnly? ToDate { get; init; }

    /// <summary>Filter by status (optional).</summary>
    public AppointmentStatus? Status { get; init; }

    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
