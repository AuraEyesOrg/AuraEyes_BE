using System.Text.Json.Serialization;

namespace Application.Common.Models;

/// <summary>
/// Standard API response wrapper.
/// All API endpoints should return this format for consistency.
/// </summary>
public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Paginated API response.
/// </summary>
public class PaginatedApiResponse<T> : ApiResponse<IEnumerable<T>>
{
    [JsonPropertyName("pagination")]
    public PaginationInfo Pagination { get; set; } = new();
}

/// <summary>
/// Pagination metadata.
/// </summary>
public class PaginationInfo
{
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("totalCount")]
    public long TotalCount { get; set; }

    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage => CurrentPage < TotalPages;

    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage => CurrentPage > 1;
}

/// <summary>
/// Factory for creating standardized API responses.
/// </summary>
public static class ApiResponseFactory
{
    #region Success Responses

    /// <summary>
    /// Creates a successful response with data.
    /// </summary>
    public static ApiResponse<T> Success<T>(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a successful response without data.
    /// </summary>
    public static ApiResponse<object> Success(string message = "Operation completed successfully")
    {
        return new ApiResponse<object>
        {
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a successful paginated response.
    /// </summary>
    public static PaginatedApiResponse<T> SuccessPaginated<T>(
        IEnumerable<T> data,
        int currentPage,
        int pageSize,
        long totalCount,
        string message = "Data retrieved successfully")
    {
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PaginatedApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow,
            Pagination = new PaginationInfo
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount
            }
        };
    }

    #endregion

    #region Error Responses

    /// <summary>
    /// Creates an error response.
    /// </summary>
    public static ApiResponse<T> Error<T>(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string> { message },
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an error response without generic type.
    /// </summary>
    public static ApiResponse<object> Error(string message, List<string>? errors = null)
    {
        return Error<object>(message, errors);
    }

    /// <summary>
    /// Creates an error response from a Result.
    /// </summary>
    public static ApiResponse<T> FromResult<T>(Result<T> result, string successMessage = "Operation completed successfully")
    {
        if (result.IsSuccess)
        {
            return Success(result.Data!, successMessage);
        }

        return new ApiResponse<T>
        {
            Success = false,
            Message = result.ErrorMessage,
            Errors = result.Errors,
            Timestamp = DateTime.UtcNow
        };
    }

    #endregion

    #region HTTP Status Specific

    public static ApiResponse<object> NotFound(string message = "Resource not found")
        => Error(message);

    public static ApiResponse<object> Unauthorized(string message = "Unauthorized access")
        => Error(message);

    public static ApiResponse<object> Forbidden(string message = "Access forbidden")
        => Error(message);

    public static ApiResponse<object> BadRequest(string message = "Bad request")
        => Error(message);

    public static ApiResponse<object> Conflict(string message = "Resource conflict")
        => Error(message);

    public static ApiResponse<object> InternalServerError(string message = "An internal server error occurred")
        => Error(message);

    #endregion
}
