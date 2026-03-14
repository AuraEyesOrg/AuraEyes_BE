# FluentValidation for Commands

**Impact: HIGH**

All commands must have a corresponding FluentValidation validator.

## Rule

- One validator per command: `[CommandName]Validator`
- Use FluentValidation's built-in rules
- Custom validators for complex rules
- Validation runs automatically via ValidationBehavior

## Examples

### Basic Validator

```csharp
using FluentValidation;

namespace Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");
    }
}
```

### Advanced Validator with Dependency

```csharp
using FluentValidation;
using Domain.Repositories;

namespace Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters")
            .MustAsync(BeUniqueName).WithMessage("A product with this name already exists");

        RuleFor(x => x.Sku)
            .Matches(@"^[A-Z]{3}-\d{4}$")
            .When(x => !string.IsNullOrEmpty(x.Sku))
            .WithMessage("SKU must be in format XXX-0000");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _productRepository.ExistsByNameAsync(name, cancellationToken);
    }
}
```

### Nested Object Validation

```csharp
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must have at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductName)
                .NotEmpty().WithMessage("Product name is required");
            
            item.RuleFor(i => i.Price)
                .GreaterThan(0).WithMessage("Price must be positive");
            
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1");
        });
    }
}
```

### Common Validation Patterns

```csharp
// Required string
RuleFor(x => x.Name).NotEmpty();

// Email
RuleFor(x => x.Email).NotEmpty().EmailAddress();

// Phone (regex)
RuleFor(x => x.Phone).Matches(@"^\+?[0-9]{10,15}$");

// Enum
RuleFor(x => x.Status).IsInEnum();

// Date range
RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);

// Conditional
RuleFor(x => x.ShippingAddress)
    .NotEmpty()
    .When(x => x.RequiresShipping);

// Numeric range
RuleFor(x => x.Quantity)
    .InclusiveBetween(1, 1000);

// Collection size
RuleFor(x => x.Tags)
    .Must(tags => tags.Count <= 10)
    .WithMessage("Maximum 10 tags allowed");
```

## Why This Matters

- **Input Validation**: Catches invalid data before processing
- **Separation of Concerns**: Validation logic separate from handler
- **Automatic Execution**: ValidationBehavior runs validators
- **Clear Error Messages**: User-friendly validation errors
