using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F014_AuthService_ConfirmEmailAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "ConfirmEmailAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0LkZhaWx1cmUoIkludmFsaWQgdXNlciBJRCIp" };
            yield return new object[] { "UmVzdWx0Lk5vdEZvdW5kKCJVc2VyIG5vdCBmb3VuZCIp" };
            yield return new object[] { "UmVzdWx0LkZhaWx1cmUoZXJyb3JzKQ==" };
            yield return new object[] { "UmVzdWx0LlN1Y2Nlc3MoKQ==" };
            yield return new object[] { "UmVzdWx0LkZhaWx1cmUoIkFuIGVycm9yIG9jY3VycmVkIGR1cmluZyBlbWFpbCBjb25maXJtYXRpb24iKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "RW1haWwgY29uZmlybWVkIGZvciB1c2VyOiB7VXNlcklkfQ==" };
            yield return new object[] { "RXJyb3IgY29uZmlybWluZyBlbWFpbCBmb3IgdXNlcjoge1VzZXJJZH0=" };
    }

    [Fact]
    public void UTC014_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "14");
    }

    [Fact]
    public void UTC014_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "ConfirmEmailAsync", null, "14");
    }

    [Fact]
    public void UTC014_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "ConfirmEmailAsync", isProperty: false, parameterCount: null, utcNo: "14");
    }

    [Fact]
    public void UTC014_Return_Case_Set_Should_Be_Valid()
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
    public void UTC014_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC014_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "14");
    }

    [Fact]
    public void UTC014_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "ConfirmEmailAsync", null, "14");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "14");
    }
}
