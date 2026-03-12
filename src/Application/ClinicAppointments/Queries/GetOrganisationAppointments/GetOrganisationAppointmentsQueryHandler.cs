using Application.ClinicAppointments.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.ClinicAppointments.Queries.GetOrganisationAppointments;

/// <summary>
/// Handler for GetOrganisationAppointmentsQuery.
/// </summary>
public class GetOrganisationAppointmentsQueryHandler : IQueryHandler<GetOrganisationAppointmentsQuery, PagedResult<ClinicAppointmentListDto>>
{
    private readonly IClinicAppointmentRepository _clinicAppointmentRepository;

    public GetOrganisationAppointmentsQueryHandler(IClinicAppointmentRepository clinicAppointmentRepository)
    {
        _clinicAppointmentRepository = clinicAppointmentRepository;
    }

    public async Task<Result<PagedResult<ClinicAppointmentListDto>>> Handle(
        GetOrganisationAppointmentsQuery request, 
        CancellationToken cancellationToken)
    {
        // Determine date range
        DateOnly? fromDate = request.Date ?? request.FromDate;
        DateOnly? toDate = request.Date ?? request.ToDate;

        var (items, totalCount) = await _clinicAppointmentRepository.GetPagedAsync(
            organisationId: request.OrganisationId,
            status: request.Status,
            fromDate: fromDate,
            toDate: toDate,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var dtos = items.Select(a => new ClinicAppointmentListDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            PatientName = "Patient",  // Name is in ApplicationUser, not directly accessible
            PatientEmail = "",
            OrganisationId = a.OrganisationId,
            OrganisationName = a.Organisation?.Name ?? "Unknown",
            Date = a.AppointmentSlot?.Date ?? default,
            StartTime = a.AppointmentSlot?.StartTime ?? default,
            EndTime = a.AppointmentSlot?.EndTime ?? default,
            Status = a.Status.ToString(),
            VisitReason = a.VisitReason,
            AssignedDoctorId = a.AssignedDoctorId,
            AssignedDoctorName = null,  // Doctor name is in ApplicationUser
            CreatedAt = a.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<ClinicAppointmentListDto>(
            dtos, 
            totalCount, 
            request.PageNumber, 
            request.PageSize);

        return Result<PagedResult<ClinicAppointmentListDto>>.Success(pagedResult);
    }
}
