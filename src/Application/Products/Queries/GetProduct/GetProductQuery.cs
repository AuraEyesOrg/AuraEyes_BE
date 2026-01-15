using Application.Common.Interfaces;

namespace Application.Products.Queries.GetProduct;

public record GetProductQuery(Guid Id) : IQuery<ProductDto>;
