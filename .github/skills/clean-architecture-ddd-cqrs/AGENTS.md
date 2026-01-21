# Clean Architecture + DDD + CQRS Best Practices

**Version 1.0.0**  
AuraEyes Backend  
January 2026

> **Note:**  
> This document is for agents and LLMs to follow when maintaining,  
> generating, or refactoring ASP.NET Core backends. Humans may also  
> find it useful, but guidance here is optimized for automation  
> and consistency by AI-assisted workflows.

---

## Abstract

Comprehensive architectural guide for building microservices using ASP.NET Core 8.0 with Clean Architecture, Domain-Driven Design (DDD), and Command Query Responsibility Segregation (CQRS) patterns. This guide provides detailed rules, real-world examples, and templates to ensure consistency across the codebase.

---

## Table of Contents

1. [Domain Layer](#1-domain-layer) — **CRITICAL**
2. [CQRS Pattern](#2-cqrs-pattern) — **CRITICAL**
3. [Repository Pattern](#3-repository-pattern) — **HIGH**
4. [Validation](#4-validation) — **HIGH**
5. [API Layer](#5-api-layer) — **MEDIUM-HIGH**
6. [Infrastructure](#6-infrastructure) — **MEDIUM**
7. [Cross-Cutting Concerns](#7-cross-cutting-concerns) — **MEDIUM**
8. [Quick Reference Templates](#8-quick-reference-templates)

---

## 1. Domain Layer

**Impact: CRITICAL**

The domain layer is the heart of the application. It contains business logic, entities, value objects, and domain events. It has ZERO external dependencies.

### 1.1 Entity Structure

**Impact: CRITICAL (ensures proper encapsulation)**

Entities must inherit from `BaseEntity`, implement `IAggregateRoot` if they're aggregate roots, use private setters, and validate in constructors/methods.

**Incorrect: Public setters, no validation**

```csharp
public class Order
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
}
```

**Correct: Private setters, validation, domain events**

```csharp
using Domain.Common;
using Domain.Events;

namespace Domain.Entities;

public class Order : BaseEntity, IAggregateRoot
{
    public string CustomerName { get; private set; } = string.Empty;
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // EF Core constructor

    public Order(string customerName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name cannot be empty", nameof(customerName));

        CustomerName = customerName;
        Status = OrderStatus.Pending;
        Total = 0;

        AddDomainEvent(new OrderCreatedEvent(Id, CustomerName));
    }

    public void AddItem(string productName, decimal price, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot modify confirmed order");

        var item = new OrderItem(Id, productName, price, quantity);
        _items.Add(item);
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (!_items.Any())
            throw new InvalidOperationException("Cannot confirm empty order");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderConfirmedEvent(Id, Total));
    }

    private void RecalculateTotal()
    {
        Total = _items.Sum(i => i.Price * i.Quantity);
    }
}

public enum OrderStatus { Pending = 1, Confirmed = 2, Shipped = 3, Cancelled = 4 }
```

### 1.2 Value Objects

**Impact: HIGH (immutability and type safety)**

Value objects are immutable, compared by value, and encapsulate validation logic.

**Incorrect: Using primitive types**

```csharp
public class Product
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}
```

**Correct: Using Value Object**

```csharp
using Domain.Common;

namespace Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money() { } // EF Core

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}
```

### 1.3 Domain Events

**Impact: MEDIUM (decoupled communication)**

Domain events communicate state changes within the aggregate.

```csharp
using Domain.Common;

namespace Domain.Events;

public class OrderCreatedEvent : DomainEvent
{
    public Guid OrderId { get; }
    public string CustomerName { get; }

    public OrderCreatedEvent(Guid orderId, string customerName)
    {
        OrderId = orderId;
        CustomerName = customerName;
    }
}

public class OrderConfirmedEvent : DomainEvent
{
    public Guid OrderId { get; }
    public decimal TotalAmount { get; }

    public OrderConfirmedEvent(Guid orderId, decimal totalAmount)
    {
        OrderId = orderId;
        TotalAmount = totalAmount;
    }
}
```

### 1.4 Repository Interface

**Impact: HIGH (abstraction for data access)**

Repository interfaces belong in Domain layer. One repository per aggregate root.

```csharp
using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetOrderWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersByCustomerAsync(string customerName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCustomerAsync(string customerName, CancellationToken cancellationToken = default);
}
```

---

## 2. CQRS Pattern

**Impact: CRITICAL**

Commands modify state, Queries read state. Separate handlers for each operation.

### 2.1 Command Structure

**Impact: CRITICAL**

Commands use record types, implement ICommand<T>, and have descriptive names.

```csharp
using Application.Common.Interfaces;

namespace Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand : ICommand<Guid>
{
    public string CustomerName { get; init; } = string.Empty;
    public List<OrderItemDto> Items { get; init; } = new();
}

public record OrderItemDto
{
    public string ProductName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
}
```

### 2.2 Command Handler

**Impact: CRITICAL**

Handlers implement ICommandHandler<TCommand, TResponse>, use repository and UnitOfWork, return Result<T>.

```csharp
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = new Order(request.CustomerName);

            foreach (var item in request.Items)
            {
                order.AddItem(item.ProductName, item.Price, item.Quantity);
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(order.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Error creating order: {ex.Message}");
        }
    }
}
```

### 2.3 Command Validator

**Impact: HIGH**

Use FluentValidation for input validation.

```csharp
using FluentValidation;

namespace Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(200).WithMessage("Customer name must not exceed 200 characters");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must have at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductName)
                .NotEmpty().WithMessage("Product name is required");
            
            item.RuleFor(i => i.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero");
            
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero");
        });
    }
}
```

### 2.4 Query Structure

**Impact: CRITICAL**

Queries use record types and implement IQuery<T>.

```csharp
using Application.Common.Interfaces;

namespace Application.Orders.Queries.GetOrder;

public record GetOrderQuery(Guid Id) : IQuery<OrderDto>;
```

### 2.5 Query Handler

**Impact: CRITICAL**

Query handlers read data and map to DTOs.

```csharp
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.Orders.Queries.GetOrder;

public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDto>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(request.Id, cancellationToken);

        if (order == null)
        {
            return Result<OrderDto>.Failure($"Order with ID {request.Id} not found");
        }

        var orderDto = new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            Total = order.Total,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                Id = i.Id,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity,
                Subtotal = i.Price * i.Quantity
            }).ToList()
        };

        return Result<OrderDto>.Success(orderDto);
    }
}
```

### 2.6 DTOs

**Impact: MEDIUM**

DTOs are simple classes for data transfer, located with their query.

```csharp
namespace Application.Orders.Queries.GetOrder;

public class OrderDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}
```

---

## 3. Repository Pattern

**Impact: HIGH**

Repositories abstract data access. Interface in Domain, implementation in Infrastructure.

### 3.1 Generic Repository

**Impact: HIGH**

Base repository with common operations.

```csharp
// Domain/Common/IRepository.cs
using System.Linq.Expressions;

namespace Domain.Common;

public interface IRepository<T> where T : BaseEntity, IAggregateRoot
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
```

### 3.2 Repository Implementation

**Impact: HIGH**

```csharp
// Infrastructure/Persistence/Repositories/OrderRepository.cs
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetOrderWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByCustomerAsync(string customerName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.CustomerName == customerName)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.Status == status)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCustomerAsync(string customerName, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(o => o.CustomerName == customerName, cancellationToken);
    }
}
```

---

## 4. Validation

**Impact: HIGH**

Two levels of validation: Input validation (FluentValidation) and Business validation (Domain entities).

### 4.1 FluentValidation Rules

**Common Patterns:**

```csharp
// String validation
RuleFor(x => x.Name)
    .NotEmpty().WithMessage("Name is required")
    .MaximumLength(200).WithMessage("Name must not exceed 200 characters")
    .Matches(@"^[a-zA-Z\s]+$").WithMessage("Name can only contain letters");

// Numeric validation
RuleFor(x => x.Price)
    .GreaterThan(0).WithMessage("Price must be greater than zero")
    .LessThanOrEqualTo(1000000).WithMessage("Price cannot exceed 1,000,000");

RuleFor(x => x.Quantity)
    .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative")
    .LessThanOrEqualTo(10000).WithMessage("Quantity cannot exceed 10,000");

// Email validation
RuleFor(x => x.Email)
    .NotEmpty().WithMessage("Email is required")
    .EmailAddress().WithMessage("Invalid email format");

// Enum validation
RuleFor(x => x.Status)
    .IsInEnum().WithMessage("Invalid status value");

// Collection validation
RuleFor(x => x.Items)
    .NotEmpty().WithMessage("At least one item is required");

RuleForEach(x => x.Items).SetValidator(new OrderItemValidator());

// Custom validation
RuleFor(x => x.EndDate)
    .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");
```

### 4.2 ValidationBehavior Pipeline

```csharp
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

---

## 5. API Layer

**Impact: MEDIUM-HIGH**

Controllers are thin, only dispatching to MediatR.

### 5.1 Controller Structure

**Incorrect: Fat controller with business logic**

```csharp
[HttpPost]
public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
{
    if (string.IsNullOrEmpty(request.CustomerName))
        return BadRequest("Customer name required");
    
    var order = new Order { CustomerName = request.CustomerName };
    _context.Orders.Add(order);
    await _context.SaveChangesAsync();
    
    return Ok(order);
}
```

**Correct: Thin controller**

```csharp
using Application.Common.Models;
using Application.Orders.Commands.CreateOrder;
using Application.Orders.Queries.GetOrder;
using Application.Orders.Queries.GetOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Orders management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get orders with pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<OrderListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<PagedResult<OrderListDto>>>> GetOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null)
    {
        var query = new GetOrdersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Status = status
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Get order by ID with items
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<OrderDto>>> GetOrder(Guid id)
    {
        var query = new GetOrderQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<Guid>>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetOrder), new { id = result.Data }, result);
    }

    /// <summary>
    /// Update an existing order
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UpdateOrder(Guid id, [FromBody] UpdateOrderCommand command)
    {
        if (id != command.Id)
            return BadRequest(Result.Failure("Route ID does not match command ID"));

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
```

---

## 6. Infrastructure

**Impact: MEDIUM**

Infrastructure implements domain interfaces and provides external integrations.

### 6.1 Entity Configuration

**Impact: MEDIUM**

Use Fluent API for entity configuration.

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Total)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events - not persisted
        builder.Ignore(o => o.DomainEvents);

        // Indexes
        builder.HasIndex(o => o.CustomerName);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
    }
}
```

### 6.2 Dependency Injection Registration

**Impact: MEDIUM**

Register all infrastructure services in DependencyInjection.cs.

```csharp
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Unit of Work
        services.AddScoped<IUnitOfWork>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());

        // Generic Repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Specific Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Services
        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}
```

---

## 7. Cross-Cutting Concerns

**Impact: MEDIUM**

Implement behaviors and middleware for logging, exceptions, and performance.

### 7.1 Exception Handling Middleware

```csharp
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (code, result) = exception switch
        {
            ValidationException validationEx => (HttpStatusCode.BadRequest, new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation Error",
                Detail = validationEx.Message,
                Extensions = { ["errors"] = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) }
            }),
            ArgumentException argEx => (HttpStatusCode.BadRequest, new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Bad Request",
                Detail = argEx.Message
            }),
            InvalidOperationException invalidOpEx => (HttpStatusCode.Conflict, new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = "Conflict",
                Detail = invalidOpEx.Message
            }),
            _ => (HttpStatusCode.InternalServerError, new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred"
            })
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsJsonAsync(result);
    }
}
```

### 7.2 Logging Behavior

```csharp
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Handling {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds}ms", 
            requestName, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
```

### 7.3 Performance Behavior

```csharp
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning("Long Running Request: {RequestName} ({ElapsedMilliseconds}ms)",
                requestName, elapsedMilliseconds);
        }

        return response;
    }
}
```

---

## 8. Quick Reference Templates

### 8.1 Creating a New Feature Checklist

When adding a new feature `[Entity]`:

```
□ Domain/Entities/[Entity].cs
  - Inherit from BaseEntity, implement IAggregateRoot
  - Private setters, validation in constructor/methods
  - Add domain events for state changes

□ Domain/Events/[Entity]Events.cs
  - [Entity]CreatedEvent, [Entity]UpdatedEvent, etc.

□ Domain/Repositories/I[Entity]Repository.cs
  - Inherit from IRepository<[Entity]>
  - Add domain-specific query methods

□ Application/[Feature]/Commands/Create[Entity]/
  - Create[Entity]Command.cs (record)
  - Create[Entity]CommandHandler.cs
  - Create[Entity]CommandValidator.cs

□ Application/[Feature]/Commands/Update[Entity]/
  - Update[Entity]Command.cs
  - Update[Entity]CommandHandler.cs
  - Update[Entity]CommandValidator.cs

□ Application/[Feature]/Queries/Get[Entity]/
  - Get[Entity]Query.cs
  - Get[Entity]QueryHandler.cs
  - [Entity]Dto.cs

□ Application/[Feature]/Queries/Get[Entities]/
  - Get[Entities]Query.cs (with pagination)
  - Get[Entities]QueryHandler.cs
  - [Entity]ListDto.cs

□ Infrastructure/Persistence/Configurations/[Entity]Configuration.cs
  - Table name, columns, indexes, relationships

□ Infrastructure/Persistence/Repositories/[Entity]Repository.cs
  - Implement I[Entity]Repository

□ Infrastructure/DependencyInjection.cs
  - Register I[Entity]Repository

□ API/Controllers/[Entities]Controller.cs
  - GET (list), GET (by id), POST, PUT, DELETE
```

### 8.2 Naming Conventions

| Type | Convention | Example |
|------|------------|---------|
| Entity | PascalCase, singular | `Product`, `Order` |
| Value Object | PascalCase, descriptive | `Money`, `Address` |
| Command | `[Verb][Entity]Command` | `CreateProductCommand` |
| Query | `Get[Entity]Query` | `GetProductQuery` |
| Handler | `[CommandName]Handler` | `CreateProductCommandHandler` |
| Validator | `[CommandName]Validator` | `CreateProductCommandValidator` |
| DTO | `[Entity]Dto` | `ProductDto`, `ProductListDto` |
| Repository Interface | `I[Entity]Repository` | `IProductRepository` |
| Repository Impl | `[Entity]Repository` | `ProductRepository` |
| Configuration | `[Entity]Configuration` | `ProductConfiguration` |
| Controller | `[Entities]Controller` | `ProductsController` |
| Event | `[Entity][Action]Event` | `ProductCreatedEvent` |

### 8.3 File Location Rules

```
Commands → Application/[Feature]/Commands/[Action]/
Queries → Application/[Feature]/Queries/[Action]/
Entities → Domain/Entities/
Events → Domain/Events/
Value Objects → Domain/ValueObjects/
Repository Interfaces → Domain/Repositories/
Repository Implementations → Infrastructure/Persistence/Repositories/
Entity Configurations → Infrastructure/Persistence/Configurations/
Controllers → API/Controllers/
Middleware → API/Middleware/
```

---

## Summary

This architecture ensures:

1. **Separation of Concerns**: Each layer has a single responsibility
2. **Dependency Inversion**: High-level modules don't depend on low-level modules
3. **Testability**: Domain and Application layers can be unit tested in isolation
4. **Maintainability**: Clear structure makes code easy to find and modify
5. **Scalability**: Features can be added without modifying existing code
6. **Consistency**: Standard patterns across all features

Always refer to the **Product** feature in this codebase as the reference implementation.
