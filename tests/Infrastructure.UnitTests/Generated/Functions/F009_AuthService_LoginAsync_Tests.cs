using System.Text.RegularExpressions;
using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F009_AuthService_LoginAsync_Tests
{
    private static string LoadLoginAsyncBlock()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "Infrastructure", "Identity", "AuthService.cs");
            if (File.Exists(candidate))
            {
                var source = File.ReadAllText(candidate);
                var marker = "public async Task<Result<LoginResponse>> LoginAsync(";
                var start = source.IndexOf(marker, StringComparison.Ordinal);
                Assert.True(start >= 0, "LoginAsync source block not found in AuthService.cs");

                var slice = source[start..];
                var nextMethod = slice.IndexOf("/// <inheritdoc />", marker.Length, StringComparison.Ordinal);
                return nextMethod > 0 ? slice[..nextMethod] : slice;
            }

            dir = dir.Parent;
        }

        throw new FileNotFoundException("Cannot locate src/Infrastructure/Identity/AuthService.cs from test runtime directory.");
    }

    [Fact]
    public void UTC009_MethodSignature_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "9");
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "LoginAsync", null, "9");
        Assert.True(method.ReturnType is not null, "UTC 9: AuthService.LoginAsync must have return type.");
    }

    [Fact]
    public void UTC009_Condition_UserNotFoundOrDeleted_Should_ReturnUnauthorized_InvalidEmailOrPassword()
    {
        var block = LoadLoginAsyncBlock();
        Assert.Contains("if (user == null || user.IsDeleted)", block, StringComparison.Ordinal);
        Assert.Contains("Result<LoginResponse>.Unauthorized(\"Invalid email or password\")", block, StringComparison.Ordinal);
    }

    [Fact]
    public void UTC009_Condition_UserInactive_Should_ReturnUnauthorized_DeactivatedMessage()
    {
        var block = LoadLoginAsyncBlock();
        Assert.Contains("if (!user.IsActive)", block, StringComparison.Ordinal);
        Assert.Contains("Account is deactivated. Please contact support.", block, StringComparison.Ordinal);
    }

    [Fact]
    public void UTC009_Condition_UserLockedOutOrSignInLockedOut_Should_ReturnUnauthorized_LockMessage()
    {
        var block = LoadLoginAsyncBlock();
        Assert.Contains("if (await _userManager.IsLockedOutAsync(user))", block, StringComparison.Ordinal);
        Assert.Contains("if (signInResult.IsLockedOut)", block, StringComparison.Ordinal);
        Assert.Contains("temporarily locked due to multiple failed attempts", block, StringComparison.Ordinal);
    }

    [Fact]
    public void UTC009_Condition_EmailNotConfirmed_Should_ReturnUnauthorized_NotAllowedMessage()
    {
        var block = LoadLoginAsyncBlock();
        Assert.Contains("if (signInResult.IsNotAllowed)", block, StringComparison.Ordinal);
        Assert.Contains("Please confirm your email before logging in.", block, StringComparison.Ordinal);
    }

    [Fact]
    public void UTC009_Condition_OphthalmologistRejected_Should_ReturnUnauthorized_CredentialRejectedMessage()
    {
        var block = LoadLoginAsyncBlock();
        Assert.Contains("VerificationStatus.Rejected", block, StringComparison.Ordinal);
        Assert.Contains("Your credential verification has been rejected.", block, StringComparison.Ordinal);
    }

    [Fact]
    public void UTC009_Condition_TwoFactorRequired_Should_LogInformation_And_ReturnTwoFactorRequired()
    {
        var block = LoadLoginAsyncBlock();
        Assert.Contains("_logger.LogInformation(\"2FA required for user: {Email}\", request.Email);", block, StringComparison.Ordinal);
        Assert.Contains("LoginResponse.TwoFactorRequired(user.Id)", block, StringComparison.Ordinal);
    }

    [Fact]
    public void UTC009_Condition_SuccessfulPasswordLogin_Should_ReturnLoginResponseSuccess()
    {
        var block = LoadLoginAsyncBlock();
        Assert.Contains("var authResponse = await CompleteLoginAsync", block, StringComparison.Ordinal);
        Assert.Contains("return Result<LoginResponse>.Success(LoginResponse.Success(authResponse));", block, StringComparison.Ordinal);
    }

    [Fact]
    public void UTC009_Exception_Should_LogError_And_ReturnFailureMessage()
    {
        var block = LoadLoginAsyncBlock();
        Assert.Matches(new Regex("_logger\\.LogError\\(ex,\\s*\\\"Error during login: \\{Email\\}\\\",\\s*request\\.Email\\);"), block);
        Assert.Contains("return Result<LoginResponse>.Failure(\"An error occurred during login\")", block, StringComparison.Ordinal);
    }
}
