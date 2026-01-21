# Entity Framework Configuration

**Impact: MEDIUM**

Use IEntityTypeConfiguration for all entity configurations. Keep configurations separate from DbContext.

## Rule

- One configuration class per entity
- Implement `IEntityTypeConfiguration<TEntity>`
- Located in `Infrastructure/Persistence/Configurations/`
- Apply configurations in DbContext.OnModelCreating
- Ignore DomainEvents property

## Examples

### Entity Configuration

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table name
        builder.ToTable("Products");

        // Primary key
        builder.HasKey(p => p.Id);

        // String properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Sku)
            .HasMaxLength(50);

        builder.Property(p => p.Category)
            .HasMaxLength(100);

        // Decimal properties
        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // Integer properties
        builder.Property(p => p.Stock)
            .IsRequired();

        // Enum as string
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // DateTime properties
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.LastRestockedAt);

        // Relationships
        builder.HasMany(p => p.Reviews)
            .WithOne()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events (not persisted)
        builder.Ignore(p => p.DomainEvents);

        // Indexes
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.Sku).IsUnique().HasFilter("[Sku] IS NOT NULL");
    }
}
```

### Child Entity Configuration

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReviewerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(2000);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.Rating);
    }
}
```

### Value Object Configuration

```csharp
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        // Value object as owned type
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("ShippingStreet")
                .HasMaxLength(200);

            address.Property(a => a.City)
                .HasColumnName("ShippingCity")
                .HasMaxLength(100);

            address.Property(a => a.ZipCode)
                .HasColumnName("ShippingZipCode")
                .HasMaxLength(20);

            address.Property(a => a.Country)
                .HasColumnName("ShippingCountry")
                .HasMaxLength(100);
        });
    }
}
```

### DbContext Configuration

```csharp
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
}
```

## Common Patterns

```csharp
// Required field
.IsRequired()

// Max length
.HasMaxLength(200)

// Decimal precision
.HasColumnType("decimal(18,2)")

// Enum as string
.HasConversion<string>()

// Unique constraint
.HasIndex(x => x.Email).IsUnique()

// Conditional unique
.HasIndex(x => x.Sku).IsUnique().HasFilter("[Sku] IS NOT NULL")

// Cascade delete
.OnDelete(DeleteBehavior.Cascade)

// Restrict delete
.OnDelete(DeleteBehavior.Restrict)

// Ignore property
.Ignore(x => x.DomainEvents)
```

## Why This Matters

- **Separation of Concerns**: Configuration separate from entity
- **Clean DbContext**: Only DbSets in context
- **Automatic Discovery**: ApplyConfigurationsFromAssembly finds all configs
- **Type Safety**: Strongly typed configuration
