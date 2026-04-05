using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F047_RefreshTokenService_GetByTokenHashAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.RefreshTokenService), "GetByTokenHashAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bnVsbA==" };
            yield return new object[] { "bmV3IFJlZnJlc2hUb2tlbkR0byggdG9rZW4uSWQsIHRva2VuLlVzZXJJZCwgdG9rZW4uSnd0SWQsIHRva2VuLkNyZWF0ZWRBdCwgdG9rZW4uRXhwaXJlc0F0LCB0b2tlbi5Vc2VkQXQsIHRva2VuLlJldm9rZWRBdCwgdG9rZW4uSXNBY3RpdmUgKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC047_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.RefreshTokenService), "47");
    }

    [Fact]
    public void UTC047_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.RefreshTokenService), "GetByTokenHashAsync", null, "47");
    }

    [Fact]
    public void UTC047_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.RefreshTokenService), "GetByTokenHashAsync", isProperty: false, parameterCount: null, utcNo: "47");
    }

    [Fact]
    public void UTC047_Return_Case_Set_Should_Be_Valid()
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
    public void UTC047_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC047_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "47");
    }

    [Fact]
    public void UTC047_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.RefreshTokenService), "GetByTokenHashAsync", null, "47");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "47");
    }
}
