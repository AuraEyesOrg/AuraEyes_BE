namespace Domain.Common;

/// <summary>
/// Thrown when a concurrent write conflict is detected during a save operation.
/// Infrastructure wraps DbUpdateConcurrencyException into this type so the
/// Application layer can handle it without depending on EF Core.
/// </summary>
public class ConcurrencyException : Exception
{
    public ConcurrencyException() : base("A concurrency conflict occurred.") { }
    public ConcurrencyException(string message) : base(message) { }
    public ConcurrencyException(string message, Exception inner) : base(message, inner) { }
}
