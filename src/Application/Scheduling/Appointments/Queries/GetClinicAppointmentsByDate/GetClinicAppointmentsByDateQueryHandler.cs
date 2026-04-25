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

    public GetClinicAppointmentsByDateQueryHandler(
        IAppointmentRepository appointmentRepository,
        IOrderRepository orderRepository,
        IPatientVisitRepository patientVisitRepository)
    {
        _appointmentRepository = appointmentRepository;
        _orderRepository = orderRepository;
        _patientVisitRepository = patientVisitRepository;
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
        var orderMap = orders.ToDictionary(o => o.AppointmentId!.Value);
        var visits = await _patientVisitRepository.Query()
            .Where(v => v.AppointmentId.HasValue && appointmentIds.Contains(v.AppointmentId.Value))
            .ToListAsync(cancellationToken);
        var visitMap = visits.ToDictionary(v => v.AppointmentId!.Value);

        var items = appointments
            .Where(a => a.AppointmentSlot is not null)
            .Select(a => {
                orderMap.TryGetValue(a.Id, out var order);
                visitMap.TryGetValue(a.Id, out var visit);
                
                return new ClinicAppointmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient?.FullName,
                    PatientAvatarUrl = null,
                    SlotId = a.AppointmentSlotId,
                    Date = a.AppointmentSlot!.Date,
                    StartTime = a.AppointmentSlot.StartTime,
                    EndTime = a.AppointmentSlot.EndTime,
                    VisitReason = a.VisitReason,
                    Status = visit?.Status.ToString() ?? a.Status.ToString(),
                    CreatedAt = a.CreatedAt,
                    HasFeedback = false,
                    
                    // Billing
                    OrderId = order?.Id,
                    TotalAmount = order?.TotalAmount,
                    DepositAmount = order?.DepositAmount,
                    IsPaidDeposit = order?.Payments.Any(p => p.Status == PaymentStatus.Completed) ?? false,
                    RemainingAmount = order != null ? order.TotalAmount - order.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount) : null,
                    OrderStatus = order?.Status
                };
            })
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .ToList();

        return Result<IReadOnlyList<ClinicAppointmentDto>>.Success(items);
    }
}
