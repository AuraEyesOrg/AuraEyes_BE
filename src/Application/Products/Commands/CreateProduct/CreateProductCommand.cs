using Application.Common.Interfaces;

namespace Application.Products.Commands.CreateProduct;

public record CreateProductCommand : ICommand<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
