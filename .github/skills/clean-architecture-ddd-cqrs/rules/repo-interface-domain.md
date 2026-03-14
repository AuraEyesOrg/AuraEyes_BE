# Repository Interface in Domain

**Impact: HIGH**

Repository interfaces must be defined in the Domain layer, implementations in Infrastructure.

## Rule

- Interfaces in `Domain/Repositories/`
- Implementations in `Infrastructure/Persistence/Repositories/`
- One repository per Aggregate Root only
- Inherit from generic `IRepository<T>`
- Add domain-specific query methods

## Examples

### ❌ Incorrect

```csharp
// Wrong: interface in Infrastructure
namespace Infrastructure.Repositories;

public interface IProductRepository
{
    Task<Product> GetByIdAsync(Guid id);
}
```

```csharp
// Wrong: repository for non-aggregate
namespace Domain.Repositories;

public interface IProductReviewRepository : IRepository<ProductReview>
{
    // ProductReview is a child entity, not an aggregate root
}
```

### ✅ Correct

**Domain/Repositories/IProductRepository.cs**

```csharp
using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    // Domain-specific queries
    Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Product?> GetProductWithReviewsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}
```

**Infrastructure/Persistence/Repositories/ProductRepository.cs**

```csharp
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<Product?> GetProductWithReviewsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Status == ProductStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(p => p.Name == name, cancellationToken);
    }
}
```

## Registration

```csharp
// Infrastructure/DependencyInjection.cs
services.AddScoped<IProductRepository, ProductRepository>();
```

## Why This Matters

- **Dependency Inversion**: Domain doesn't depend on Infrastructure
- **Testability**: Easy to mock repositories in tests
- **Aggregate Boundaries**: Enforces DDD aggregate rules
- **Single Responsibility**: Each repository handles one aggregate
