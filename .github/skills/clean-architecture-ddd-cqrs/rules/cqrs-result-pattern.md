# CQRS Result Pattern

**Impact: HIGH**

All handlers must return `Result<T>` for type-safe error handling.

## Rule

- Commands return `Result<T>` where T is the response type
- Queries return `Result<TDto>` where TDto is the data transfer object
- Use `Result.Success(data)` for success
- Use `Result.Failure(message)` for failure
- Never throw exceptions for expected business errors

## Examples

### Result Class

```csharp
namespace Application.Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }
    public List<string> Errors { get; }

    private Result(bool isSuccess, T? data, string? error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(string error) => new(false, default, error);
    public static Result<T> Failure(List<string> errors) => new(false, default, null, errors);
}
```

### ❌ Incorrect

```csharp
// Wrong: throwing exception for business error
public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
{
    var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
    
    if (product == null)
        throw new NotFoundException($"Product {request.Id} not found");
    
    return MapToDto(product);
}
```

### ✅ Correct

```csharp
public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
{
    var product = await _repository.GetByIdAsync(request.Id, cancellationToken);

    if (product == null)
        return Result<ProductDto>.Failure($"Product with ID {request.Id} not found");

    var dto = new ProductDto
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price
    };

    return Result<ProductDto>.Success(dto);
}
```

### Controller Usage

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<Result<ProductDto>>> GetProduct(Guid id)
{
    var result = await _mediator.Send(new GetProductQuery(id));

    if (!result.IsSuccess)
        return NotFound(result);

    return Ok(result);
}
```

## Why This Matters

- **Type Safety**: Compiler ensures error handling
- **Consistent API**: All endpoints return same format
- **No Exceptions for Control Flow**: Better performance
- **Clear Error Messages**: Structured error responses
