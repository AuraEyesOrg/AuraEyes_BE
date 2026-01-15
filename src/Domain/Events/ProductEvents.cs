using Domain.Common;
using Domain.Entities;

namespace Domain.Events;

/// <summary>
/// Domain events for Product aggregate
/// Used to communicate state changes within the domain
/// </summary>

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
    public int NewStock { get; }
    public int QuantityChanged { get; }

    public ProductStockChangedEvent(Guid productId, int newStock, int quantityChanged)
    {
        ProductId = productId;
        NewStock = newStock;
        QuantityChanged = quantityChanged;
    }
}

public class ProductStatusChangedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public ProductStatus NewStatus { get; }

    public ProductStatusChangedEvent(Guid productId, ProductStatus newStatus)
    {
        ProductId = productId;
        NewStatus = newStatus;
    }
}

public class ProductReviewAddedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public int Rating { get; }

    public ProductReviewAddedEvent(Guid productId, int rating)
    {
        ProductId = productId;
        Rating = rating;
    }
}
