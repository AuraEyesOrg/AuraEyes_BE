using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Financial.Common.DTOs;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Financial.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly IPayOSService _payOSService;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IClinicVisitService _clinicVisitService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository,
        ICurrentUserService currentUserService,
        IIdentityService identityService,
        IPayOSService payOSService,
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IClinicVisitService clinicVisitService,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<GetOrderByIdQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _currentUserService = currentUserService;
        _identityService = identityService;
        _payOSService = payOSService;
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _clinicVisitService = clinicVisitService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithPaymentsAsync(request.Id, cancellationToken);
        if (order == null) return null;

        if (!await IsAuthorizedAsync(order)) return null;

        await TryProactiveStatusSyncAsync(order, cancellationToken);

        var user = await _identityService.GetUserByIdAsync(order.UserId, cancellationToken);
        var displayDescription = GetDisplayDescription(order.Description);

        return MapToDto(order, user?.FullName, displayDescription);
    }

    private async Task<bool> IsAuthorizedAsync(Order order)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue) return false;

        if (order.UserId == currentUserId.Value) return true;

        return await _identityService.IsInRoleAsync(currentUserId.Value, Roles.ClinicStaff) ||
               await _identityService.IsInRoleAsync(currentUserId.Value, Roles.SystemAdmin);
    }

    private async Task TryProactiveStatusSyncAsync(Order order, CancellationToken cancellationToken)
    {
        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed) return;

        var pendingPayOsPayment = order.Payments.FirstOrDefault(p => p.Status == PaymentStatus.Pending && p.Method == PaymentMethod.PayOS);
        if (pendingPayOsPayment == null || string.IsNullOrEmpty(pendingPayOsPayment.PaymentOrderCode)) return;

        try
        {
            var (payOsStatus, _, txnRef) = await _payOSService.GetPaymentStatusAsync(pendingPayOsPayment.PaymentOrderCode);

            if (payOsStatus is "PAID" or "00")
            {
                await HandlePaidStatusAsync(order, pendingPayOsPayment, txnRef, cancellationToken);
            }
            else if (payOsStatus == "CANCELLED")
            {
                await HandleCancelledStatusAsync(order, pendingPayOsPayment, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Proactive sync failed for OrderCode={OrderCode}", pendingPayOsPayment.PaymentOrderCode);
        }
    }

    private async Task HandlePaidStatusAsync(Order order, Payment payment, string? txnRef, CancellationToken cancellationToken)
    {
        payment.Complete(txnRef, "Proactive Sync via GetOrder");

        if (IsDepositPayment(order, payment))
        {
            order.Confirm();
            await SyncAppointmentOnConfirmationAsync(order, cancellationToken);
        }
        else
        {
            order.Complete();
            await _clinicVisitService.ProcessPaymentCompletionAsync(order, "Proactive Sync", cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await SendAppointmentConfirmationEmailIfApplicableAsync(order, cancellationToken);
    }

    private async Task HandleCancelledStatusAsync(Order order, Payment payment, CancellationToken cancellationToken)
    {
        payment.Cancel();
        order.Cancel();

        if (order.AppointmentId.HasValue)
        {
            await CancelAppointmentAndReleaseSlotAsync(order, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static bool IsDepositPayment(Order order, Payment payment)
    {
        return order.Status == OrderStatus.Pending && 
               order.DepositAmount.HasValue && 
               Math.Abs(payment.Amount - order.DepositAmount.Value) < 0.01m;
    }

    private async Task SyncAppointmentOnConfirmationAsync(Order order, CancellationToken cancellationToken)
    {
        if (!order.AppointmentId.HasValue) return;

        var appointment = await _appointmentRepository.GetByIdAsync(order.AppointmentId.Value, cancellationToken);
        if (appointment != null && appointment.Status == AppointmentStatus.Pending)
        {
            appointment.Confirm();
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            _logger.LogInformation("Appointment {AppointmentId} confirmed automatically via proactive status sync for Order {OrderId}.", appointment.Id, order.Id);
        }
    }

    private async Task SendAppointmentConfirmationEmailIfApplicableAsync(Order order, CancellationToken cancellationToken)
    {
        if (order.AppointmentId.HasValue)
        {
            await _mediator.Send(
                new Application.Scheduling.Appointments.Commands.CreateClinicAppointment.SendClinicAppointmentConfirmationEmailCommand(order.AppointmentId.Value),
                cancellationToken);
        }
    }

    private async Task CancelAppointmentAndReleaseSlotAsync(Order order, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(order.AppointmentId.Value, cancellationToken);
        if (appointment != null && appointment.Status == AppointmentStatus.Pending)
        {
            appointment.Cancel(order.UserId, "Payment cancelled by user (Proactive Sync).");

            var slot = await _appointmentSlotRepository.GetByIdAsync(appointment.AppointmentSlotId, cancellationToken);
            if (slot != null)
            {
                slot.CancelBooking();
                await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
            }

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            _logger.LogInformation("Appointment {AppointmentId} cancelled and slot {SlotId} released due to payment cancellation (Proactive Sync).", appointment.Id, appointment.AppointmentSlotId);
        }
    }

    private static string? GetDisplayDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return description;

        if (description.Contains("[Appt:"))
        {
            return System.Text.RegularExpressions.Regex.Replace(description, @"\s*\[Appt:[^\]]+\]", "").Trim();
        }

        return description;
    }

    private static OrderDto MapToDto(Order order, string? userFullName, string? displayDescription)
    {
        return new OrderDto(
            order.Id,
            order.UserId,
            order.TotalAmount,
            order.DepositAmount,
            userFullName,
            displayDescription,
            MapStatusToDto(order.Status),
            order.CreatedAt,
            order.PaidAmount,
            order.Payments.Select(p => new PaymentDto(
                p.Id,
                p.OrderId,
                p.Amount,
                p.Status,
                p.Method,
                p.PaidAt,
                p.PaymentUrl,
                p.Description ?? displayDescription,
                p.PaymentOrderCode)).ToList());
    }

    private static string MapStatusToDto(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Confirmed => "PartiallyPaid",
            OrderStatus.Completed => "FullyPaid",
            _ => status.ToString()
        };
    }
}
