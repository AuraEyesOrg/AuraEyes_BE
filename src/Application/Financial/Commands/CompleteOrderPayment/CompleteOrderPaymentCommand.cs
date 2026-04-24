using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Entities.Financial;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Financial.Commands.CompleteOrderPayment;

public record CompleteOrderPaymentCommand(Guid OrderId, PaymentMethod Method = PaymentMethod.Cash) : IRequest<Result<bool>>;

public class CompleteOrderPaymentCommandHandler : IRequestHandler<CompleteOrderPaymentCommand, Result<bool>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteOrderPaymentCommandHandler> _logger;

    public CompleteOrderPaymentCommandHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CompleteOrderPaymentCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(CompleteOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null) return Result<bool>.NotFound("Order not found.");

        if (order.Status == OrderStatus.Completed)
            return Result<bool>.Failure("Order is already completed.");

        // Calculate paid amount
        var paidAmount = order.Payments
            .Where(p => p.Status == PaymentStatus.Completed)
            .Sum(p => p.Amount);

        var remainingAmount = order.TotalAmount - paidAmount;

        if (remainingAmount <= 0)
        {
            order.Complete();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }

        // Create final payment
        var description = $"Thanh toán nốt cho đơn {order.Id.ToString().Substring(0, 8)}";
        var payment = new Payment(order.Id, remainingAmount, request.Method, description);
        
        if (request.Method == PaymentMethod.Cash)
        {
            payment.Complete("CASH_PAYMENT", "Paid at counter");
            order.Complete();
        }
        else
        {
            // If they choose PayOS or other online methods, we might need to generate a link.
            // For now, let's assume it's Cash (walk-in completion).
            // If they want PayOS here, we'd need to return a URL.
            return Result<bool>.Failure("Online payment completion via staff dashboard is not implemented yet. Please use Cash.");
        }

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} completed via staff manual payment ({Method}). Amount: {Amount}", 
            order.Id, request.Method, remainingAmount);

        return Result<bool>.Success(true);
    }
}
