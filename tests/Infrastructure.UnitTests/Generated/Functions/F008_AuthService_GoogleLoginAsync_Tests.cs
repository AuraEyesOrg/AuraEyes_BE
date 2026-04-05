using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F008_AuthService_GoogleLoginAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "GoogleLoginAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0PExvZ2luUmVzcG9uc2U+LlVuYXV0aG9yaXplZCgiSW52YWxpZCBHb29nbGUgdG9rZW4iKQ==" };
            yield return new object[] { "UmVzdWx0PExvZ2luUmVzcG9uc2U+LkZhaWx1cmUoIkdvb2dsZSBhY2NvdW50IGRvZXMgbm90IGhhdmUgYW4gZW1haWwgYWRkcmVzcyIp" };
            yield return new object[] { "UmVzdWx0PExvZ2luUmVzcG9uc2U+LlVuYXV0aG9yaXplZCgiQWNjb3VudCBoYXMgYmVlbiBkZWxldGVkIik=" };
            yield return new object[] { "UmVzdWx0PExvZ2luUmVzcG9uc2U+LlVuYXV0aG9yaXplZCgiQWNjb3VudCBpcyBkZWFjdGl2YXRlZC4gUGxlYXNlIGNvbnRhY3Qgc3VwcG9ydC4iKQ==" };
            yield return new object[] { "UmVzdWx0PExvZ2luUmVzcG9uc2U+LlN1Y2Nlc3MoTG9naW5SZXNwb25zZS5Ud29GYWN0b3JSZXF1aXJlZCh1c2VyLklkKSk=" };
            yield return new object[] { "UmVzdWx0PExvZ2luUmVzcG9uc2U+LlN1Y2Nlc3MoTG9naW5SZXNwb25zZS5TdWNjZXNzKGF1dGhSZXNwb25zZSkp" };
            yield return new object[] { "UmVzdWx0PExvZ2luUmVzcG9uc2U+LkZhaWx1cmUoY3JlYXRlUmVzdWx0LkVycm9ycy5TZWxlY3QoZSA9PiBlLkRlc2NyaXB0aW9uKSk=" };
            yield return new object[] { "UmVzdWx0PExvZ2luUmVzcG9uc2U+LlN1Y2Nlc3MoTG9naW5SZXNwb25zZS5TdWNjZXNzKG5ld0F1dGhSZXNwb25zZSkp" };
            yield return new object[] { "UmVzdWx0PExvZ2luUmVzcG9uc2U+LkZhaWx1cmUoIkFuIGVycm9yIG9jY3VycmVkIGR1cmluZyBHb29nbGUgbG9naW4iKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "MkZBIHJlcXVpcmVkIGZvciBHb29nbGUgdXNlcjoge0VtYWlsfQ==" };
            yield return new object[] { "TmV3IHBhdGllbnQgcmVnaXN0ZXJlZCB2aWEgR29vZ2xlOiB7RW1haWx9" };
            yield return new object[] { "RXJyb3IgZHVyaW5nIEdvb2dsZSBsb2dpbg==" };
    }

    [Fact]
    public void UTC008_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "8");
    }

    [Fact]
    public void UTC008_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "GoogleLoginAsync", null, "8");
    }

    [Fact]
    public void UTC008_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "GoogleLoginAsync", isProperty: false, parameterCount: null, utcNo: "8");
    }

    [Fact]
    public void UTC008_Return_Case_Set_Should_Be_Valid()
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
    public void UTC008_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC008_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "8");
    }

    [Fact]
    public void UTC008_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "GoogleLoginAsync", null, "8");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "8");
    }
}
