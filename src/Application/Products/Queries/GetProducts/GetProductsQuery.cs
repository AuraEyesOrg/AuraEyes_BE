using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;

namespace Application.Products.Queries.GetProducts;

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
