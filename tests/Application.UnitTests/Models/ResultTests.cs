using Application.Common.Models;
using FluentAssertions;

namespace Application.UnitTests.Models;

public class ResultTests
{
    #region Generic Result<T>

    [Fact]
    public void Success_ShouldCreateSuccessResult()
    {
        var result = Result<string>.Success("test data");

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be("test data");
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithMessage_ShouldCreateFailureResult()
    {
        var result = Result<string>.Failure("something failed");

        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Errors.Should().ContainSingle("something failed");
        result.ErrorMessage.Should().Be("something failed");
    }

    [Fact]
    public void Failure_WithMultipleErrors_ShouldJoinErrors()
    {
        var result = Result<string>.Failure(new[] { "error1", "error2" });

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.ErrorMessage.Should().Be("error1, error2");
    }

    [Fact]
    public void Unauthorized_ShouldSetFlag()
    {
        var result = Result<string>.Unauthorized();

        result.IsUnauthorized.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Unauthorized");
    }

    [Fact]
    public void Forbidden_ShouldSetFlag()
    {
        var result = Result<string>.Forbidden();

        result.IsForbidden.Should().BeTrue();
        result.ErrorMessage.Should().Be("Forbidden");
    }

    [Fact]
    public void NotFound_ShouldSetFlag()
    {
        var result = Result<string>.NotFound("User not found");

        result.IsNotFound.Should().BeTrue();
        result.ErrorMessage.Should().Be("User not found");
    }

    [Fact]
    public void Conflict_ShouldSetFlag()
    {
        var result = Result<string>.Conflict("Already exists");

        result.IsConflict.Should().BeTrue();
        result.ErrorMessage.Should().Be("Already exists");
    }

    #endregion

    #region Non-Generic Result

    [Fact]
    public void NonGeneric_Success_ShouldBeSuccessful()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void NonGeneric_Failure_ShouldNotBeSuccessful()
    {
        var result = Result.Failure("failed");

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("failed");
    }

    [Fact]
    public void NonGeneric_Unauthorized_ShouldSetFlag()
    {
        var result = Result.Unauthorized();

        result.IsUnauthorized.Should().BeTrue();
    }

    [Fact]
    public void NonGeneric_Forbidden_ShouldSetFlag()
    {
        var result = Result.Forbidden();

        result.IsForbidden.Should().BeTrue();
    }

    [Fact]
    public void NonGeneric_NotFound_ShouldSetFlag()
    {
        var result = Result.NotFound();

        result.IsNotFound.Should().BeTrue();
    }

    [Fact]
    public void NonGeneric_Conflict_ShouldSetFlag()
    {
        var result = Result.Conflict();

        result.IsConflict.Should().BeTrue();
    }

    #endregion
}
