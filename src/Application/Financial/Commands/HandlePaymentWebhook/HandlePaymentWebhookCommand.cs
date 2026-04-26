using Application.Common.Interfaces;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
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
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<Domain.Entities.Users.Patient> _patientRepository;
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
        IPatientVisitRepository patientVisitRepository,
        IConsultationSessionRepository sessionRepository,
        IRepository<Domain.Entities.Users.Patient> patientRepository,
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
        _patientVisitRepository = patientVisitRepository;
        _sessionRepository = sessionRepository;
        _patientRepository = patientRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<bool> Handle(HandlePaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify signature
        var isValid = await _payOSService.VerifyWebhookSignatureAsync(request.Signature, request.Payload);
        if (!isValid)
        {
            _logger.LogWarning("Invalid PayOS webhook signature received.");
            return false;
        }

        // 2. Parse webhook payload
        string? orderCode = null;
        string? status = null;
        string? txnRef = null;

        try
        {
            using var doc = JsonDocument.Parse(request.Payload);
            var root = doc.RootElement;

            // PayOS webhook root structure:
            // { "code": "00", "desc": "success", "data": { "orderCode": 12345, "status": "PAID", "transactions": [...] } }

            // Root-level code ("00" = success)
            if (root.TryGetProperty("code", out var rootCode))
            {
                var code = rootCode.GetString();
                if (code == "00") status = "PAID";
                else if (code == "01" || code == "02") status = "CANCELLED";
            }

            // data node
            if (root.TryGetProperty("data", out var data))
            {
                // orderCode in data (can be number or string)
                if (data.TryGetProperty("orderCode", out var ocNode))
                {
                    orderCode = ocNode.ValueKind == JsonValueKind.Number
                        ? ocNode.GetInt64().ToString()
                        : ocNode.GetString();
                }

                // status override if present in data (e.g. "PAID", "CANCELLED")
                if (data.TryGetProperty("status", out var dataStatus))
                {
                    var ds = dataStatus.GetString();
                    if (!string.IsNullOrEmpty(ds))
                        status = ds.ToUpperInvariant();
                }

                // Transaction reference
                if (data.TryGetProperty("transactions", out var txnsNode)
                    && txnsNode.ValueKind == JsonValueKind.Array
                    && txnsNode.GetArrayLength() > 0)
                {
                    if (txnsNode[0].TryGetProperty("reference", out var refNode))
                        txnRef = refNode.GetString();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse PayOS webhook payload.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(orderCode))
        {
            _logger.LogWarning("PayOS webhook missing orderCode in payload.");
            return true; // Acknowledge to PayOS even if we can't find the order
        }

        _logger.LogInformation(
            "PayOS webhook received: OrderCode={OrderCode}, Status={Status}, TxnRef={TxnRef}",
            orderCode, status, txnRef);

        // 3. Find payment by orderCode
        var payment = await _paymentRepository.GetByOrderCodeAsync(orderCode, cancellationToken);
        if (payment == null)
        {
            _logger.LogWarning("PayOS webhook: No payment found for OrderCode={OrderCode}", orderCode);
            return true; // Acknowledge anyway
        }

        // 4. Update payment & order status
        var isPaid = status is "PAID" or "00";

        if (isPaid && payment.Status == PaymentStatus.Pending)
        {
            payment.Complete(txnRef, request.Payload);
            await _paymentRepository.UpdateAsync(payment, cancellationToken);

            var order = await _orderRepository.GetWithPaymentsAsync(payment.OrderId, cancellationToken);
            if (order != null)
            {
                // If it's a deposit payment, mark order as Confirmed. 
                // If it's a full payment (or final installment), mark as Completed.
                if (order.DepositAmount.HasValue && Math.Abs(payment.Amount - order.DepositAmount.Value) < 0.01m && order.Status == OrderStatus.Pending)
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
                    _logger.LogInformation("Appointment {AppointmentId} confirmed automatically via successful deposit for Order {OrderId}.", appointment.Id, order.Id);
                }
            }
                    _logger.LogInformation("Order {OrderId} confirmed (deposit received).", order.Id);
                }
                else
                {
                    // Basic logic: if this payment completes the total amount, or if no deposit was defined
                    order.Complete();
                    _logger.LogInformation("Order {OrderId} completed.", order.Id);
                }

                // CHECK FOR CLINIC VISIT METADATA IN DESCRIPTION
                if (!string.IsNullOrWhiteSpace(order.Description) && order.Description.StartsWith("METADATA:"))
                {
                    await HandleClinicVisitCompletionAsync(order, cancellationToken);
                }

                // If this is a clinic booking order, extract AppointmentId and send confirmation email
                if (order.AppointmentId.HasValue)
                {
                    await _mediator.Send(
                        new Application.Scheduling.Appointments.Commands.CreateClinicAppointment.SendClinicAppointmentConfirmationEmailCommand(order.AppointmentId.Value),
                        cancellationToken);
                }
            }
        }
        else if (status is "CANCELLED" && payment.Status == PaymentStatus.Pending)
        {
            payment.Cancel();

            var order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
            if (order != null)
            {
                order.Cancel();

                // If this is a clinic booking, we must also cancel the appointment and release the slot
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
                        _logger.LogInformation("Appointment {AppointmentId} cancelled and slot {SlotId} released due to payment cancellation.", appointment.Id, appointment.AppointmentSlotId);
                    }
                }
            }
        }
        else
        {
            _logger.LogInformation(
                "PayOS webhook: payment {PaymentId} already in status {Status}, no update needed.",
                payment.Id, payment.Status);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task HandleClinicVisitCompletionAsync(Order order, CancellationToken cancellationToken)
    {
        try
        {
            var marker = "METADATA:";
            var pipeIndex = order.Description!.IndexOf(" |");
            var json = pipeIndex > 0 
                ? order.Description[marker.Length..pipeIndex].Trim()
                : order.Description[marker.Length..].Trim();

            var metadata = JsonSerializer.Deserialize<JsonElement>(json);
            if (metadata.TryGetProperty("V", out var visitIdProp))
            {
                var visitId = visitIdProp.GetGuid();
                var visit = await _patientVisitRepository.GetByIdWithDetailsAsync(visitId, cancellationToken);
                
                if (visit != null && visit.Status == PatientVisitStatus.WaitingForPayment)
                {
                    _logger.LogInformation("Processing clinic visit completion for Visit {VisitId} after payment.", visitId);
                    
                    visit.Complete("Payment received via Cashier/PayOS");
                    await _patientVisitRepository.UpdateAsync(visit, cancellationToken);

                    if (visit.AppointmentId.HasValue)
                    {
                        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(visit.AppointmentId.Value, cancellationToken);
                        if (appointment != null)
                        {
                            appointment.Complete();
                            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

                            // Create Consultation Chat Session (Post-visit follow-up)
                            if (appointment.AppointmentSlot?.ScheduleTemplate != null)
                            {
                                var session = ConsultationSession.CreateClinicBooking(
                                    appointment.PatientId,
                                    appointment.AppointmentSlot.ScheduleTemplate.OrgId.GetValueOrDefault(),
                                    0,
                                    DateTime.UtcNow,
                                    visit.AssignedDoctorId.GetValueOrDefault());

                                session.OpenChat();
                                session.EndSession(visit.AssignedDoctorId ?? Guid.Empty, "ClinicVisitPaidAndCompleted");
                                
                                await _sessionRepository.AddAsync(session, cancellationToken);

                                // Notify patient
                                var patient = await _patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
                                if (patient?.UserId != null)
                                {
                                    await _notificationService.SendAsync(
                                        patient.UserId.Value,
                                        "Kết quả khám lâm sàng & Tư vấn",
                                        "Thanh toán hoàn tất. Bạn có thể trao đổi thêm với bác sĩ trong vòng 14 ngày qua mục Chat.",
                                        NotificationType.ConsultationResultProvided,
                                        new { ConsultationId = session.Id, AppointmentId = appointment.Id },
                                        cancellationToken,
                                        session.Id);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process clinic visit completion from order metadata.");
        }
    }
}
