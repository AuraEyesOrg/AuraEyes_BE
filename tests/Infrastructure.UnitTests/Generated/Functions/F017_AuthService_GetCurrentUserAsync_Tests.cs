using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F017_AuthService_GetCurrentUserAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "GetCurrentUserAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0PFVzZXJJbmZvUmVzcG9uc2U+LlVuYXV0aG9yaXplZCgiVXNlciBub3QgZm91bmQiKQ==" };
            yield return new object[] { "UmVzdWx0PFVzZXJJbmZvUmVzcG9uc2U+LlN1Y2Nlc3MobmV3IFVzZXJJbmZvUmVzcG9uc2UgeyBJZCA9IHVzZXJEdG8uSWQsIEVtYWlsID0gdXNlckR0by5FbWFpbCwgRnVsbE5hbWUgPSB1c2VyRHRvLkZ1bGxOYW1lLCBBdmF0YXJVcmwgPSB1c2VyRGV0YWlscz8uQXZhdGFyVXJsLCBSb2xlcyA9IHJvbGVzLlRvQXJyYXkoKSwgRW1haWxDb25maXJtZWQgPSB1c2VyRHRvLkVtYWlsQ29uZmlybWVkLCBPcmdhbml6YXRpb25JZCA9IHVzZXJEdG8uT3JnYW5pemF0aW9uSWQsIFJvbGVJZCA9IHJvbGVJZCwgVHdvRmFjdG9yRW5hYmxlZCA9IHR3b0ZhY3RvckVuYWJsZWQsIElzVmVyaWZpZWQgPSBpc1ZlcmlmaWVkLCBWZXJpZmljYXRpb25TdGF0dXMgPSB2ZXJpZmljYXRpb25TdGF0dXMsIENvbnRyYWN0U3RhdHVzID0gY29udHJhY3RTdGF0dXMgfSk=" };
            yield return new object[] { "UmVzdWx0PFVzZXJJbmZvUmVzcG9uc2U+LkZhaWx1cmUoIkFuIGVycm9yIG9jY3VycmVkIHdoaWxlIHJldHJpZXZpbmcgdXNlciBpbmZvcm1hdGlvbiIp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "RXJyb3IgZ2V0dGluZyBjdXJyZW50IHVzZXI6IHtVc2VySWR9" };
    }

    [Fact]
    public void UTC017_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "17");
    }

    [Fact]
    public void UTC017_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "GetCurrentUserAsync", null, "17");
    }

    [Fact]
    public void UTC017_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "GetCurrentUserAsync", isProperty: false, parameterCount: null, utcNo: "17");
    }

    [Fact]
    public void UTC017_Return_Case_Set_Should_Be_Valid()
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
    public void UTC017_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC017_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "17");
    }

    [Fact]
    public void UTC017_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "GetCurrentUserAsync", null, "17");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "17");
    }
}
