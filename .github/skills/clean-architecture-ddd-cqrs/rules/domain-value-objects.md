# Domain Value Objects

**Impact: HIGH**

Value objects are immutable, compared by value, and encapsulate validation logic.

## Rule

- Inherit from `ValueObject` base class
- Immutable (no setters or only private setters)
- Equality based on value, not identity
- Validation in constructor
- No side effects

## Examples

### ❌ Incorrect: Using Primitives

```csharp
public class Product
{
    public decimal PriceAmount { get; set; }
    public string PriceCurrency { get; set; }
    
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
}
```

### ✅ Correct: Using Value Objects

**Money Value Object**

```csharp
using Domain.Common;

namespace Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money() { } // EF Core

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("Currency must be 3 characters (ISO 4217)", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}
```

**Address Value Object**

```csharp
using Domain.Common;

namespace Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string Country { get; private set; }

    private Address() { } // EF Core

    public Address(string street, string city, string state, string zipCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        Street = street;
        City = city;
        State = state ?? string.Empty;
        ZipCode = zipCode ?? string.Empty;
        Country = country;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }

    public override string ToString()
    {
        var parts = new List<string> { Street, City };
        if (!string.IsNullOrEmpty(State)) parts.Add(State);
        if (!string.IsNullOrEmpty(ZipCode)) parts.Add(ZipCode);
        parts.Add(Country);
        return string.Join(", ", parts);
    }
}
```

**DateRange Value Object**

```csharp
using Domain.Common;

namespace Domain.ValueObjects;

public class DateRange : ValueObject
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    private DateRange() { }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date must be after start date");

        StartDate = startDate;
        EndDate = endDate;
    }

    public int DurationInDays => (EndDate - StartDate).Days;
    public bool Contains(DateTime date) => date >= StartDate && date <= EndDate;
    public bool Overlaps(DateRange other) => StartDate <= other.EndDate && EndDate >= other.StartDate;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }
}
```

### Base ValueObject Class

```csharp
namespace Domain.Common;

public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}
```

## When to Use Value Objects

| Concept | Value Object? | Reason |
|---------|---------------|--------|
| Price | ✅ Yes | Amount + Currency together |
| Address | ✅ Yes | Multiple fields as single concept |
| Date Range | ✅ Yes | Start + End as single concept |
| Email | ✅ Yes | Validated format |
| Phone | ✅ Yes | Validated format |
| User ID | ❌ No | Use Guid directly |
| Order Number | ✅ Yes | Special format/generation |

## Why This Matters

- **Type Safety**: Compiler prevents mixing types (Money vs plain decimal)
- **Validation**: Always valid by construction
- **Immutability**: No accidental modifications
- **Self-Documenting**: Clear intent in code
