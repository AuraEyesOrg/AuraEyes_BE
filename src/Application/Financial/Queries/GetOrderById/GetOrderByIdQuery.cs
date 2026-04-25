using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Financial.Common.DTOs;
using Domain.Common;
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
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithPaymentsAsync(request.Id, cancellationToken);
        if (order == null) return null;

        // Security: only return the order if it belongs to the current user OR if they are staff
        var currentUserId = _currentUserService.UserId;
        bool isStaff = false;
        if (currentUserId.HasValue)
        {
            isStaff = await _identityService.IsInRoleAsync(currentUserId.Value, Roles.ClinicStaff) ||
                      await _identityService.IsInRoleAsync(currentUserId.Value, Roles.SystemAdmin);
        }

        if (!isStaff && currentUserId.HasValue && order.UserId != currentUserId.Value)
            return null;

        // Proactive Status Sync for Local Dev / Polling
        if (order.Status == OrderStatus.Pending)
        {
            var firstPayment = order.Payments.FirstOrDefault(p => p.Status == PaymentStatus.Pending && p.Method == PaymentMethod.PayOS);
            if (firstPayment != null && !string.IsNullOrEmpty(firstPayment.PaymentOrderCode))
            {
                try
                {
                    var (payOsStatus, _, txnRef) = await _payOSService.GetPaymentStatusAsync(firstPayment.PaymentOrderCode);
                    
                    if (payOsStatus is "PAID" or "00")
                    {
                        firstPayment.Complete(txnRef, "Proactive Sync via GetOrder");
                        
                        if (order.DepositAmount.HasValue && Math.Abs(firstPayment.Amount - order.DepositAmount.Value) < 0.01m)
                        {
                            order.Confirm();
                        
                        // Sync with Appointment if this is a clinic booking deposit
                        if (order.AppointmentId.HasValue)
                        {
                            var appointment = await _appointmentRepository.GetByIdAsync(order.AppointmentId.Value, cancellationToken);
                            if (appointment != null && appointment.Status == AppointmentStatus.Pending)
                            {
                                appointment.Confirm();
                                await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
                                _logger.LogInformation("Appointment {AppointmentId} confirmed automatically via proactive status sync for Order {OrderId}.", appointment.Id, order.Id);
                            }
                        }
                        }
                        else
                        {
                            order.Complete();
                        }
                        
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        
                        // Trigger email logic if it's a clinic booking
                        if (order.AppointmentId.HasValue)
                        {
                            await _mediator.Send(
                                new Application.Scheduling.Appointments.Commands.CreateClinicAppointment.SendClinicAppointmentConfirmationEmailCommand(order.AppointmentId.Value),
                                cancellationToken);
                        }
                    }
                    else if (payOsStatus is "CANCELLED")
                    {
                        firstPayment.Cancel();
                        order.Cancel();
                        
                        // If this is a clinic booking, we must also cancel the appointment and release the slot
                        if (order.AppointmentId.HasValue)
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
                        
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Proactive sync failed for OrderCode={OrderCode}", firstPayment.PaymentOrderCode);
                }
            }
        }

        // Fetch patient name
        var user = await _identityService.GetUserByIdAsync(order.UserId, cancellationToken);

        // Strip out the internal [Appt:...] tag from the description before sending to UI (Legacy check)
        var displayDescription = order.Description;
        if (!string.IsNullOrEmpty(displayDescription) && displayDescription.Contains("[Appt:"))
        {
            displayDescription = System.Text.RegularExpressions.Regex.Replace(displayDescription, @"\s*\[Appt:[^\]]+\]", "").Trim();
        }

        return new OrderDto(
            order.Id,
            order.UserId,
            order.TotalAmount,
            order.DepositAmount,
            user?.FullName,
            displayDescription,
            order.Status,
            order.CreatedAt,
            order.Payments.Select(p => new PaymentDto(
                p.Id,
                p.OrderId,
                p.Amount,
                p.Status,
                p.Method,
                p.PaidAt,
                p.PaymentUrl,
                displayDescription)).ToList());
    }
}
