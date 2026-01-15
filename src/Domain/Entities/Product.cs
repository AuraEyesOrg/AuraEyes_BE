using Domain.Common;
using Domain.Events;

namespace Domain.Entities;

/// <summary>
/// Product aggregate root demonstrating DDD patterns
/// Encapsulates business logic for product management
/// </summary>
public class Product : BaseEntity, IAggregateRoot
{
    // Properties with private setters for encapsulation
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Sku { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public ProductStatus Status { get; private set; }
    public string? Category { get; private set; }
    public DateTime? LastRestockedAt { get; private set; }

    // Navigation properties - child entities managed by aggregate
    private readonly List<ProductReview> _reviews = new();
    public IReadOnlyCollection<ProductReview> Reviews => _reviews.AsReadOnly();

    // Parameterless constructor required by EF Core
    private Product() 
    { 
        Name = string.Empty;
        Description = string.Empty;
    }

    // Constructor with validation - creates valid aggregate
    public Product(string name, string description, decimal price, int stock, string? sku = null, string? category = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        if (stock < 0)
            throw new ArgumentException("Stock cannot be negative", nameof(stock));

        Name = name;
        Description = description ?? string.Empty;
        Price = price;
        Stock = stock;
        Sku = sku;
        Category = category;
        Status = ProductStatus.Active;

        // Raise domain event for product creation
        AddDomainEvent(new ProductCreatedEvent(Id, Name, Price));
    }

    // Business method to update product details
    public void UpdateDetails(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        Name = name;
        Description = description;
        Price = price;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductUpdatedEvent(Id, Name));
    }

    // Business method to update category
    public void UpdateCategory(string? category)
    {
        Category = category;
        UpdatedAt = DateTime.UtcNow;
    }

    // Business method to update SKU
    public void UpdateSku(string? sku)
    {
        Sku = sku;
        UpdatedAt = DateTime.UtcNow;
    }

    // Business method to manage stock with validation
    public void UpdateStock(int quantity)
    {
        if (Stock + quantity < 0)
            throw new InvalidOperationException($"Insufficient stock. Current stock: {Stock}, requested change: {quantity}");

        Stock += quantity;
        
        if (quantity > 0)
        {
            LastRestockedAt = DateTime.UtcNow;
        }
        
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductStockChangedEvent(Id, Stock, quantity));
    }

    // Business method to set stock directly (e.g., for inventory adjustments)
    public void SetStock(int newStock)
    {
        if (newStock < 0)
            throw new ArgumentException("Stock cannot be negative", nameof(newStock));

        var change = newStock - Stock;
        Stock = newStock;
        
        if (newStock > Stock)
        {
            LastRestockedAt = DateTime.UtcNow;
        }
        
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductStockChangedEvent(Id, Stock, change));
    }

    // Business method to deactivate product
    public void Deactivate()
    {
        if (Status == ProductStatus.Discontinued)
            throw new InvalidOperationException("Cannot deactivate a discontinued product");

        Status = ProductStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductStatusChangedEvent(Id, Status));
    }

    // Business method to activate product
    public void Activate()
    {
        if (Status == ProductStatus.Discontinued)
            throw new InvalidOperationException("Cannot activate a discontinued product");

        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductStatusChangedEvent(Id, Status));
    }

    // Business method to discontinue product (permanent)
    public void Discontinue()
    {
        Status = ProductStatus.Discontinued;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductStatusChangedEvent(Id, Status));
    }

    // Business method to add review (manages child entity)
    public void AddReview(string reviewerName, int rating, string comment)
    {
        var review = new ProductReview(Id, reviewerName, rating, comment);
        _reviews.Add(review);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ProductReviewAddedEvent(Id, rating));
    }

    // Helper method to check if product is in stock
    public bool IsInStock() => Stock > 0;

    // Helper method to check if product is low on stock
    public bool IsLowStock(int threshold = 10) => Stock > 0 && Stock <= threshold;
}

public enum ProductStatus
{
    Active = 1,
    Inactive = 2,
    Discontinued = 3
}

/// <summary>
/// ProductReview entity (not an aggregate root)
/// Managed by Product aggregate
/// </summary>
public class ProductReview : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ReviewerName { get; private set; } = string.Empty;
    public int Rating { get; private set; }
    public string Comment { get; private set; } = string.Empty;

    // Parameterless constructor for EF Core
    private ProductReview() 
    {
        ReviewerName = string.Empty;
        Comment = string.Empty;
    }

    // Constructor with validation
    public ProductReview(Guid productId, string reviewerName, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        if (string.IsNullOrWhiteSpace(reviewerName))
            throw new ArgumentException("Reviewer name cannot be empty", nameof(reviewerName));

        ProductId = productId;
        ReviewerName = reviewerName;
        Rating = rating;
        Comment = comment ?? string.Empty;
    }
}
