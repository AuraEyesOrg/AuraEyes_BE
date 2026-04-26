using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Entities.Financial;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Users;
using Domain.Entities.Consultation;
using Domain.Entities.Scheduling;
using Application.Common.Models;

namespace Application.Financial.Commands.CompleteOrderPayment;

public record CompleteOrderPaymentResponse(
    Guid OrderId, 
    Guid PaymentId, 
    PaymentStatus PaymentStatus, 
    string? PaymentUrl = null, 
    string? PaymentOrderCode = null);

public record CompleteOrderPaymentCommand(Guid OrderId, PaymentMethod Method = PaymentMethod.Cash) : IRequest<Result<CompleteOrderPaymentResponse>>;

public class CompleteOrderPaymentCommandHandler : IRequestHandler<CompleteOrderPaymentCommand, Result<CompleteOrderPaymentResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPayOSService _payOSService;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly INotificationService _notificationService;
    private readonly IIdentityService _identityService;
    private readonly IChatHubService _chatHubService;
    private readonly IClinicVisitService _clinicVisitService;
    private readonly PayOSSettings _payOSSettings;
    private readonly ILogger<CompleteOrderPaymentCommandHandler> _logger;

    public CompleteOrderPaymentCommandHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IPayOSService payOSService,
        IPatientVisitRepository patientVisitRepository,
        IRepository<Patient> patientRepository,
        IAppointmentRepository appointmentRepository,
        IConsultationSessionRepository sessionRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        INotificationService notificationService,
        IIdentityService identityService,
        IChatHubService chatHubService,
        IClinicVisitService clinicVisitService,
        Microsoft.Extensions.Options.IOptions<PayOSSettings> payOSSettings,
        ILogger<CompleteOrderPaymentCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _payOSService = payOSService;
        _patientVisitRepository = patientVisitRepository;
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _sessionRepository = sessionRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _notificationService = notificationService;
        _identityService = identityService;
        _chatHubService = chatHubService;
        _clinicVisitService = clinicVisitService;
        _payOSSettings = payOSSettings.Value;
        _logger = logger;
    }

    public async Task<Result<CompleteOrderPaymentResponse>> Handle(CompleteOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithPaymentsAsync(request.OrderId, cancellationToken);
        if (order == null) return Result<CompleteOrderPaymentResponse>.NotFound("Order not found.");

        if (order.Status == OrderStatus.Completed)
            return Result<CompleteOrderPaymentResponse>.Failure("Order is already completed.");

        // Calculate paid amount
        var paidAmount = order.Payments
            .Where(p => p.Status == PaymentStatus.Completed)
            .Sum(p => p.Amount);

        var remainingAmount = order.TotalAmount - paidAmount;

        if (remainingAmount <= 0)
        {
            order.Complete();

            await _clinicVisitService.ProcessPaymentCompletionAsync(order, "Legacy/Pre-paid", cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<CompleteOrderPaymentResponse>.Success(new CompleteOrderPaymentResponse(order.Id, Guid.Empty, PaymentStatus.Completed));
        }

        // Create final payment
        var description = $"Thanh toán nốt cho đơn {order.Id.ToString().Substring(0, 8)}";
        var payment = new Payment(order.Id, remainingAmount, request.Method, description);
        
        await _paymentRepository.AddAsync(payment, cancellationToken);
        // Save to get the ID for PayOS
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (request.Method == PaymentMethod.Cash)
        {
            payment.Complete("CASH_PAYMENT", "Paid at counter");
            order.Complete();

            await _clinicVisitService.ProcessPaymentCompletionAsync(order, "Cashier", cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<CompleteOrderPaymentResponse>.Success(new CompleteOrderPaymentResponse(
                order.Id, 
                payment.Id, 
                payment.Status));
        }
        else if (request.Method == PaymentMethod.PayOS)
        {
            // Generate PayOS link
            try 
            {
                var returnUrl = _payOSSettings.DefaultReturnUrl;
                var cancelUrl = _payOSSettings.DefaultCancelUrl;

                var (paymentUrl, orderCode) = await _payOSService.CreatePaymentLinkAsync(
                    payment.Id,
                    remainingAmount,
                    description,
                    returnUrl,
                    cancelUrl);

                payment.SetPaymentLink(paymentUrl, orderCode);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<CompleteOrderPaymentResponse>.Success(new CompleteOrderPaymentResponse(
                    order.Id,
                    payment.Id,
                    payment.Status,
                    paymentUrl,
                    orderCode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create PayOS payment link for order {OrderId}", order.Id);
                return Result<CompleteOrderPaymentResponse>.Failure("Failed to create PayOS payment link.");
            }
        }
        else
        {
            return Result<CompleteOrderPaymentResponse>.Failure($"Payment method {request.Method} is not supported via staff dashboard.");
        }
    }
}
