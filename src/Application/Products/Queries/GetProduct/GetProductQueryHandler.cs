using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.Products.Queries.GetProduct;

public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductWithReviewsAsync(request.Id, cancellationToken);

        if (product == null)
        {
            return Result<ProductDto>.Failure($"Product with ID {request.Id} not found");
        }

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku,
            Price = product.Price,
            Stock = product.Stock,
            Status = product.Status.ToString(),
            Category = product.Category,
            LastRestockedAt = product.LastRestockedAt,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Reviews = product.Reviews.Select(r => new ProductReviewDto
            {
                Id = r.Id,
                ReviewerName = r.ReviewerName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList()
        };

        return Result<ProductDto>.Success(productDto);
    }
}
