using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F039_IdentityService_GetUserMetricsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.IdentityService), "GetUserMetricsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IFVzZXJNZXRyaWNzRHRvKCB0b3RhbFVzZXJzLCBNYXRoLlJvdW5kKHRvdGFsVXNlcnNDaGFuZ2UsIDEpLCBhY3RpdmVEb2N0b3JzLCAwLCAvLyBXb3VsZCBuZWVkIGhpc3RvcmljYWwgZGF0YSBmb3IgY2hhbmdlIDAsIC8vIFdvdWxkIG5lZWQgc2NyZWVuaW5nIGRhdGEgLSBwYXNzZWQgc2VwYXJhdGVseSAwLCAvLyBXb3VsZCBuZWVkIHNjcmVlbmluZyBkYXRhIHBlbmRpbmdBcHByb3ZhbHMgKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC039_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.IdentityService), "39");
    }

    [Fact]
    public void UTC039_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetUserMetricsAsync", null, "39");
    }

    [Fact]
    public void UTC039_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.IdentityService), "GetUserMetricsAsync", isProperty: false, parameterCount: null, utcNo: "39");
    }

    [Fact]
    public void UTC039_Return_Case_Set_Should_Be_Valid()
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
    public void UTC039_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC039_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "39");
    }

    [Fact]
    public void UTC039_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetUserMetricsAsync", null, "39");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "39");
    }
}
