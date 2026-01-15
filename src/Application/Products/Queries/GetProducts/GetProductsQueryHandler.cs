using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedResult<ProductListDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<PagedResult<ProductListDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var allProducts = await _productRepository.GetAllAsync(cancellationToken);

        // Apply filters
        var filteredProducts = allProducts.AsEnumerable();

        if (request.Status.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            filteredProducts = filteredProducts.Where(p => 
                p.Name.ToLower().Contains(searchLower) || 
                (p.Description != null && p.Description.ToLower().Contains(searchLower)) ||
                (p.Sku != null && p.Sku.ToLower().Contains(searchLower)));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            filteredProducts = filteredProducts.Where(p => 
                p.Category != null && p.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase));
        }

        if (request.MinPrice.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            filteredProducts = filteredProducts.Where(p => p.Price <= request.MaxPrice.Value);
        }

        var totalCount = filteredProducts.Count();

        // Apply pagination
        var productDtos = filteredProducts
            .OrderBy(p => p.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Price = p.Price,
                Stock = p.Stock,
                Status = p.Status.ToString(),
                Category = p.Category,
                CreatedAt = p.CreatedAt,
                IsLowStock = p.IsLowStock()
            })
            .ToList();

        var pagedResult = new PagedResult<ProductListDto>(
            productDtos,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return Result<PagedResult<ProductListDto>>.Success(pagedResult);
    }
}
