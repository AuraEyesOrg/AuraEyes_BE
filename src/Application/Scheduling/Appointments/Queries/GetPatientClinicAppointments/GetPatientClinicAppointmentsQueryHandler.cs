using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Domain.Enums;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Queries.GetPatientClinicAppointments;

public class GetPatientClinicAppointmentsQueryHandler
    : IQueryHandler<GetPatientClinicAppointmentsQuery, PagedResult<ClinicAppointmentDto>>
{
    private const int MaxPageSize = 50;

    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClinicFeedbackRepository _clinicFeedbackRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IClinicStaffRepository _staffRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetPatientClinicAppointmentsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IClinicFeedbackRepository clinicFeedbackRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IOrderRepository orderRepository,
        IClinicStaffRepository staffRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _appointmentRepository = appointmentRepository;
        _clinicFeedbackRepository = clinicFeedbackRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _orderRepository = orderRepository;
        _staffRepository = staffRepository;
        _unitOfWork = unitOfWork;
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
            : await _clinicFeedbackRepository.GetAppointmentIdsWithFeedbackAsync(
                request.PatientId,
                appointmentIds,
                cancellationToken);

        // Fetch doctor names/avatars in batch
        var doctorIds = appointments
            .Where(a => a.AppointmentSlot?.OphthalId != null)
            .Select(a => a.AppointmentSlot!.OphthalId!.Value)
            .Distinct()
            .ToList();

        var doctorMap = await _ophthalmologistRepository.GetEnhancedDoctorDetailsByIdsAsync(doctorIds, cancellationToken);

        // Fetch associated orders to populate OrderId/Billing info
        var orders = await _orderRepository.GetByAppointmentIdsAsync(appointmentIds, cancellationToken);
        var orderMap = orders.GroupBy(o => o.AppointmentId)
            .ToDictionary(g => g.Key!.Value, g => g.OrderByDescending(o => o.CreatedAt).First());

        // In a real scenario, we'd link a staff member to the appointment lifecycle (confirmed by, etc.)
        // For now, we don't have a direct StaffId in Appointment entity.

        var items = appointments
            .Where(a => a.AppointmentSlot is not null)
            .Select(a =>
            {
                doctorMap.TryGetValue(a.AppointmentSlot!.OphthalId ?? Guid.Empty, out var doc);

                return new ClinicAppointmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    SlotId = a.AppointmentSlotId,
                    Date = a.AppointmentSlot!.Date,
                    StartTime = a.AppointmentSlot.StartTime,
                    EndTime = a.AppointmentSlot.EndTime,
                    VisitReason = a.VisitReason,
                    Status = a.Status.ToString(),
                    CreatedAt = a.CreatedAt,
                    HasFeedback = feedbackAppointmentIds.Contains(a.Id),
                    OrganisationId = null,
                    OrganisationName = "Aura Clinic",
                    OphthalId = a.AppointmentSlot.OphthalId,
                    OphthalFullName = doc.FullName ?? "Clinic Doctor",
                    OphthalAvatarUrl = doc.AvatarUrl,
                    StaffId = null, // No specific staff linked yet
                    StaffName = null,
                    
                    // Billing info
                    OrderId = orderMap.TryGetValue(a.Id, out var ord) ? ord.Id : null,
                    OrderStatus = ord?.Status switch
                    {
                        OrderStatus.Confirmed => "PartiallyPaid",
                        OrderStatus.Completed => "FullyPaid",
                        _ => ord?.Status.ToString()
                    },
                    TotalAmount = ord?.TotalAmount,
                    DepositAmount = ord?.DepositAmount,
                    IsPaidDeposit = ord?.Status == OrderStatus.Confirmed || ord?.Status == OrderStatus.Completed
                };
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
