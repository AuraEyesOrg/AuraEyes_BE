namespace Application.Common.Models;

/// <summary>
/// Generic result for service operations.
/// Similar to EVCSMS CommandResult/QueryResult pattern.
/// </summary>
public class Result<T>
{
    public T? Data { get; protected set; }
    public bool IsSuccess { get; protected set; }
    public bool IsUnauthorized { get; protected set; }
    public bool IsForbidden { get; protected set; }
    public bool IsNotFound { get; protected set; }
    public bool IsConflict { get; protected set; }
    public bool IsPaymentRequired { get; protected set; }

    public List<string> Errors { get; protected set; } = new();
    public string ErrorMessage => string.Join(", ", Errors);

    protected Result() { }

    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static Result<T> Failure(string message) => new()
    {
        IsSuccess = false,
        Errors = new() { message }
    };

    public static Result<T> Failure(IEnumerable<string> errors) => new()
    {
        IsSuccess = false,
        Errors = errors.ToList()
    };

    public static Result<T> Unauthorized(string message = "Unauthorized") => new()
    {
        IsUnauthorized = true,
        Errors = new() { message }
    };

    public static Result<T> Forbidden(string message = "Forbidden") => new()
    {
        IsForbidden = true,
        Errors = new() { message }
    };

    public static Result<T> NotFound(string message = "Not found") => new()
    {
        IsNotFound = true,
        Errors = new() { message }
    };

    public static Result<T> Conflict(string message = "Conflict") => new()
    {
        IsConflict = true,
        Errors = new() { message }
    };

    public static Result<T> PaymentRequired(string message = "Payment required") => new()
    {
        IsPaymentRequired = true,
        Errors = new() { message }
    };
}

/// <summary>
/// Non-generic result for operations that don't return data.
/// </summary>
public class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsUnauthorized { get; protected set; }
    public bool IsForbidden { get; protected set; }
    public bool IsNotFound { get; protected set; }
    public bool IsConflict { get; protected set; }
    public bool IsPaymentRequired { get; protected set; }

    public List<string> Errors { get; protected set; } = new();
    public string ErrorMessage => string.Join(", ", Errors);

    protected Result() { }

    public static Result Success() => new() { IsSuccess = true };

    public static Result Failure(string message) => new()
    {
        IsSuccess = false,
        Errors = new() { message }
    };

    public static Result Failure(IEnumerable<string> errors) => new()
    {
        IsSuccess = false,
        Errors = errors.ToList()
    };

    public static Result Unauthorized(string message = "Unauthorized") => new()
    {
        IsUnauthorized = true,
        Errors = new() { message }
    };

    public static Result Forbidden(string message = "Forbidden") => new()
    {
        IsForbidden = true,
        Errors = new() { message }
    };

    public static Result NotFound(string message = "Not found") => new()
    {
        IsNotFound = true,
        Errors = new() { message }
    };

    public static Result Conflict(string message = "Conflict") => new()
    {
        IsConflict = true,
        Errors = new() { message }
    };

    public static Result PaymentRequired(string message = "Payment required") => new()
    {
        IsPaymentRequired = true,
        Errors = new() { message }
    };
}
