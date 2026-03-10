using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Base API controller with standardized response handling.
/// All controllers should inherit from this class.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Returns appropriate HTTP response based on Result status.
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result, string successMessage = "Operation completed successfully")
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponseFactory.Success(result.Data!, successMessage));
        }

        if (result.IsUnauthorized)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized(result.ErrorMessage));
        }

        if (result.IsForbidden)
        {
            return StatusCode(403, ApiResponseFactory.Forbidden(result.ErrorMessage));
        }

        if (result.IsNotFound)
        {
            return NotFound(ApiResponseFactory.NotFound(result.ErrorMessage));
        }

        if (result.IsConflict)
        {
            return Conflict(ApiResponseFactory.Conflict(result.ErrorMessage));
        }

        if (result.IsPaymentRequired)
        {
            return StatusCode(402, ApiResponseFactory.Error(result.ErrorMessage, result.Errors));
        }

        return BadRequest(ApiResponseFactory.Error(result.ErrorMessage, result.Errors));
    }

    /// <summary>
    /// Returns appropriate HTTP response based on non-generic Result status.
    /// </summary>
    protected IActionResult HandleResult(Result result, string successMessage = "Operation completed successfully")
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponseFactory.Success(successMessage));
        }

        if (result.IsUnauthorized)
        {
            return Unauthorized(ApiResponseFactory.Unauthorized(result.ErrorMessage));
        }

        if (result.IsForbidden)
        {
            return StatusCode(403, ApiResponseFactory.Forbidden(result.ErrorMessage));
        }

        if (result.IsNotFound)
        {
            return NotFound(ApiResponseFactory.NotFound(result.ErrorMessage));
        }

        if (result.IsConflict)
        {
            return Conflict(ApiResponseFactory.Conflict(result.ErrorMessage));
        }

        if (result.IsPaymentRequired)
        {
            return StatusCode(402, ApiResponseFactory.Error(result.ErrorMessage, result.Errors));
        }

        return BadRequest(ApiResponseFactory.Error(result.ErrorMessage, result.Errors));
    }

    /// <summary>
    /// Returns a success response with data.
    /// </summary>
    protected IActionResult OkResponse<T>(T data, string message = "Operation completed successfully")
    {
        return Ok(ApiResponseFactory.Success(data, message));
    }

    /// <summary>
    /// Returns a success response without data.
    /// </summary>
    protected IActionResult OkResponse(string message = "Operation completed successfully")
    {
        return Ok(ApiResponseFactory.Success(message));
    }

    /// <summary>
    /// Returns an error response.
    /// </summary>
    protected IActionResult ErrorResponse(string message, int statusCode = 400)
    {
        return StatusCode(statusCode, ApiResponseFactory.Error(message));
    }

    /// <summary>
    /// Returns an internal server error response.
    /// </summary>
    protected IActionResult InternalError(string message = "An internal server error occurred")
    {
        return StatusCode(500, ApiResponseFactory.InternalServerError(message));
    }
}
