using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Domain.Entities.Scheduling;
using Domain.Entities.Financial;
using Domain.Entities.Consultation;

namespace Application.Financial.Commands.HandlePaymentWebhook;

public record HandlePaymentWebhookCommand(string Signature, string Payload) : IRequest<bool>;

public class HandlePaymentWebhookCommandHandler : IRequestHandler<HandlePaymentWebhookCommand, bool>
{
    private readonly IPayOSService _payOSService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IClinicVisitService _clinicVisitService;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HandlePaymentWebhookCommandHandler> _logger;
    private readonly IMediator _mediator;

    public HandlePaymentWebhookCommandHandler(
        IPayOSService payOSService,
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IClinicVisitService clinicVisitService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<HandlePaymentWebhookCommandHandler> logger,
        IMediator mediator)
    {
        _payOSService = payOSService;
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _clinicVisitService = clinicVisitService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<bool> Handle(HandlePaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        if (!await _payOSService.VerifyWebhookSignatureAsync(request.Signature, request.Payload))
        {
            _logger.LogWarning("Invalid PayOS webhook signature received.");
            return false;
        }

        var (orderCode, status, txnRef) = ParseWebhookPayload(request.Payload);

        if (string.IsNullOrWhiteSpace(orderCode))
        {
            _logger.LogWarning("PayOS webhook missing orderCode in payload.");
            return true;
        }

        _logger.LogInformation("PayOS webhook received: OrderCode={OrderCode}, Status={Status}, TxnRef={TxnRef}", orderCode, status, txnRef);

        var payment = await _paymentRepository.GetByOrderCodeAsync(orderCode, cancellationToken);
        if (payment == null)
        {
            _logger.LogWarning("PayOS webhook: No payment found for OrderCode={OrderCode}", orderCode);
            return true;
        }

        await ProcessStatusUpdateAsync(payment, status, txnRef, request.Payload, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task ProcessStatusUpdateAsync(Payment payment, string? status, string? txnRef, string payload, CancellationToken cancellationToken)
    {
        if (payment.Status != PaymentStatus.Pending)
        {
            _logger.LogInformation("PayOS webhook: payment {PaymentId} already in status {Status}, no update needed.", payment.Id, payment.Status);
            return;
        }

        if (status is "PAID" or "00")
        {
            await ProcessSuccessfulPaymentAsync(payment, txnRef, payload, cancellationToken);
        }
        else if (status is "CANCELLED")
        {
            await ProcessCancelledPaymentAsync(payment, cancellationToken);
        }
    }

    private (string? OrderCode, string? Status, string? TxnRef) ParseWebhookPayload(string payload)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;
            string? orderCode = null;
            string? status = null;
            string? txnRef = null;

            if (root.TryGetProperty("code", out var rootCode))
            {
                var code = rootCode.GetString();
                if (code == "00") status = "PAID";
                else if (code == "01" || code == "02") status = "CANCELLED";
            }

            if (root.TryGetProperty("data", out var data))
            {
                if (data.TryGetProperty("orderCode", out var ocNode))
                    orderCode = ocNode.ValueKind == JsonValueKind.Number ? ocNode.GetInt64().ToString() : ocNode.GetString();

                if (data.TryGetProperty("status", out var dataStatus))
                    status = dataStatus.GetString()?.ToUpperInvariant() ?? status;

                if (data.TryGetProperty("transactions", out var txnsNode) && txnsNode.ValueKind == JsonValueKind.Array && txnsNode.GetArrayLength() > 0)
                {
                    if (txnsNode[0].TryGetProperty("reference", out var refNode))
                        txnRef = refNode.GetString();
                }
            }
            return (orderCode, status, txnRef);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse PayOS webhook payload.");
            return (null, null, null);
        }
    }

    private async Task ProcessSuccessfulPaymentAsync(Payment payment, string? txnRef, string payload, CancellationToken cancellationToken)
    {
        payment.Complete(txnRef, payload);
        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        var order = await _orderRepository.GetWithPaymentsAsync(payment.OrderId, cancellationToken);
        if (order == null) return;

        if (order.DepositAmount.HasValue && Math.Abs(payment.Amount - order.DepositAmount.Value) < 0.01m && order.Status == OrderStatus.Pending)
        {
            order.Confirm();
            await _notificationService.SendAsync(order.UserId, "Nạp tiền cọc thành công", $"Bạn đã thanh toán đặt cọc thành công. Số tiền: {payment.Amount:N0} VNĐ", NotificationType.WalletDepositSuccess, new { OrderId = order.Id, Amount = payment.Amount }, cancellationToken);
        }
        else
        {
            order.Complete();
            await _notificationService.SendAsync(order.UserId, "Thanh toán thành công", $"Thanh toán hoàn tất. Số tiền: {payment.Amount:N0} VNĐ", NotificationType.WalletPaymentProcessed, new { OrderId = order.Id, Amount = payment.Amount }, cancellationToken);
        }

        await _clinicVisitService.ProcessPaymentCompletionAsync(order, "PayOS Webhook", cancellationToken);

        if (order.AppointmentId.HasValue)
        {
            await _mediator.Send(new Application.Scheduling.Appointments.Commands.CreateClinicAppointment.SendClinicAppointmentConfirmationEmailCommand(order.AppointmentId.Value), cancellationToken);
        }
    }

    private async Task ProcessCancelledPaymentAsync(Payment payment, CancellationToken cancellationToken)
    {
        payment.Cancel();
        var order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
        if (order == null) return;

        order.Cancel();
        if (order.AppointmentId.HasValue)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(order.AppointmentId.Value, cancellationToken);
            if (appointment != null && appointment.Status == AppointmentStatus.Pending)
            {
                appointment.Cancel(order.UserId, "Payment cancelled by user.");
                var slot = await _appointmentSlotRepository.GetByIdAsync(appointment.AppointmentSlotId, cancellationToken);
                if (slot != null)
                {
                    slot.CancelBooking();
                    await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
                }
                await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            }
        }
    }
}
