using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F071_DailyQuotaResetJob_ExecuteAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.DailyQuotaResetJob), "ExecuteAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield break;
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "U3RhcnRpbmcgZGFpbHkgQUkgcXVvdGEgcmVzZXQgam9iIGF0IHtUaW1lfSBVVEM=" };
            yield return new object[] { "RmFpbGVkIHRvIGV4ZWN1dGUgZGFpbHkgcXVvdGEgcmVzZXQgam9i" };
    }

    [Fact]
    public void UTC071_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.DailyQuotaResetJob), "71");
    }

    [Fact]
    public void UTC071_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DailyQuotaResetJob), "ExecuteAsync", null, "71");
    }

    [Fact]
    public void UTC071_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.DailyQuotaResetJob), "ExecuteAsync", isProperty: false, parameterCount: null, utcNo: "71");
    }

    [Fact]
    public void UTC071_Return_Case_Set_Should_Be_Valid()
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
    public void UTC071_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC071_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "71");
    }

    [Fact]
    public void UTC071_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DailyQuotaResetJob), "ExecuteAsync", null, "71");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "71");
    }
}
