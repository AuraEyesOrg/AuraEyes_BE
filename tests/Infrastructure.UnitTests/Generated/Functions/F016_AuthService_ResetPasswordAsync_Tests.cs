using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F016_AuthService_ResetPasswordAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "ResetPasswordAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0LkZhaWx1cmUoIkludmFsaWQgdXNlciBJRCIp" };
            yield return new object[] { "UmVzdWx0Lk5vdEZvdW5kKCJVc2VyIG5vdCBmb3VuZCIp" };
            yield return new object[] { "UmVzdWx0LkZhaWx1cmUoZXJyb3JzKQ==" };
            yield return new object[] { "UmVzdWx0LlN1Y2Nlc3MoKQ==" };
            yield return new object[] { "UmVzdWx0LkZhaWx1cmUoIkFuIGVycm9yIG9jY3VycmVkIHdoaWxlIHJlc2V0dGluZyBwYXNzd29yZCIp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "UGFzc3dvcmQgcmVzZXQgZm9yIHVzZXI6IHtVc2VySWR9" };
            yield return new object[] { "RXJyb3IgcmVzZXR0aW5nIHBhc3N3b3JkIGZvciB1c2VyOiB7VXNlcklkfQ==" };
    }

    [Fact]
    public void UTC016_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "16");
    }

    [Fact]
    public void UTC016_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "ResetPasswordAsync", null, "16");
    }

    [Fact]
    public void UTC016_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "ResetPasswordAsync", isProperty: false, parameterCount: null, utcNo: "16");
    }

    [Fact]
    public void UTC016_Return_Case_Set_Should_Be_Valid()
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
    public void UTC016_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC016_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "16");
    }

    [Fact]
    public void UTC016_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "ResetPasswordAsync", null, "16");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "16");
    }
}
