using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F052_RefreshTokenService_CleanupExpiredTokensAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.RefreshTokenService), "CleanupExpiredTokensAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "dG9rZW5zVG9EZWxldGUuQ291bnQ=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC052_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.RefreshTokenService), "52");
    }

    [Fact]
    public void UTC052_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.RefreshTokenService), "CleanupExpiredTokensAsync", null, "52");
    }

    [Fact]
    public void UTC052_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.RefreshTokenService), "CleanupExpiredTokensAsync", isProperty: false, parameterCount: null, utcNo: "52");
    }

    [Fact]
    public void UTC052_Return_Case_Set_Should_Be_Valid()
    {
        var source = LoadSource();
        foreach (var c in ReturnCases())
        {
            var expected = FunctionAssertionHelper.DecodeCase((string)c[0]);
            Assert.False(string.IsNullOrWhiteSpace(expected));
            FunctionAssertionHelper.AssertCaseSnippetMatch(source, expected);
        }
    }

    [Fact]
    public void UTC052_Log_Message_Case_Set_Should_Be_Valid()
    {
        var source = LoadSource();
        foreach (var c in LogCases())
        {
            var expected = FunctionAssertionHelper.DecodeCase((string)c[0]);
            Assert.False(string.IsNullOrWhiteSpace(expected));
            FunctionAssertionHelper.AssertCaseSnippetMatch(source, expected);
        }
    }

    [Fact]
    public void UTC052_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "52");
    }

    [Fact]
    public void UTC052_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.RefreshTokenService), "CleanupExpiredTokensAsync", null, "52");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "52");
    }
}
