using Application.Common.Interfaces;
using Application.Financial.Common.DTOs;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Financial.Queries.GetAllOrders;

public record GetAllOrdersQuery(int PageNumber = 1, int PageSize = 20) : IRequest<AllOrdersResult>;

public record AllOrdersResult(
    IEnumerable<OrderDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    decimal TotalRevenue = 0,
    decimal TotalPending = 0)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, AllOrdersResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IIdentityService _identityService;

    public GetAllOrdersQueryHandler(IOrderRepository orderRepository, IIdentityService identityService)
    {
        _orderRepository = orderRepository;
        _identityService = identityService;
    }

    public async Task<AllOrdersResult> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _orderRepository.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var userIds = items.Select(o => o.UserId).Distinct();
        var users = await _identityService.GetUsersByIdsAsync(userIds, cancellationToken);
        var userMap = users.ToDictionary(u => u.Id, u => u.FullName);

        var dtos = items.Select(order => {
            var displayDescription = order.Description;
            if (!string.IsNullOrEmpty(displayDescription) && displayDescription.Contains("[Appt:"))
            {
                displayDescription = System.Text.RegularExpressions.Regex.Replace(displayDescription, @"\s*\[Appt:[^\]]+\]", "").Trim();
            }
            
            userMap.TryGetValue(order.UserId, out var patientName);

            var paidAmount = order.Payments
                .Where(p => p.Status == PaymentStatus.Completed)
                .Sum(p => p.Amount);

            return new OrderDto(
                order.Id,
                order.UserId,
                order.TotalAmount,
                order.DepositAmount,
                patientName,
                displayDescription,
                order.Status switch
                {
                    OrderStatus.Confirmed => "PartiallyPaid",
                    OrderStatus.Completed => "FullyPaid",
                    _ => order.Status.ToString()
                },
                order.CreatedAt,
                paidAmount,
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
        }).ToList();

        var (totalRevenue, totalPending) = await _orderRepository.GetFinancialSummaryAsync(cancellationToken);

        return new AllOrdersResult(dtos, totalCount, request.PageNumber, request.PageSize, totalRevenue, totalPending);
    }
}
