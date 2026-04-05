using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F012_AuthService_LogoutAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "LogoutAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0LlN1Y2Nlc3MoKQ==" };
            yield return new object[] { "UmVzdWx0LkZhaWx1cmUoIkFuIGVycm9yIG9jY3VycmVkIGR1cmluZyBsb2dvdXQiKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "VXNlciBsb2dnZWQgb3V0" };
            yield return new object[] { "RXJyb3IgZHVyaW5nIGxvZ291dA==" };
    }

    [Fact]
    public void UTC012_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "12");
    }

    [Fact]
    public void UTC012_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "LogoutAsync", null, "12");
    }

    [Fact]
    public void UTC012_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "LogoutAsync", isProperty: false, parameterCount: null, utcNo: "12");
    }

    [Fact]
    public void UTC012_Return_Case_Set_Should_Be_Valid()
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
    public void UTC012_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC012_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "12");
    }

    [Fact]
    public void UTC012_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "LogoutAsync", null, "12");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "12");
    }
}
