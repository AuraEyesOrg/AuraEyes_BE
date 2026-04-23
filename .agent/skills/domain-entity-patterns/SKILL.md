# Domain Entity Patterns Skill

## Overview
This skill ensures that all domain entities follow the project's architectural requirements for Clean Architecture and DDD.

## Requirements
All domain entities that are intended to be used with a repository must satisfy the following constraints:
1.  **Inheritance**: Must inherit from `Domain.Common.BaseEntity`.
2.  **Implementation**: Must implement the `Domain.Common.IAggregateRoot` marker interface.

## Rationale
The generic repository interface `IRepository<T>` has a generic constraint:
`where T : BaseEntity, IAggregateRoot`

Failing to implement these will result in compiler errors like:
`The type 'EntityName' cannot be used as type parameter 'T' in the generic type or method 'IRepository<T>'. There is no implicit reference conversion from 'EntityName' to 'Domain.Common.IAggregateRoot'.`

## Example
```csharp
namespace Domain.Entities.Example;

using Domain.Common;

public class MyEntity : BaseEntity, IAggregateRoot
{
    // ...
}
```
