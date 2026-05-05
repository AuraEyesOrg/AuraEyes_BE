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
            .AsNoTracking()
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
            .AsNoTracking()
            .Include(o => o.Payments)
            .Where(o => o.AppointmentId.HasValue && appointmentIds.Contains(o.AppointmentId.Value))
            .ToListAsync(cancellationToken);
    }

    public async Task<(decimal TotalRevenue, decimal TotalPending)> GetFinancialSummaryAsync(CancellationToken cancellationToken = default)
    {
        var paidStatuses = new[] { OrderStatus.Confirmed, OrderStatus.Completed };
        
        // Revenue is what we actually keep (Completed payments - Refunded payments)
        // Sequential awaits are required because EF Core DbContext is not thread-safe
        var completedTotal = await _dbSet
            .AsNoTracking()
            .Where(o => paidStatuses.Contains(o.Status))
            .SelectMany(o => o.Payments)
            .Where(p => p.Status == PaymentStatus.Completed)
            .SumAsync(p => p.Amount, cancellationToken);

        var refundedTotal = await _dbSet
            .AsNoTracking()
            .SelectMany(o => o.Payments)
            .Where(p => p.Status == PaymentStatus.Refunded)
            .SumAsync(p => p.Amount, cancellationToken);

        var totalPending = await _dbSet
            .AsNoTracking()
            .Where(o => o.Status != OrderStatus.Cancelled && 
                        o.Status != OrderStatus.Refunded && 
                        o.Status != OrderStatus.Completed)
            .SumAsync(o => o.TotalAmount - o.Payments
                .Where(p => p.Status == PaymentStatus.Completed)
                .Sum(p => p.Amount), cancellationToken);

        var totalRevenue = completedTotal - refundedTotal;
        return (totalRevenue, totalPending);
    }
}
