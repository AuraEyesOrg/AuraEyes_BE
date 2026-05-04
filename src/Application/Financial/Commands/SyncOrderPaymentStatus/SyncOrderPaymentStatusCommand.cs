using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Financial.Commands.SyncOrderPaymentStatus;

public record SyncOrderPaymentStatusCommand(Guid OrderId) : IRequest<bool>;

public class SyncOrderPaymentStatusCommandHandler : IRequestHandler<SyncOrderPaymentStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPayOSService _payOSService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClinicVisitService _clinicVisitService;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SyncOrderPaymentStatusCommandHandler> _logger;

    public SyncOrderPaymentStatusCommandHandler(
        IOrderRepository orderRepository,
        IPayOSService payOSService,
        IUnitOfWork unitOfWork,
        IClinicVisitService clinicVisitService,
        INotificationService notificationService,
        ICurrentUserService currentUserService,
        ILogger<SyncOrderPaymentStatusCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _payOSService = payOSService;
        _unitOfWork = unitOfWork;
        _clinicVisitService = clinicVisitService;
        _notificationService = notificationService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<bool> Handle(SyncOrderPaymentStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithPaymentsAsync(request.OrderId, cancellationToken);
        if (order == null) return false;

        // Authorization check: User must be either Staff/Admin OR the owner of the order
        var currentUserId = _currentUserService.UserId;
        bool isStaffOrAdmin = _currentUserService.IsInRole("ClinicStaff") || _currentUserService.IsInRole("SystemAdmin");
        
        if (!isStaffOrAdmin && order.UserId != currentUserId)
        {
            _logger.LogWarning("User {UserId} attempted to sync Order {OrderId} which belongs to {OwnerId}.", 
                currentUserId, order.Id, order.UserId);
            return false; // Resulting in 404/403 behavior via controller logic
        }

        var pendingPayments = order.Payments
            .Where(p => p.Status == PaymentStatus.Pending && !string.IsNullOrEmpty(p.PaymentOrderCode))
            .ToList();

        if (!pendingPayments.Any()) return true;

        bool updated = false;
        foreach (var payment in pendingPayments)
        {
            if (await SyncPaymentStatusAsync(order, payment, cancellationToken))
            {
                updated = true;
            }
        }

        if (updated)
        {
            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    private async Task<bool> SyncPaymentStatusAsync(Order order, Payment payment, CancellationToken cancellationToken)
    {
        try
        {
            var (status, _, txnRef) = await _payOSService.GetPaymentStatusAsync(payment.PaymentOrderCode!);

            if (status == "PAID")
            {
                await HandlePaidPaymentAsync(order, payment, txnRef, cancellationToken);
                return true;
            }

            if (status is "CANCELLED" or "EXPIRED")
            {
                if (status == "CANCELLED")
                    payment.Cancel();
                else
                    payment.Fail(status);

                // If deposit payment fails/cancelled, cancel the order and appointment to release the slot
                if (IsDepositPayment(order, payment))
                {
                    order.Cancel();
                    _logger.LogInformation("Deposit payment {Status} for Order {OrderId}. Cancelling order.", status, order.Id);

                    if (order.AppointmentId.HasValue)
                    {
                        _logger.LogInformation("Cancelling associated Appointment {AppointmentId}.", order.AppointmentId.Value);
                        var appointment = await _clinicVisitService.GetAppointmentByIdAsync(order.AppointmentId.Value, cancellationToken);
                        if (appointment != null && appointment.Status == AppointmentStatus.Pending)
                        {
                            await _clinicVisitService.CancelAppointmentAsync(appointment.Id, $"Payment {status} via PayOS sync", cancellationToken);
                        }
                    }
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to sync payment {PaymentId} status from PayOS.", payment.Id);
        }

        return false;
    }

    private async Task HandlePaidPaymentAsync(Order order, Payment payment, string? txnRef, CancellationToken cancellationToken)
    {
        payment.Complete(txnRef, "Synced via Verify command");
        _logger.LogInformation("Payment {PaymentId} for Order {OrderId} synced to PAID.", payment.Id, order.Id);

        if (IsDepositPayment(order, payment))
        {
            order.Confirm();
            await SendNotificationAsync(order.UserId, "Nạp tiền cọc thành công", $"Bạn đã thanh toán đặt cọc thành công. Số tiền: {payment.Amount:N0} VNĐ", NotificationType.WalletDepositSuccess, order.Id, payment.Amount, cancellationToken);
        }
        else
        {
            order.Complete();
            await SendNotificationAsync(order.UserId, "Thanh toán thành công", $"Thanh toán hoàn tất. Số tiền: {payment.Amount:N0} VNĐ", NotificationType.WalletPaymentProcessed, order.Id, payment.Amount, cancellationToken);
            await _clinicVisitService.ProcessPaymentCompletionAsync(order, "PayOS Polling/Verify", cancellationToken);
        }
    }

    private static bool IsDepositPayment(Order order, Payment payment)
    {
        return order.DepositAmount.HasValue &&
               Math.Abs(payment.Amount - order.DepositAmount.Value) < 0.01m &&
               order.Status == OrderStatus.Pending;
    }

    private async Task SendNotificationAsync(Guid userId, string title, string message, NotificationType type, Guid orderId, decimal amount, CancellationToken cancellationToken)
    {
        await _notificationService.SendAsync(
            userId,
            title,
            message,
            type,
            new { OrderId = orderId, Amount = amount },
            cancellationToken);
    }
}
