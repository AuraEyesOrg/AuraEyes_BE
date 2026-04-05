using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F013_AuthService_LogoutAllAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "LogoutAllAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0LlN1Y2Nlc3MoKQ==" };
            yield return new object[] { "UmVzdWx0LkZhaWx1cmUoIkFuIGVycm9yIG9jY3VycmVkIGR1cmluZyBsb2dvdXQiKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "QWxsIHRva2VucyByZXZva2VkIGZvciB1c2VyOiB7VXNlcklkfQ==" };
            yield return new object[] { "RXJyb3IgZHVyaW5nIGxvZ291dCBhbGw=" };
    }

    [Fact]
    public void UTC013_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "13");
    }

    [Fact]
    public void UTC013_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "LogoutAllAsync", null, "13");
    }

    [Fact]
    public void UTC013_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "LogoutAllAsync", isProperty: false, parameterCount: null, utcNo: "13");
    }

    [Fact]
    public void UTC013_Return_Case_Set_Should_Be_Valid()
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
    public void UTC013_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC013_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "13");
    }

    [Fact]
    public void UTC013_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "LogoutAllAsync", null, "13");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "13");
    }
}
