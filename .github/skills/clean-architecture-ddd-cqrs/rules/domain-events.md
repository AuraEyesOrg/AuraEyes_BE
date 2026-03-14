# Domain Events

**Impact: MEDIUM**

Domain events communicate state changes within the aggregate. Use them to track significant business occurrences.

## Rule

- Events are immutable records of something that happened
- Inherit from `DomainEvent` base class
- Named in past tense: `[Entity][Action]Event`
- Contain all relevant data for the event
- Raised in entity methods using `AddDomainEvent()`

## Examples

### ❌ Incorrect

```csharp
// Wrong: present tense
public class ProductCreate { }

// Wrong: no data
public class ProductCreatedEvent { }

// Wrong: mutable
public class ProductCreatedEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
}
```

### ✅ Correct

**Domain Event Base Class**

```csharp
namespace Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }
    public Guid EventId { get; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
        EventId = Guid.NewGuid();
    }
}
```

**Product Events**

```csharp
using Domain.Common;

namespace Domain.Events;

public class ProductCreatedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public string ProductName { get; }
    public decimal Price { get; }

    public ProductCreatedEvent(Guid productId, string productName, decimal price)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
    }
}

public class ProductUpdatedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public string ProductName { get; }

    public ProductUpdatedEvent(Guid productId, string productName)
    {
        ProductId = productId;
        ProductName = productName;
    }
}

public class ProductStockChangedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public int PreviousStock { get; }
    public int NewStock { get; }
    public int QuantityChanged { get; }

    public ProductStockChangedEvent(Guid productId, int previousStock, int newStock, int quantityChanged)
    {
        ProductId = productId;
        PreviousStock = previousStock;
        NewStock = newStock;
        QuantityChanged = quantityChanged;
    }
}

public class ProductStatusChangedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public string PreviousStatus { get; }
    public string NewStatus { get; }

    public ProductStatusChangedEvent(Guid productId, string previousStatus, string newStatus)
    {
        ProductId = productId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
    }
}

public class ProductReviewAddedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public Guid ReviewId { get; }
    public int Rating { get; }

    public ProductReviewAddedEvent(Guid productId, Guid reviewId, int rating)
    {
        ProductId = productId;
        ReviewId = reviewId;
        Rating = rating;
    }
}
```

**Using Events in Entity**

```csharp
public class Product : BaseEntity, IAggregateRoot
{
    public Product(string name, decimal price, int stock)
    {
        // ... validation and assignment

        // Raise event on creation
        AddDomainEvent(new ProductCreatedEvent(Id, name, price));
    }

    public void UpdateStock(int quantity)
    {
        var previousStock = Stock;
        
        if (Stock + quantity < 0)
            throw new InvalidOperationException("Insufficient stock");

        Stock += quantity;
        UpdatedAt = DateTime.UtcNow;

        // Raise event on significant change
        AddDomainEvent(new ProductStockChangedEvent(Id, previousStock, Stock, quantity));
    }

    public void Activate()
    {
        if (Status == ProductStatus.Active)
            return;

        var previousStatus = Status.ToString();
        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductStatusChangedEvent(Id, previousStatus, Status.ToString()));
    }
}
```

### Event Handler (Optional)

```csharp
using Application.Common.Interfaces;
using Domain.Events;

namespace Application.Products.EventHandlers;

public class ProductCreatedEventHandler : IDomainEventHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Product created: {ProductId} - {ProductName} at ${Price}",
            notification.ProductId,
            notification.ProductName,
            notification.Price);

        // Could also: send notification, update cache, trigger workflow, etc.

        return Task.CompletedTask;
    }
}
```

## Event Naming Convention

| Action | Event Name |
|--------|------------|
| Created | `ProductCreatedEvent` |
| Updated | `ProductUpdatedEvent` |
| Deleted | `ProductDeletedEvent` |
| Stock changed | `ProductStockChangedEvent` |
| Status changed | `ProductStatusChangedEvent` |
| Review added | `ProductReviewAddedEvent` |
| Shipped | `OrderShippedEvent` |
| Cancelled | `OrderCancelledEvent` |

## Why This Matters

- **Audit Trail**: Track what happened and when
- **Decoupling**: Other parts of system react to events
- **Integration**: Events can be published to message bus
- **Testing**: Easy to verify correct events raised
