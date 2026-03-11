using Domain.Entities.Contracts;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ContractTemplateRepository : Repository<ContractTemplate>, IContractTemplateRepository
{
    public ContractTemplateRepository(ApplicationDbContext context) : base(context) { }

    public async Task<ContractTemplate?> GetByIdWithVariablesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.ContractTemplates
            .Include(t => t.Variables)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<ContractTemplate> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        ContractType? type = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ContractTemplates
            .AsNoTracking()
            .Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(term));
        }

        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        if (isActive.HasValue)
            query = query.Where(t => t.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByTypeAndVersionAsync(
        ContractType type,
        string version,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ContractTemplates
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.Type == type && t.ContractVersion == version);

        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}
