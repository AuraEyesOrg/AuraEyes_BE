# CQRS Query Naming

**Impact: CRITICAL**

Queries must follow the naming convention `Get[Entity]Query` or `Get[Entities]Query`.

## Rule

- Single entity: `Get[Entity]Query`
- Multiple entities: `Get[Entities]Query`
- Search queries: `Search[Entities]Query`
- Queries are records implementing `IQuery<TResponse>`

## Examples

### Query Names

| Purpose | Query Name |
|---------|------------|
| Get single product by ID | `GetProductQuery` |
| Get list of products | `GetProductsQuery` |
| Search products | `SearchProductsQuery` |
| Get product reviews | `GetProductReviewsQuery` |
| Get orders by customer | `GetCustomerOrdersQuery` |

### ❌ Incorrect

```csharp
// Wrong: missing "Query" suffix
public record GetProduct { }

// Wrong: not using IQuery interface
public class GetProductQuery : IRequest<ProductDto> { }

// Wrong: unclear naming
public record ProductQueryRequest { }
```

### ✅ Correct

```csharp
using Application.Common.Interfaces;

namespace Application.Products.Queries.GetProduct;

// Single entity query
public record GetProductQuery(Guid Id) : IQuery<ProductDto>;
```

```csharp
using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Products.Queries.GetProducts;

// List query with filtering and pagination
public record GetProductsQuery : IQuery<PagedResult<ProductListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public ProductStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
    public string? Category { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
}
```

## File Location

```
Application/
└── [Feature]/
    └── Queries/
        ├── Get[Entity]/
        │   ├── Get[Entity]Query.cs
        │   ├── Get[Entity]QueryHandler.cs
        │   └── [Entity]Dto.cs
        └── Get[Entities]/
            ├── Get[Entities]Query.cs
            ├── Get[Entities]QueryHandler.cs
            └── [Entity]ListDto.cs
```

## Why This Matters

- **Consistency**: Predictable query naming
- **Separation**: Clear distinction between single and list queries
- **Type Safety**: IQuery<T> ensures correct return type
