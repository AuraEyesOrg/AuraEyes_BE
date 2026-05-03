using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Queries.GetPendingCancellations;

public class GetPendingCancellationsQueryHandler : IQueryHandler<GetPendingCancellationsQuery, PagedResult<ClinicAppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IIdentityService _identityService;

    public GetPendingCancellationsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IOrderRepository orderRepository,
        IIdentityService identityService)
    {
        _appointmentRepository = appointmentRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _orderRepository = orderRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<ClinicAppointmentDto>>> Handle(GetPendingCancellationsQuery request, CancellationToken cancellationToken)
    {
        var (appointments, totalCount) = await _appointmentRepository.GetPagedAsync(
            status: AppointmentStatus.CancellationRequested,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var appointmentIds = appointments.Select(a => a.Id).ToArray();
        var orders = await _orderRepository.GetByAppointmentIdsAsync(appointmentIds, cancellationToken);
        var orderMap = orders.GroupBy(o => o.AppointmentId)
            .ToDictionary(g => g.Key!.Value, g => g.OrderByDescending(o => o.CreatedAt).First());

        var doctorIds = appointments
            .Where(a => a.AppointmentSlot?.OphthalId != null)
            .Select(a => a.AppointmentSlot!.OphthalId!.Value)
            .Distinct()
            .ToList();
        var doctorMap = await _ophthalmologistRepository.GetDoctorDetailsByIdsAsync(doctorIds, cancellationToken);

        var registeredUserIds = appointments
            .Where(a => a.Patient?.UserId != null)
            .Select(a => a.Patient!.UserId!.Value)
            .Distinct()
            .ToList();
        var userMap = (await _identityService.GetUsersByIdsAsync(registeredUserIds, cancellationToken))
            .ToDictionary(u => u.Id);

        var items = appointments.Select(a =>
        {
            doctorMap.TryGetValue(a.AppointmentSlot?.OphthalId ?? Guid.Empty, out var doc);
            
            string patientName = "Patient";
            if (a.Patient != null)
            {
                if (a.Patient.IsWalkIn)
                {
                    patientName = a.Patient.FullName ?? "Patient";
                }
                else if (a.Patient.UserId.HasValue && userMap.TryGetValue(a.Patient.UserId.Value, out var user))
                {
                    patientName = user.FullName ?? user.Email ?? "Patient";
                }
            }

            return new ClinicAppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = patientName,
                SlotId = a.AppointmentSlotId,
                Date = a.AppointmentSlot?.Date ?? default,
                StartTime = a.AppointmentSlot?.StartTime ?? default,
                EndTime = a.AppointmentSlot?.EndTime ?? default,
                VisitReason = a.VisitReason,
                Status = a.Status.ToString(),
                CreatedAt = a.CreatedAt,
                OphthalId = a.AppointmentSlot?.OphthalId,
                OphthalFullName = doc.FullName ?? "Clinic Doctor",
                OphthalAvatarUrl = doc.AvatarUrl,
                OrderId = orderMap.TryGetValue(a.Id, out var ord) ? ord.Id : null,
                OrderStatus = ord?.Status.ToString(),
                TotalAmount = ord?.TotalAmount,
                DepositAmount = ord?.DepositAmount,
                IsPaidDeposit = ord?.Status == OrderStatus.Confirmed || ord?.Status == OrderStatus.Completed,
                RefundBankNumber = a.RefundBankNumber,
                RefundAccountName = a.RefundAccountName,
                RefundBankName = a.RefundBankName,
                CancellationReason = a.CancellationReason
            };
        }).ToList();

        return Result<PagedResult<ClinicAppointmentDto>>.Success(new PagedResult<ClinicAppointmentDto>(items, totalCount, request.PageNumber, request.PageSize));
    }
}
