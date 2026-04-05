using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F030_IdentityService_UpdateLastLoginAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.IdentityService), "UpdateLastLoginAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield break;
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC030_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.IdentityService), "30");
    }

    [Fact]
    public void UTC030_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "UpdateLastLoginAsync", null, "30");
    }

    [Fact]
    public void UTC030_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.IdentityService), "UpdateLastLoginAsync", isProperty: false, parameterCount: null, utcNo: "30");
    }

    [Fact]
    public void UTC030_Return_Case_Set_Should_Be_Valid()
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
    public void UTC030_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC030_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "30");
    }

    [Fact]
    public void UTC030_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "UpdateLastLoginAsync", null, "30");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "30");
    }
}
