using Application.Financial.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Repositories;
using MediatR;

namespace Application.Financial.Queries.GetUserOrders;

/// <summary>
/// Returns paginated orders for the currently authenticated user,
/// along with their associated payments — used for the "Payment History" page.
/// </summary>
public record GetUserOrdersQuery(int PageNumber = 1, int PageSize = 20) : IRequest<UserOrdersResult>;

public record UserOrdersResult(
    List<OrderDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    bool HasPrevious,
    bool HasNext);

public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, UserOrdersResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetUserOrdersQueryHandler(
        IOrderRepository orderRepository,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _orderRepository = orderRepository;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<UserOrdersResult> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User identity is required.");

        var (items, totalCount) = await _orderRepository.GetPagedAsync(
            userId: userId,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var users = await _identityService.GetUsersByIdsAsync(items.Select(o => o.UserId).Distinct(), cancellationToken);
        var userMap = users.ToDictionary(u => u.Id, u => u.FullName);

        var dtos = items.Select(order => {
            var displayDescription = order.Description;
            if (!string.IsNullOrEmpty(displayDescription))
            {
                displayDescription = System.Text.RegularExpressions.Regex.Replace(displayDescription, @"\s*\[Appt:[^\]]+\]", "").Trim();
            }
            
            userMap.TryGetValue(order.UserId, out var patientName);

            return new OrderDto(
                order.Id,
                order.UserId,
                order.TotalAmount,
                order.DepositAmount,
                patientName,
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
        }).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new UserOrdersResult(
            Items: dtos,
            TotalCount: totalCount,
            PageNumber: request.PageNumber,
            PageSize: request.PageSize,
            TotalPages: totalPages,
            HasPrevious: request.PageNumber > 1,
            HasNext: request.PageNumber < totalPages);
    }
}
