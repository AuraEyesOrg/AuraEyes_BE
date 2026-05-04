using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetWithPaymentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedAsync(
        Guid? userId = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(o => o.Payments)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Order>> GetByAppointmentIdsAsync(IEnumerable<Guid> appointmentIds, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Payments)
            .Where(o => o.AppointmentId.HasValue && appointmentIds.Contains(o.AppointmentId.Value))
            .ToListAsync(cancellationToken);
    }

    public async Task<(decimal TotalRevenue, decimal TotalPending)> GetFinancialSummaryAsync(CancellationToken cancellationToken = default)
    {
        var paidStatuses = new[] { OrderStatus.Confirmed, OrderStatus.Completed };
        
        // Revenue is what we actually keep (Completed payments - Refunded payments)
        var completedTotal = await _dbSet
            .Where(o => paidStatuses.Contains(o.Status))
            .SelectMany(o => o.Payments)
            .Where(p => p.Status == PaymentStatus.Completed)
            .SumAsync(p => p.Amount, cancellationToken);

        var refundedTotal = await _dbSet
            .SelectMany(o => o.Payments)
            .Where(p => p.Status == PaymentStatus.Refunded)
            .SumAsync(p => p.Amount, cancellationToken);

        var totalRevenue = completedTotal - refundedTotal;

        // Pending is the remaining amount on orders that are still active (not cancelled, not refunded, not fully paid)
        var totalPending = await _dbSet
            .Where(o => o.Status != OrderStatus.Cancelled && 
                        o.Status != OrderStatus.Refunded && 
                        o.Status != OrderStatus.Completed) // Completed in OrderStatus enum usually means FullyPaid in DTO
            .SumAsync(o => o.TotalAmount - o.Payments
                .Where(p => p.Status == PaymentStatus.Completed)
                .Sum(p => p.Amount), cancellationToken);

        return (totalRevenue, totalPending);
    }
}
