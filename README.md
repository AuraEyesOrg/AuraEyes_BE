# Microservice Boilerplate - Clean Architecture + DDD + CQRS

A production-ready boilerplate for building microservices using ASP.NET Core 8.0 with Clean Architecture, Domain-Driven Design (DDD), and Command Query Responsibility Segregation (CQRS) patterns.

## � Table of Contents

- [Quick Start](#quick-start)
- [Architecture](#architecture-overview)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Product Feature Reference](#product-feature-reference-implementation)
- [Adding New Features](#adding-new-features-step-by-step)
- [Quick Reference Templates](#quick-reference-templates)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Best Practices](#best-practices)
- [Design Patterns](#design-patterns-used)

---

## Architecture Overview

This boilerplate follows Clean Architecture principles with clear separation of concerns across four main layers:

### Domain Layer
- **Entities**: Business objects with identity (e.g., Product, ProductReview)
- **Value Objects**: Immutable objects with no identity (e.g., Money, Address)
- **Aggregates**: Clusters of domain objects treated as a single unit
- **Domain Events**: Events that occur within the domain
- **Repository Interfaces**: Abstractions for data access

### Application Layer
- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations (Get, List, Search)
- **Handlers**: Process commands and queries
- **DTOs**: Data transfer objects
- **Validators**: FluentValidation for input validation
- **Behaviors**: MediatR pipeline behaviors (Logging, Validation, Performance)

### Infrastructure Layer
- **DbContext**: Entity Framework Core configuration
- **Repositories**: Implementation of repository interfaces
- **Configurations**: Entity configurations for EF Core
- **External Services**: Third-party service integrations

### API Layer
- **Controllers**: RESTful API endpoints
- **Middleware**: Exception handling, request logging
- **Configuration**: Swagger, CORS, Health checks

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server (configurable)
- **Validation**: FluentValidation
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI
- **Mediator**: MediatR
- **Mapping**: AutoMapper

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server (or Docker)
- Visual Studio 2022 / VS Code / Rider

### Setup (5 minutes)

```bash
# 1. Clone and navigate
git clone <repository-url>
cd BoilerPlate

# 2. Update connection string in src/API/appsettings.json
# "DefaultConnection": "Your connection string here"

# 3. Run migrations
cd src/Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../API
dotnet ef database update --startup-project ../API

# 4. Run the application
cd ../API
dotnet run

# 5. Open Swagger: https://localhost:5001/swagger
```

### Docker Support

```bash
docker-compose up --build
# API available at: http://localhost:5000 and https://localhost:5001
```

## Project Structure

```
BoilerPlate/
├── src/
│   ├── Domain/                  # Core business logic and domain models
│   │   ├── Common/             # Base classes and interfaces
│   │   ├── Entities/           # Domain entities
│   │   ├── Events/             # Domain events
│   │   ├── Repositories/       # Repository interfaces
│   │   └── ValueObjects/       # Value objects
│   │
│   ├── Application/            # Business rules and use cases
│   │   ├── Common/            # Shared application logic
│   │   │   ├── Behaviors/     # MediatR behaviors
│   │   │   ├── Interfaces/    # Application interfaces
│   │   │   └── Models/        # Common models (Result, PagedResult)
│   │   └── Products/          # Product feature
│   │       ├── Commands/      # Write operations
│   │       └── Queries/       # Read operations
│   │
│   ├── Infrastructure/         # External dependencies
│   │   ├── Persistence/       # Database context and configurations
│   │   │   ├── Configurations/
│   │   │   └── Repositories/
│   │   └── Services/          # External service implementations
│   │
│   └── API/                    # Presentation layer
│       ├── Controllers/       # API controllers
│       ├── Middleware/        # Custom middleware
│       └── Program.cs         # Application entry point
│
├── Dockerfile
├── docker-compose.yml
└── MicroserviceBoilerplate.sln
```

---

## Product Feature (Reference Implementation)

This boilerplate includes a **comprehensive Product feature** demonstrating all patterns:

### What's Included
- ✅ **Aggregate Root**: Product entity with full business logic encapsulation
- ✅ **Child Entity**: ProductReview managed within aggregate
- ✅ **Domain Events**: ProductCreated, ProductUpdated, ProductStockChanged, ProductStatusChanged, ProductReviewAdded
- ✅ **Repository Pattern**: Interface + implementation with 6 custom query methods
- ✅ **CQRS**: Separate commands (CreateProduct, UpdateProduct) and queries (GetProduct, GetProducts)
- ✅ **Validation**: FluentValidation validators for all commands
- ✅ **Pagination & Filtering**: GetProducts with status, price range, category, and search
- ✅ **EF Core**: Entity configurations with relationships
- ✅ **RESTful API**: Full CRUD with Swagger documentation and proper HTTP status codes

### Files Included

```
📁 Domain/
  └── Entities/Product.cs ⭐ START HERE (includes ProductReview child entity)
  └── Events/ProductEvents.cs (5 domain events)
  └── Repositories/IProductRepository.cs (6 custom methods)

📁 Application/Products/
  ├── Commands/CreateProduct/     (Command, Validator, Handler)
  ├── Commands/UpdateProduct/     (Command, Validator, Handler)
  ├── Queries/GetProduct/         (Query, DTO with reviews, Handler)
  └── Queries/GetProducts/        (Query with filtering, DTO, Handler)

📁 Infrastructure/Persistence/
  ├── Configurations/ProductConfiguration.cs
  ├── Configurations/ProductReviewConfiguration.cs  
  └── Repositories/ProductRepository.cs (implements 6 custom queries)

📁 API/Controllers/
  └── ProductsController.cs ⭐ API ENDPOINTS (4 endpoints with full documentation)
```

### Product Business Methods
- `UpdateDetails(name, description, price)` - Update product information
- `UpdateStock(quantity)` - Add/remove stock with validation
- `SetStock(newStock)` - Set absolute stock level
- `UpdateCategory(category)` / `UpdateSku(sku)` - Update classification
- `Activate()` / `Deactivate()` / `Discontinue()` - Manage product lifecycle
- `AddReview(reviewerName, rating, comment)` - Add product review
- `IsInStock()` / `IsLowStock(threshold)` - Stock status helpers

---

## Design Patterns Used

### Clean Architecture
- Dependency Rule: Dependencies point inward
- Domain layer has no external dependencies
- Application layer depends only on Domain
- Infrastructure and API depend on Application

### Domain-Driven Design (DDD)
- **Aggregates**: Product aggregate with ProductReview entities
- **Value Objects**: Money, Address
- **Domain Events**: ProductCreated, ProductUpdated, ProductStockChanged
- **Repository Pattern**: Abstract data access
- **Unit of Work**: Transaction management

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Modify state (CreateProduct, UpdateProduct)
- **Queries**: Read state (GetProduct, GetProducts)
- **Handlers**: Separate handlers for each command/query
- **MediatR**: Implements mediator pattern for CQRS

### Additional Patterns
- **Result Pattern**: Type-safe error handling
- **Pipeline Behavior**: Cross-cutting concerns (validation, logging, performance)
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: IoC container for loose coupling

## Key Features

- ✅ Clean Architecture with clear layer separation
- ✅ Domain-Driven Design with aggregates and value objects
- ✅ CQRS pattern with MediatR
- ✅ FluentValidation for input validation
- ✅ Global exception handling middleware
- ✅ Request/Response logging
- ✅ Structured logging with Serilog
- ✅ Swagger documentation
- ✅ Health checks
- ✅ Docker support
- ✅ EF Core with repository pattern
- ✅ Domain events
- ✅ Unit of Work pattern
- ✅ Result pattern for error handling

## API Endpoints

### Products (Reference Implementation)
- `GET /api/products` - List with pagination, filtering (status, category, price range), search
- `GET /api/products/{id}` - Get product by ID with reviews
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product

### Health
- `GET /health` - Health check endpoint

---

## Database Schema

**Products Table**:
```sql
CREATE TABLE Products (
    Id uniqueidentifier PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    Description nvarchar(2000),
    Sku nvarchar(50),
    Price decimal(18,2) NOT NULL,
    Stock int NOT NULL,
    Status nvarchar(50) NOT NULL,
    Category nvarchar(100),
    LastRestockedAt datetime2,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2,
    CreatedBy nvarchar(100),
    UpdatedBy nvarchar(100)
);

CREATE INDEX IX_Products_Status ON Products(Status);
CREATE INDEX IX_Products_Category ON Products(Category);
```

**ProductReviews Table**:
```sql
CREATE TABLE ProductReviews (
    Id uniqueidentifier PRIMARY KEY,
    ProductId uniqueidentifier NOT NULL,
    ReviewerName nvarchar(100) NOT NULL,
    Rating int NOT NULL,
    Comment nvarchar(2000),
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

CREATE INDEX IX_ProductReviews_ProductId ON ProductReviews(ProductId);
```

---

## Adding New Features (Step-by-Step)

Follow these 8 steps to add a new feature. Use the Product feature as reference.

### Step 1: Create Domain Entity

**Location**: `src/Domain/Entities/YourEntity.cs`

```csharp
using Domain.Common;

namespace Domain.Entities;

public class YourEntity : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public YourEntityStatus Status { get; private set; }
    
    private YourEntity() { } // EF Core
    
    public YourEntity(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");
        
        Name = name;
        Status = YourEntityStatus.Active;
        AddDomainEvent(new YourEntityCreatedEvent(Id));
    }
    
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum YourEntityStatus { Active = 1, Inactive = 2 }
```

**Key Points**: Private setters, validation in constructor/methods, domain events, EF Core constructor

**Example**: See [Product.cs](src/Domain/Entities/Product.cs) for comprehensive implementation

---

## Architecture Patterns

### Request Flow (CQRS)

**Command Flow (Write)**:
```
Client → Controller → MediatR 
  → ValidationBehavior (FluentValidation)
  → CommandHandler 
    → Domain Entity (business logic + validation)
    → Repository.AddAsync()
    → UnitOfWork.SaveChangesAsync()
      → Dispatch Domain Events
  → Return Result<T>
→ HTTP Response (201 Created / 400 Bad Request)
```

**Query Flow (Read)**:
```
Client → Controller → MediatR
  → LoggingBehavior
  → QueryHandler
    → Repository.GetByIdAsync()
    → Map Entity to DTO
  → Return Result<DTO>
→ HTTP Response (200 OK / 404 Not Found)
```

### Layer Dependencies

```
┌─────────────────────────────────────────┐
│           API Layer                     │  ← Presentation
│  Controllers, Middleware                │
└──────────────────┬──────────────────────┘
                   │ depends on
                   ↓
┌─────────────────────────────────────────┐
│       Application Layer                 │  ← Business Rules
│  Commands, Queries, Handlers, DTOs      │
└──────────────────┬──────────────────────┘
                   │ depends on
                   ↓
┌─────────────────────────────────────────┐
│         Domain Layer                    │  ← Core Business
│  Entities, Value Objects, Events        │
└─────────────────────────────────────────┘
                   ↑
                   │ implements
┌──────────────────┴──────────────────────┐
│      Infrastructure Layer               │  ← External
│  Repositories, DbContext, Services      │
└─────────────────────────────────────────┘
```

### Domain Model (Product Example)

```
Product (Aggregate Root)
├── Properties (private setters)
│   ├── Id, Name, Description, Sku
│   ├── Price, Stock, Category
│   ├── Status (enum: Active, Inactive, Discontinued)
│   └── LastRestockedAt
├── Child Entities
│   └── ProductReview[] (managed by aggregate)
├── Domain Events
│   ├── ProductCreatedEvent
│   ├── ProductUpdatedEvent
│   ├── ProductStockChangedEvent
│   ├── ProductStatusChangedEvent
│   └── ProductReviewAddedEvent
└── Business Methods
    ├── UpdateDetails(...)
    ├── UpdateStock(int quantity)
    ├── SetStock(int newStock)
    ├── Activate() / Deactivate() / Discontinue()
    ├── AddReview(...)
    ├── IsInStock() / IsLowStock(threshold)
    └── UpdateCategory(...) / UpdateSku(...)
```

---

## Best Practices

### Domain Layer ✅
- Entities encapsulate business logic (no anemic models)
- Use private setters and validate in constructors/business methods
- Raise domain events for important state changes
- Value objects are immutable
- Zero dependencies on external frameworks

### Application Layer ✅
- One command/query per operation (single responsibility)
- FluentValidation validators for all commands
- Handlers are thin orchestrators (delegate to domain)
- Use DTOs for responses (never expose entities)
- Return Result type for error handling

### Infrastructure Layer ✅
- Repository implementations use EF Core
- Entity configurations separate from entities
- No business logic in repositories
- Use proper indexes and constraints

### API Layer ✅
- Controllers use MediatR (no direct database access)
- Proper HTTP status codes (200, 201, 400, 404)
- XML documentation for Swagger
- Validate ID consistency (route vs body)

### Common Mistakes to Avoid ❌
- ❌ Business logic in controllers → ✅ Put it in domain entities
- ❌ Referencing Infrastructure from Domain → ✅ Use interfaces
- ❌ Exposing entities in API → ✅ Use DTOs
- ❌ Forgetting validators → ✅ Create FluentValidation validators
- ❌ Returning void from handlers → ✅ Use Result pattern
- ❌ Public setters on entities → ✅ Use business methods

---

## Step 2: Create Repository Interface

**Location**: `src/Domain/Repositories/IYourEntityRepository.cs`

```csharp
using Domain.Common;

namespace Domain.Entities;

public class YourEntity : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public YourEntityStatus Status { get; private set; }
    
    private YourEntity() { } // EF Core
    
    public YourEntity(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");
        
        Name = name;
        Status = YourEntityStatus.Active;
        AddDomainEvent(new YourEntityCreatedEvent(Id));
    }
    
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum YourEntityStatus { Active = 1, Inactive = 2 }
```

**Key Points**: Private setters, validation in constructor/methods, domain events, EF Core constructor

**Example**: See [Product.cs](src/Domain/Entities/Product.cs) for comprehensive implementation

### Step 2: Create Repository Interface

**Location**: `src/Domain/Repositories/IYourEntityRepository.cs`

```csharp
using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IYourEntityRepository : IRepository<YourEntity>
{
    Task<YourEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
```

### Step 3: Create Command

**Location**: `src/Application/YourFeature/Commands/CreateYourEntity/`

**CreateYourEntityCommand.cs**:
```csharp
using Application.Common.Interfaces;

namespace Application.YourFeature.Commands.CreateYourEntity;

public record CreateYourEntityCommand : ICommand<Guid>
{
    public string Name { get; init; } = string.Empty;
}
```

**CreateYourEntityCommandValidator.cs**:
```csharp
using FluentValidation;

namespace Application.YourFeature.Commands.CreateYourEntity;

public class CreateYourEntityCommandValidator : AbstractValidator<CreateYourEntityCommand>
{
    public CreateYourEntityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");
    }
}
```

**CreateYourEntityCommandHandler.cs**:
```csharp
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;

namespace Application.YourFeature.Commands.CreateYourEntity;

public class CreateYourEntityCommandHandler : ICommandHandler<CreateYourEntityCommand, Guid>
{
    private readonly IYourEntityRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateYourEntityCommandHandler(IYourEntityRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateYourEntityCommand request, CancellationToken cancellationToken)
    {
        var entity = new YourEntity(request.Name);
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id);
    }
}
```

### Step 4: Create Query

**Location**: `src/Application/YourFeature/Queries/GetYourEntity/`

**GetYourEntityQuery.cs**:
```csharp
using Application.Common.Interfaces;

namespace Application.YourFeature.Queries.GetYourEntity;

public record GetYourEntityQuery(Guid Id) : IQuery<YourEntityDto>;
```

**YourEntityDto.cs**:
```csharp
namespace Application.YourFeature.Queries.GetYourEntity;

public class YourEntityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

**GetYourEntityQueryHandler.cs**:
```csharp
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.YourFeature.Queries.GetYourEntity;

public class GetYourEntityQueryHandler : IQueryHandler<GetYourEntityQuery, YourEntityDto>
{
    private readonly IYourEntityRepository _repository;

    public GetYourEntityQueryHandler(IYourEntityRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<YourEntityDto>> Handle(GetYourEntityQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity == null) 
            return Result<YourEntityDto>.Failure("Not found");
        
        return Result<YourEntityDto>.Success(new YourEntityDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt
        });
    }
}
```

### Step 5: Implement Repository

**Location**: `src/Infrastructure/Persistence/Repositories/YourEntityRepository.cs`

```csharp
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class YourEntityRepository : Repository<YourEntity>, IYourEntityRepository
{
    public YourEntityRepository(ApplicationDbContext context) : base(context) { }

    public async Task<YourEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
    }
}
```

### Step 6: Configure EF Core

**Location**: `src/Infrastructure/Persistence/Configurations/YourEntityConfiguration.cs`

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class YourEntityConfiguration : IEntityTypeConfiguration<YourEntity>
{
    public void Configure(EntityTypeBuilder<YourEntity> builder)
    {
        builder.ToTable("YourEntities");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .IsRequired();
        
        builder.Ignore(e => e.DomainEvents);
    }
}
```

### Step 7: Create API Controller

**Location**: `src/API/Controllers/YourEntitiesController.cs`

```csharp
using Application.Common.Models;
using Application.YourFeature.Commands.CreateYourEntity;
using Application.YourFeature.Queries.GetYourEntity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class YourEntitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public YourEntitiesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<YourEntityDto>>> Get(Guid id)
    {
        var result = await _mediator.Send(new GetYourEntityQuery(id));
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<ActionResult<Result<Guid>>> Create([FromBody] CreateYourEntityCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess 
            ? CreatedAtAction(nameof(Get), new { id = result.Data }, result) 
            : BadRequest(result);
    }
}
```

### Step 8: Register Dependencies

**In `src/Infrastructure/DependencyInjection.cs`**:
```csharp
services.AddScoped<IYourEntityRepository, YourEntityRepository>();
```

**In `src/Infrastructure/Persistence/ApplicationDbContext.cs`**:
```csharp
public DbSet<YourEntity> YourEntities => Set<YourEntity>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfiguration(new YourEntityConfiguration());
    // ... other configurations
}
```

### Step 9: Create Migration

```bash
cd src/Infrastructure
dotnet ef migrations add AddYourEntity --startup-project ../API
dotnet ef database update --startup-project ../API
```

---

## Quick Reference Templates

### Naming Conventions
- **Entity**: `Product`, `Order` (singular, PascalCase)
- **Repository**: `IProductRepository`, `ProductRepository`
- **Command**: `CreateProductCommand`, `UpdateProductCommand`
- **Query**: `GetProductQuery`, `GetProductsQuery`
- **Handler**: `CreateProductCommandHandler`
- **DTO**: `ProductDto`, `ProductListDto`
- **Controller**: `ProductsController` (plural)
- **Route**: `/api/products` (plural, lowercase)

### Common Patterns

**Child Entity (ProductReview example)**:
```csharp
public class ProductReview : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ReviewerName { get; private set; } = string.Empty;
    public int Rating { get; private set; }
    
    private ProductReview() { } // EF Core
    
    public ProductReview(Guid productId, string reviewerName, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5");
        
        ProductId = productId;
        ReviewerName = reviewerName;
        Rating = rating;
    }
}

// In Aggregate:
private readonly List<ProductReview> _reviews = new();
public IReadOnlyCollection<ProductReview> Reviews => _reviews.AsReadOnly();

public void AddReview(string reviewerName, int rating, string comment)
{
    var review = new ProductReview(Id, reviewerName, rating, comment);
    _reviews.Add(review);
}
```

**Domain Event Handler**:
```csharp
public class ProductCreatedEventHandler : IDomainEventHandler<ProductCreatedEvent>
{
    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Send notification, update search index, etc.
    }
}
```

**Pagination with Filtering**:
```csharp
public record GetProductsQuery : IQuery<PagedResult<ProductListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public ProductStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
}

// In Handler - apply filters before pagination
var filtered = allProducts.Where(p => /* filters */);
var pagedResult = new PagedResult<ProductListDto>(
    filtered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
    filtered.Count(),
    pageNumber,
    pageSize
);
```

## License

This project is licensed under the MIT License.

## Support

For questions and support, please open an issue in the GitHub repository.

---

**Quick Tips**:
- 🎯 Start by reviewing [Product.cs](src/Domain/Entities/Product.cs)
- 📋 Copy the Product feature structure for new features
- 🔄 Follow the 8-step process above for consistency
- ✅ Use the checklist in Best Practices section
- 🧪 Write tests as you implement features

**Happy Coding! 🚀**
