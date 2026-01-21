# Thin Controllers

**Impact: MEDIUM-HIGH**

Controllers must be thin, only dispatching to MediatR. No business logic in controllers.

## Rule

- Controllers only create command/query and dispatch to MediatR
- No business logic in controllers
- Map Result<T> to appropriate HTTP status codes
- Use proper HTTP verbs and status codes
- Document with XML comments and ProducesResponseType

## Examples

### ❌ Incorrect: Fat Controller

```csharp
[HttpPost]
public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
{
    // Business logic in controller - WRONG!
    if (string.IsNullOrEmpty(request.Name))
        return BadRequest("Name is required");

    if (request.Price <= 0)
        return BadRequest("Price must be positive");

    var product = new Product
    {
        Name = request.Name,
        Price = request.Price,
        Stock = request.Stock
    };

    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    return Ok(product);
}
```

### ✅ Correct: Thin Controller

```csharp
using Application.Common.Models;
using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetProduct;
using Application.Products.Queries.GetProducts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Products management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get products with pagination and filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="status">Filter by status</param>
    /// <param name="searchTerm">Search in name/description</param>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<ProductListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<PagedResult<ProductListDto>>>> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] ProductStatus? status = null,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetProductsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Status = status,
            SearchTerm = searchTerm
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ProductDto>>> GetProduct(Guid id)
    {
        var result = await _mediator.Send(new GetProductQuery(id));

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="command">Create product command</param>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<Guid>>> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetProduct), new { id = result.Data }, result);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="command">Update product command</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest(Result.Failure("Route ID does not match command ID"));

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));

        if (!result.IsSuccess)
            return NotFound(result);

        return NoContent();
    }
}
```

## HTTP Status Codes

| Result | HTTP Status |
|--------|-------------|
| Success (GET) | 200 OK |
| Success (POST) | 201 Created |
| Success (PUT) | 200 OK |
| Success (DELETE) | 204 No Content |
| Validation Error | 400 Bad Request |
| Not Found | 404 Not Found |
| Business Error | 400 Bad Request or 409 Conflict |

## Why This Matters

- **Single Responsibility**: Controllers only handle HTTP
- **Testability**: Business logic tested via handlers
- **Consistency**: All endpoints follow same pattern
- **Documentation**: Clear API documentation
