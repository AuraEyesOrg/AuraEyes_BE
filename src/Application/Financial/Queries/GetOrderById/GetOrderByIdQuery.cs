using Application.Financial.Common.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Financial.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetWithPaymentsAsync(request.Id, cancellationToken);
        if (order == null) return null;

        return new OrderDto(
            order.Id,
            order.UserId,
            order.TotalAmount,
            order.Description,
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
                p.Description)).ToList());
    }
}
