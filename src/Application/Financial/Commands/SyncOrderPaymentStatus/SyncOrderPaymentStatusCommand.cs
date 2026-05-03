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
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPayOSService _payOSService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClinicVisitService _clinicVisitService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SyncOrderPaymentStatusCommandHandler> _logger;

    public SyncOrderPaymentStatusCommandHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IPayOSService payOSService,
        IUnitOfWork unitOfWork,
        IClinicVisitService clinicVisitService,
        INotificationService notificationService,
        ILogger<SyncOrderPaymentStatusCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _payOSService = payOSService;
        _unitOfWork = unitOfWork;
        _clinicVisitService = clinicVisitService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<bool> Handle(SyncOrderPaymentStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithPaymentsAsync(request.OrderId, cancellationToken);
        if (order == null) return false;

        var pendingPayments = order.Payments
            .Where(p => p.Status == PaymentStatus.Pending && !string.IsNullOrEmpty(p.PaymentOrderCode))
            .ToList();

        if (!pendingPayments.Any()) return true;

        bool updated = false;

        foreach (var payment in pendingPayments)
        {
            try
            {
                var (status, amount, txnRef) = await _payOSService.GetPaymentStatusAsync(payment.PaymentOrderCode!);
                
                if (status == "PAID")
                {
                    payment.Complete(txnRef, "Synced via Verify command");
                    updated = true;
                    
                    _logger.LogInformation("Payment {PaymentId} for Order {OrderId} synced to PAID.", payment.Id, order.Id);

                    // Logic to update order status
                    if (order.DepositAmount.HasValue && Math.Abs(payment.Amount - order.DepositAmount.Value) < 0.01m && order.Status == OrderStatus.Pending)
                    {
                        order.Confirm();
                        
                        await _notificationService.SendAsync(
                            order.UserId,
                            "Nạp tiền cọc thành công",
                            $"Bạn đã thanh toán đặt cọc thành công. Số tiền: {payment.Amount:N0} VNĐ",
                            NotificationType.WalletDepositSuccess,
                            new { OrderId = order.Id, Amount = payment.Amount },
                            cancellationToken);
                    }
                    else
                    {
                        order.Complete();
                        
                        await _notificationService.SendAsync(
                            order.UserId,
                            "Thanh toán thành công",
                            $"Thanh toán hoàn tất. Số tiền: {payment.Amount:N0} VNĐ",
                            NotificationType.WalletPaymentProcessed,
                            new { OrderId = order.Id, Amount = payment.Amount },
                            cancellationToken);

                        // Notify clinic visit service to complete the visit and appointment
                        await _clinicVisitService.ProcessPaymentCompletionAsync(order, "PayOS Polling/Verify", cancellationToken);
                    }
                }
                else if (status == "CANCELLED" || status == "EXPIRED")
                {
                    payment.Fail(status);
                    updated = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sync payment {PaymentId} status from PayOS.", payment.Id);
            }
        }

        if (updated)
        {
            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}
