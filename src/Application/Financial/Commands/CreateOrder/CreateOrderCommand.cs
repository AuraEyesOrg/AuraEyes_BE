using Application.Common.Interfaces;
using Application.Financial.Common.DTOs;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Financial.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<OrderDto>
{
    public decimal TotalAmount { get; init; }
    public string? Description { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public string? ReturnUrl { get; init; }
    public string? CancelUrl { get; init; }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPayOSService _payOSService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        ICurrentUserService currentUserService,
        IPayOSService payOSService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _currentUserService = currentUserService;
        _payOSService = payOSService;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();

        // 1. Create Order
        var order = new Order(userId, request.TotalAmount, null, request.Description);
        await _orderRepository.AddAsync(order, cancellationToken);

        // 2. Create Initial Payment
        var payment = new Payment(order.Id, request.TotalAmount, request.PaymentMethod, request.Description);
        await _paymentRepository.AddAsync(payment, cancellationToken);

        // 3. If PayOS, generate payment link
        if (request.PaymentMethod == PaymentMethod.PayOS)
        {
            var returnUrl = request.ReturnUrl ?? "https://aura-eyes.com/payment/success"; 
            var cancelUrl = request.CancelUrl ?? "https://aura-eyes.com/payment/cancel";

            var (paymentUrl, orderCode) = await _payOSService.CreatePaymentLinkAsync(
                payment.Id,
                request.TotalAmount,
                request.Description ?? $"Thanh toán đơn hàng {order.Id}",
                returnUrl,
                cancelUrl);

            payment.SetPaymentLink(paymentUrl, orderCode);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new OrderDto(
            order.Id,
            order.UserId,
            order.TotalAmount,
            order.DepositAmount,
            null, // PatientName
            order.Description,
            order.Status,
            order.CreatedAt,
            new List<PaymentDto> { 
                new PaymentDto(
                    payment.Id, 
                    payment.OrderId, 
                    payment.Amount, 
                    payment.Status, 
                    payment.Method, 
                    payment.PaidAt, 
                    payment.PaymentUrl,
                    payment.Description) 
            });
    }
}
