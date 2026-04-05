using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F042_IdentityService_GetUserDetailsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.IdentityService), "GetUserDetailsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bnVsbA==" };
            yield return new object[] { "bmV3IFVzZXJEZXRhaWxzRHRvIHsgSWQgPSB1c2VyLklkLCBFbWFpbCA9IHVzZXIuRW1haWwhLCBGdWxsTmFtZSA9IHVzZXIuRnVsbE5hbWUsIFBob25lTnVtYmVyID0gdXNlci5QaG9uZU51bWJlciwgRGF0ZU9mQmlydGggPSB1c2VyLkRhdGVPZkJpcnRoLCBHZW5kZXIgPSB1c2VyLkdlbmRlciwgQWRkcmVzcyA9IHVzZXIuQWRkcmVzcywgQXZhdGFyVXJsID0gdXNlci5BdmF0YXJVcmwsIEVtYWlsQ29uZmlybWVkID0gdXNlci5FbWFpbENvbmZpcm1lZCwgQ3JlYXRlZEF0ID0gdXNlci5DcmVhdGVkQXQsIFVwZGF0ZWRBdCA9IHVzZXIuVXBkYXRlZEF0LCB9" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC042_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.IdentityService), "42");
    }

    [Fact]
    public void UTC042_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetUserDetailsAsync", null, "42");
    }

    [Fact]
    public void UTC042_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.IdentityService), "GetUserDetailsAsync", isProperty: false, parameterCount: null, utcNo: "42");
    }

    [Fact]
    public void UTC042_Return_Case_Set_Should_Be_Valid()
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
    public void UTC042_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC042_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "42");
    }

    [Fact]
    public void UTC042_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetUserDetailsAsync", null, "42");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "42");
    }
}
