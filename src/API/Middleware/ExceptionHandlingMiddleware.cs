using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;

namespace API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = System.Text.Json.JsonSerializer.Serialize(new ProblemDetails
                {
                    Status = (int)code,
                    Title = "Validation Error",
                    Detail = validationException.Message,
                    Extensions =
                    {
                        ["errors"] = validationException.Errors.Select(e => new
                        {
                            property = e.PropertyName,
                            error = e.ErrorMessage
                        })
                    }
                });
                break;

            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                result = System.Text.Json.JsonSerializer.Serialize(new ProblemDetails
                {
                    Status = (int)code,
                    Title = "Resource Not Found",
                    Detail = exception.Message
                });
                break;

            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                result = System.Text.Json.JsonSerializer.Serialize(new ProblemDetails
                {
                    Status = (int)code,
                    Title = "Unauthorized",
                    Detail = exception.Message
                });
                break;

            case PostgresException postgresException
                when postgresException.SqlState is "XX000" or "53300" &&
                     postgresException.MessageText.Contains("max clients", StringComparison.OrdinalIgnoreCase):
                code = HttpStatusCode.ServiceUnavailable;
                result = System.Text.Json.JsonSerializer.Serialize(new ProblemDetails
                {
                    Status = (int)code,
                    Title = "Database is temporarily busy",
                    Detail = "Database connection pool is exhausted. Please retry in a moment."
                });
                break;

            default:
                result = System.Text.Json.JsonSerializer.Serialize(new ProblemDetails
                {
                    Status = (int)code,
                    Title = "An error occurred",
                    Detail = exception.Message
                });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
