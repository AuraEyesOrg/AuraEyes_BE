---
name: clean-architecture-ddd-cqrs
description: ASP.NET Core Clean Architecture with DDD and CQRS patterns. This skill should be used when writing, reviewing, or refactoring .NET backend code to ensure optimal patterns and consistency. Triggers on tasks involving domain entities, commands, queries, repositories, or API controllers.
license: MIT
metadata:
  author: AuraEyes Team
  version: "1.0.0"
---

# Clean Architecture + DDD + CQRS Skills

Comprehensive guide for building ASP.NET Core microservices using Clean Architecture, Domain-Driven Design, and CQRS patterns. Contains rules across 8 categories, prioritized by architectural importance to guide automated refactoring and code generation.

## When to Apply

Reference these guidelines when:
- Writing new domain entities or aggregates
- Implementing commands (Create, Update, Delete operations)
- Implementing queries (Get, List, Search operations)
- Creating or modifying repositories
- Adding new API controllers
- Reviewing code for architectural consistency
- Refactoring existing code to follow DDD patterns

## Technology Stack

| Technology | Purpose | Version |
|------------|---------|---------|
| ASP.NET Core | Web Framework | 8.0 |
| Entity Framework Core | ORM | 8.0 |
| MediatR | CQRS Mediator | Latest |
| FluentValidation | Validation | Latest |
| Serilog | Logging | Latest |
| PostgreSQL | Database | Latest |

## Project Structure

```
src/
├── Domain/           # Core business logic (NO external dependencies)
│   ├── Common/       # BaseEntity, ValueObject, DomainEvent, IRepository
│   ├── Entities/     # Aggregate roots and child entities
│   ├── Events/       # Domain events
│   ├── Repositories/ # Repository interfaces
│   └── ValueObjects/ # Immutable value objects
│
├── Application/      # Business rules and use cases
│   ├── Common/       # Behaviors, Interfaces, Models
│   └── [Feature]/    # Commands, Queries, Handlers, DTOs
│
├── Infrastructure/   # External dependencies
│   ├── Persistence/  # DbContext, Configurations, Repositories
│   └── Services/     # External service implementations
│
└── API/              # Presentation layer
    ├── Controllers/  # API endpoints
    └── Middleware/   # Exception handling, logging
```

## Rule Categories by Priority

| Priority | Category | Impact | Prefix |
|----------|----------|--------|--------|
| 1 | Domain Layer | CRITICAL | `domain-` |
| 2 | CQRS Pattern | CRITICAL | `cqrs-` |
| 3 | Repository Pattern | HIGH | `repo-` |
| 4 | Validation | HIGH | `validation-` |
| 5 | API Layer | MEDIUM-HIGH | `api-` |
| 6 | Infrastructure | MEDIUM | `infra-` |
| 7 | Cross-Cutting | MEDIUM | `cross-` |
| 8 | Testing | LOW-MEDIUM | `test-` |

## Quick Reference

### 1. Domain Layer (CRITICAL)

- `domain-entity-encapsulation` - Private setters, validation in methods
- `domain-aggregate-root` - Implement IAggregateRoot interface
- `domain-value-objects` - Immutable, equality by value
- `domain-events` - Raise events on state changes
- `domain-no-dependencies` - Domain has NO external dependencies

### 2. CQRS Pattern (CRITICAL)

- `cqrs-command-naming` - `[Verb][Entity]Command` (CreateProductCommand)
- `cqrs-query-naming` - `Get[Entity]Query`, `Get[Entities]Query`
- `cqrs-handler-naming` - `[CommandName]Handler`, `[QueryName]Handler`
- `cqrs-result-pattern` - Always return Result<T> from handlers
- `cqrs-separate-models` - Commands write, Queries read only

### 3. Repository Pattern (HIGH)

- `repo-interface-domain` - Interfaces in Domain, implementations in Infrastructure
- `repo-aggregate-only` - One repository per aggregate root
- `repo-unit-of-work` - Use IUnitOfWork for transactions
- `repo-async-methods` - All methods should be async with CancellationToken
- `repo-specific-queries` - Domain-specific query methods

### 4. Validation (HIGH)

- `validation-fluent` - Use FluentValidation for all commands
- `validation-domain` - Business rules in domain entities
- `validation-behavior` - ValidationBehavior in MediatR pipeline
- `validation-separate-concerns` - Input validation vs business validation

### 5. API Layer (MEDIUM-HIGH)

- `api-thin-controllers` - Controllers only dispatch to MediatR
- `api-result-responses` - Map Result<T> to proper HTTP status codes
- `api-documentation` - XML comments and ProducesResponseType attributes
- `api-versioning` - Route versioning pattern
- `api-restful` - Follow RESTful conventions

### 6. Infrastructure (MEDIUM)

- `infra-ef-configurations` - Use IEntityTypeConfiguration
- `infra-dependency-injection` - Register services in DependencyInjection.cs
- `infra-connection-strings` - Use configuration for connection strings
- `infra-migrations` - Use EF Core migrations

### 7. Cross-Cutting Concerns (MEDIUM)

- `cross-exception-middleware` - Global exception handling
- `cross-logging-serilog` - Structured logging with Serilog
- `cross-behaviors` - MediatR behaviors for logging, performance
- `cross-health-checks` - Implement health check endpoints

### 8. Testing (LOW-MEDIUM)

- `test-unit-domain` - Unit test domain logic
- `test-integration-api` - Integration test API endpoints
- `test-mock-repositories` - Mock repositories in unit tests

## File Templates by Feature

When creating a new feature `[Feature]`:

```
Application/[Feature]/
├── Commands/
│   ├── Create[Entity]/
│   │   ├── Create[Entity]Command.cs
│   │   ├── Create[Entity]CommandHandler.cs
│   │   └── Create[Entity]CommandValidator.cs
│   └── Update[Entity]/
│       ├── Update[Entity]Command.cs
│       ├── Update[Entity]CommandHandler.cs
│       └── Update[Entity]CommandValidator.cs
├── Queries/
│   ├── Get[Entity]/
│   │   ├── Get[Entity]Query.cs
│   │   ├── Get[Entity]QueryHandler.cs
│   │   └── [Entity]Dto.cs
│   └── Get[Entities]/
│       ├── Get[Entities]Query.cs
│       ├── Get[Entities]QueryHandler.cs
│       └── [Entity]ListDto.cs

Domain/
├── Entities/[Entity].cs
├── Events/[Entity]Events.cs
└── Repositories/I[Entity]Repository.cs

Infrastructure/Persistence/
├── Configurations/[Entity]Configuration.cs
└── Repositories/[Entity]Repository.cs

API/Controllers/
└── [Entities]Controller.cs
```

## Dependency Flow

```
API → Application → Domain
Infrastructure → Application → Domain

Domain has ZERO external dependencies
Application depends only on Domain
Infrastructure implements Domain interfaces
API orchestrates everything via DI
```
