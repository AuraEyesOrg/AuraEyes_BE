# Domain Entity Encapsulation

**Impact: CRITICAL**

Entities must use private setters, validate in constructors and methods, and encapsulate all business logic.

## Rule

- All properties must have `private set`
- Validation logic must be in constructor and business methods
- State changes only through explicit methods
- Raise domain events on significant state changes

## Example

### ❌ Incorrect

```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

### ✅ Correct

```csharp
public class Product : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    private Product() { } // EF Core

    public Product(string name, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));
        if (stock < 0)
            throw new ArgumentException("Stock cannot be negative", nameof(stock));

        Name = name;
        Price = price;
        Stock = stock;

        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price));
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative", nameof(newPrice));

        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductPriceChangedEvent(Id, newPrice));
    }

    public void UpdateStock(int quantity)
    {
        if (Stock + quantity < 0)
            throw new InvalidOperationException($"Insufficient stock. Current: {Stock}");

        Stock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

## Why This Matters

- **Data Integrity**: Invalid state cannot be created
- **Business Logic Centralization**: All rules in one place
- **Auditability**: Events track all changes
- **Testability**: Easy to unit test entity behavior
