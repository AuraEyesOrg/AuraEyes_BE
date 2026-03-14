# CQRS Command Naming

**Impact: CRITICAL**

Commands must follow the naming convention `[Verb][Entity]Command`.

## Rule

- Command names: `[Verb][Entity]Command`
- Use action verbs: Create, Update, Delete, Activate, Deactivate, etc.
- Commands are records with `init` properties
- Implement `ICommand<TResponse>` interface

## Examples

### Command Names

| Action | Entity | Command Name |
|--------|--------|--------------|
| Create | Product | `CreateProductCommand` |
| Update | Product | `UpdateProductCommand` |
| Delete | Order | `DeleteOrderCommand` |
| Activate | User | `ActivateUserCommand` |
| Ship | Order | `ShipOrderCommand` |
| Add Review | Product | `AddProductReviewCommand` |

### ❌ Incorrect

```csharp
// Wrong: verb at end
public record ProductCreateCommand { }

// Wrong: missing "Command" suffix
public record CreateProduct { }

// Wrong: not using ICommand interface
public class CreateProductCommand : IRequest<Guid> { }
```

### ✅ Correct

```csharp
using Application.Common.Interfaces;

namespace Application.Products.Commands.CreateProduct;

public record CreateProductCommand : ICommand<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
```

## File Location

```
Application/
└── [Feature]/
    └── Commands/
        └── [Verb][Entity]/
            ├── [Verb][Entity]Command.cs
            ├── [Verb][Entity]CommandHandler.cs
            └── [Verb][Entity]CommandValidator.cs
```

## Why This Matters

- **Consistency**: Easy to find and understand commands
- **Discoverability**: Predictable naming helps code navigation
- **Clarity**: Clear intent from the name
