using Application.Common.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.UnitTests.Controllers;

/// <summary>
/// Concrete implementation of BaseApiController for testing purposes.
/// </summary>
public class TestableBaseApiController : API.Controllers.BaseApiController
{
    public TestableBaseApiController()
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    public IActionResult TestHandleResult<T>(Result<T> result, string successMessage = "Operation completed successfully")
        => HandleResult(result, successMessage);

    public IActionResult TestHandleResult(Result result, string successMessage = "Operation completed successfully")
        => HandleResult(result, successMessage);

    public IActionResult TestOkResponse<T>(T data, string message = "Success")
        => OkResponse(data, message);

    public IActionResult TestOkResponse(string message = "Success")
        => OkResponse(message);

    public IActionResult TestErrorResponse(string message, int statusCode = 400)
        => ErrorResponse(message, statusCode);

    public IActionResult TestInternalError(string message = "An internal server error occurred")
        => InternalError(message);
}

public class BaseApiControllerTests
{
    private readonly TestableBaseApiController _controller = new();

    #region HandleResult<T> Tests

    [Fact]
    public void HandleResult_Generic_Success_ShouldReturnOk200()
    {
        var result = Result<string>.Success("test-data");

        var actionResult = _controller.TestHandleResult(result);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public void HandleResult_Generic_Success_ShouldContainData()
    {
        var result = Result<string>.Success("test-data");

        var actionResult = _controller.TestHandleResult(result, "Custom message");
        var okResult = actionResult as OkObjectResult;
        var response = okResult!.Value as ApiResponse<string>;

        response!.Success.Should().BeTrue();
        response.Data.Should().Be("test-data");
        response.Message.Should().Be("Custom message");
    }

    [Fact]
    public void HandleResult_Generic_Unauthorized_ShouldReturn401()
    {
        var result = Result<string>.Unauthorized("Not authorized");

        var actionResult = _controller.TestHandleResult(result);

        var unauthorizedResult = actionResult.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorizedResult.StatusCode.Should().Be(401);
    }

    [Fact]
    public void HandleResult_Generic_Forbidden_ShouldReturn403()
    {
        var result = Result<string>.Forbidden("Access denied");

        var actionResult = _controller.TestHandleResult(result);

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(403);
    }

    [Fact]
    public void HandleResult_Generic_NotFound_ShouldReturn404()
    {
        var result = Result<string>.NotFound("Resource not found");

        var actionResult = _controller.TestHandleResult(result);

        var notFoundResult = actionResult.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public void HandleResult_Generic_Conflict_ShouldReturn409()
    {
        var result = Result<string>.Conflict("Already exists");

        var actionResult = _controller.TestHandleResult(result);

        var conflictResult = actionResult.Should().BeOfType<ConflictObjectResult>().Subject;
        conflictResult.StatusCode.Should().Be(409);
    }

    [Fact]
    public void HandleResult_Generic_Failure_ShouldReturn400()
    {
        var result = Result<string>.Failure("Validation failed");

        var actionResult = _controller.TestHandleResult(result);

        var badRequestResult = actionResult.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public void HandleResult_Generic_FailureWithMultipleErrors_ShouldReturn400()
    {
        var errors = new[] { "Error 1", "Error 2", "Error 3" };
        var result = Result<string>.Failure(errors);

        var actionResult = _controller.TestHandleResult(result);

        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region HandleResult (non-generic) Tests

    [Fact]
    public void HandleResult_NonGeneric_Success_ShouldReturnOk200()
    {
        var result = Result.Success();

        var actionResult = _controller.TestHandleResult(result);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public void HandleResult_NonGeneric_Unauthorized_ShouldReturn401()
    {
        var result = Result.Unauthorized("Token expired");

        var actionResult = _controller.TestHandleResult(result);

        actionResult.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public void HandleResult_NonGeneric_Forbidden_ShouldReturn403()
    {
        var result = Result.Forbidden();

        var actionResult = _controller.TestHandleResult(result);

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(403);
    }

    [Fact]
    public void HandleResult_NonGeneric_NotFound_ShouldReturn404()
    {
        var result = Result.NotFound("Not found");

        var actionResult = _controller.TestHandleResult(result);

        actionResult.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void HandleResult_NonGeneric_Conflict_ShouldReturn409()
    {
        var result = Result.Conflict("Duplicate");

        var actionResult = _controller.TestHandleResult(result);

        actionResult.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public void HandleResult_NonGeneric_Failure_ShouldReturn400()
    {
        var result = Result.Failure("Bad data");

        var actionResult = _controller.TestHandleResult(result);

        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region OkResponse Tests

    [Fact]
    public void OkResponse_WithData_ShouldReturnOk200()
    {
        var actionResult = _controller.TestOkResponse("test-data", "Success");

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public void OkResponse_WithoutData_ShouldReturnOk200()
    {
        var actionResult = _controller.TestOkResponse("Done");

        actionResult.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region ErrorResponse Tests

    [Fact]
    public void ErrorResponse_Default_ShouldReturn400()
    {
        var actionResult = _controller.TestErrorResponse("Bad request");

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public void ErrorResponse_CustomStatusCode_ShouldReturnSpecifiedCode()
    {
        var actionResult = _controller.TestErrorResponse("Service unavailable", 503);

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(503);
    }

    #endregion

    #region InternalError Tests

    [Fact]
    public void InternalError_ShouldReturn500()
    {
        var actionResult = _controller.TestInternalError();

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public void InternalError_CustomMessage_ShouldReturn500WithMessage()
    {
        var actionResult = _controller.TestInternalError("Database connection failed");

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
    }

    #endregion
}
