using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Scheduling.Appointments.Queries.GetClinicAppointmentsByDate;

public class GetClinicAppointmentsByDateQueryHandler
    : IQueryHandler<GetClinicAppointmentsByDateQuery, IReadOnlyList<ClinicAppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public GetClinicAppointmentsByDateQueryHandler(
        IAppointmentRepository appointmentRepository,
        IOrderRepository orderRepository,
        IPatientVisitRepository patientVisitRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _appointmentRepository = appointmentRepository;
        _orderRepository = orderRepository;
        _patientVisitRepository = patientVisitRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
    }

    public async Task<Result<IReadOnlyList<ClinicAppointmentDto>>> Handle(
        GetClinicAppointmentsByDateQuery request,
        CancellationToken cancellationToken)
    {
        var (appointments, _) = await _appointmentRepository.GetPagedAsync(
            fromDate: request.Date,
            toDate: request.Date,
            pageNumber: 1,
            pageSize: 500,
            cancellationToken: cancellationToken);

        var appointmentIds = appointments.Select(a => a.Id).ToList();
        var orders = await _orderRepository.GetByAppointmentIdsAsync(appointmentIds, cancellationToken);
        var orderLookup = orders.ToLookup(o => o.AppointmentId!.Value);
        var visits = await _patientVisitRepository.Query()
            .Where(v => v.AppointmentId.HasValue && appointmentIds.Contains(v.AppointmentId.Value))
            .ToListAsync(cancellationToken);
        var visitMap = visits.ToDictionary(v => v.AppointmentId!.Value);

        // Fetch registered user names for patient profiles
        var registeredUserIds = appointments
            .Where(a => a.Patient?.UserId != null)
            .Select(a => a.Patient!.UserId!.Value)
            .Distinct()
            .ToList();

        var userMap = (await _identityService.GetUsersByIdsAsync(registeredUserIds, cancellationToken))
            .ToDictionary(u => u.Id);

        // Fetch doctor names/avatars in batch
        var doctorIds = appointments
            .Select(a => a.RequestedDoctorId ?? a.AppointmentSlot?.OphthalId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Union(visits.Where(v => v.AssignedDoctorId.HasValue).Select(v => v.AssignedDoctorId!.Value))
            .Distinct()
            .ToList();

        var doctorMap = await _ophthalmologistRepository.GetDoctorDetailsByIdsAsync(doctorIds, cancellationToken);

        var items = appointments
            .Where(a => a.AppointmentSlot is not null && a.Status != AppointmentStatus.Cancelled)
            .Select(a => {
                var appointmentOrders = orderLookup[a.Id].ToList();
                var primaryOrder = appointmentOrders.OrderByDescending(o => o.CreatedAt).FirstOrDefault();
                
                decimal totalAmount = appointmentOrders.Sum(o => o.TotalAmount);
                decimal paidAmount = appointmentOrders.SelectMany(o => o.Payments)
                    .Where(p => p.Status == PaymentStatus.Completed)
                    .Sum(p => p.Amount);
                decimal remaining = totalAmount - paidAmount;
                
                bool isPaidDeposit = appointmentOrders.Any(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Completed);

                visitMap.TryGetValue(a.Id, out var visit);
                
                var finalDocId = visit?.AssignedDoctorId ?? a.RequestedDoctorId ?? a.AppointmentSlot?.OphthalId;
                doctorMap.TryGetValue(finalDocId ?? Guid.Empty, out var doc);

                // Resolve patient display name
                string patientName = "Patient";
                if (a.Patient != null)
                {
                    if (a.Patient.IsWalkIn)
                    {
                        patientName = a.Patient.FullName ?? "Patient";
                    }
                    else if (a.Patient.UserId.HasValue && userMap.TryGetValue(a.Patient.UserId.Value, out var user))
                    {
                        patientName = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : (user.Email ?? "Patient");
                    }
                }
                
                return new ClinicAppointmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = patientName,
                    PatientAvatarUrl = null,
                    SlotId = a.AppointmentSlotId,
                    Date = a.AppointmentSlot!.Date,
                    StartTime = a.AppointmentSlot.StartTime,
                    EndTime = a.AppointmentSlot.EndTime,
                    VisitReason = a.VisitReason,
                    Status = visit?.Status.ToString() ?? a.Status.ToString(),
                    CreatedAt = a.CreatedAt,
                    HasFeedback = false,
                    
                    // Doctor info
                    OphthalId = a.AppointmentSlot.OphthalId,
                    OphthalFullName = doc.FullName ?? "Clinic Doctor",
                    OphthalAvatarUrl = doc.AvatarUrl,

                    // Billing
                    OrderId = primaryOrder?.Id,
                    TotalAmount = totalAmount > 0 ? totalAmount : null,
                    DepositAmount = primaryOrder?.DepositAmount,
                    IsPaidDeposit = isPaidDeposit,
                    PaidAmount = paidAmount,
                    RemainingAmount = totalAmount > 0 ? (remaining > 0 ? remaining : 0) : null,
                    OrderStatus = primaryOrder?.Status switch
                    {
                        Domain.Enums.OrderStatus.Confirmed => "PartiallyPaid",
                        Domain.Enums.OrderStatus.Completed => "FullyPaid",
                        _ => primaryOrder?.Status.ToString()
                    }
                };
            })
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .ToList();

        return Result<IReadOnlyList<ClinicAppointmentDto>>.Success(items);
    }
}
