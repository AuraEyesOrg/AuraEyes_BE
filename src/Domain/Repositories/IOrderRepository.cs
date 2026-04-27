using Domain.Common;
using Domain.Entities.Financial;

namespace Domain.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithPaymentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedAsync(
        Guid? userId = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByAppointmentIdsAsync(IEnumerable<Guid> appointmentIds, CancellationToken cancellationToken = default);
}
