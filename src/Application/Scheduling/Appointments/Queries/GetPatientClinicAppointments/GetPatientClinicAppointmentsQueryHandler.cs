using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Queries.GetPatientClinicAppointments;

public class GetPatientClinicAppointmentsQueryHandler
    : IQueryHandler<GetPatientClinicAppointmentsQuery, PagedResult<ClinicAppointmentDto>>
{
    private const int MaxPageSize = 50;

    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IOrganisationFeedbackRepository _organisationFeedbackRepository;
    private readonly ICurrentUserService _currentUser;

    public GetPatientClinicAppointmentsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IOrganisationFeedbackRepository organisationFeedbackRepository,
        ICurrentUserService currentUser)
    {
        _appointmentRepository = appointmentRepository;
        _organisationFeedbackRepository = organisationFeedbackRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<ClinicAppointmentDto>>> Handle(
        GetPatientClinicAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId is null)
        {
            return Result<PagedResult<ClinicAppointmentDto>>.Unauthorized(
                "Patient profile is required.");
        }

        if (_currentUser.ProfileId.Value != request.PatientId)
        {
            return Result<PagedResult<ClinicAppointmentDto>>.Forbidden(
                "You can only view your own clinic appointments.");
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize switch
        {
            <= 0 => 10,
            > MaxPageSize => MaxPageSize,
            _ => request.PageSize
        };

        var statuses = MapTabToStatuses(request.Tab);

        var (appointments, totalCount) = await _appointmentRepository.GetPagedByPatientAsync(
            request.PatientId,
            statuses,
            pageNumber,
            pageSize,
            request.Tab == PatientAppointmentTab.Upcoming,
            cancellationToken);

        var appointmentIds = appointments
            .Where(a => a.AppointmentSlot is not null)
            .Select(a => a.Id)
            .ToArray();

        // Batch feedback presence check in a single query (avoids N+1 round-trips).
        IReadOnlySet<Guid> feedbackAppointmentIds = appointmentIds.Length == 0
            ? new HashSet<Guid>()
            : await _organisationFeedbackRepository.GetAppointmentIdsWithFeedbackAsync(
                request.PatientId,
                appointmentIds,
                cancellationToken);

        var items = appointments
            .Where(a => a.AppointmentSlot is not null)
            .Select(a => new ClinicAppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                SlotId = a.AppointmentSlotId,
                Date = a.AppointmentSlot!.Date,
                StartTime = a.AppointmentSlot.StartTime,
                EndTime = a.AppointmentSlot.EndTime,
                VisitReason = a.VisitReason,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                HasFeedback = feedbackAppointmentIds.Contains(a.Id)
            })
            .ToList();

        var paged = new PagedResult<ClinicAppointmentDto>(items, totalCount, pageNumber, pageSize);
        return Result<PagedResult<ClinicAppointmentDto>>.Success(paged);
    }

    private static IReadOnlyCollection<AppointmentStatus>? MapTabToStatuses(PatientAppointmentTab tab) =>
        tab switch
        {
            PatientAppointmentTab.Upcoming => new[]
            {
                AppointmentStatus.Pending,
                AppointmentStatus.Confirmed,
                AppointmentStatus.CheckedIn,
                AppointmentStatus.InProgress
            },
            PatientAppointmentTab.Completed => new[]
            {
                AppointmentStatus.Completed
            },
            PatientAppointmentTab.Cancelled => new[]
            {
                AppointmentStatus.Cancelled,
                AppointmentStatus.NoShow
            },
            _ => null
        };
}
