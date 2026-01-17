using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IOphthalmologistRepository : IRepository<Ophthalmologist>
{
    Task<Ophthalmologist?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ophthalmologist>> GetVerifiedOphthalmologistsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ophthalmologist>> GetByExperienceRangeAsync(int minYears, int maxYears, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
