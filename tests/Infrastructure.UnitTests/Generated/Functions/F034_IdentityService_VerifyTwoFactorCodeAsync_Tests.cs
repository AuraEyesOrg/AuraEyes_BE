using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F034_IdentityService_VerifyTwoFactorCodeAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.IdentityService), "VerifyTwoFactorCodeAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "ZmFsc2U=" };
            yield return new object[] { "YXdhaXQgX3VzZXJNYW5hZ2VyLlZlcmlmeVR3b0ZhY3RvclRva2VuQXN5bmMoIHVzZXIsIF91c2VyTWFuYWdlci5PcHRpb25zLlRva2Vucy5BdXRoZW50aWNhdG9yVG9rZW5Qcm92aWRlciwgY29kZSk=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC034_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.IdentityService), "34");
    }

    [Fact]
    public void UTC034_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "VerifyTwoFactorCodeAsync", null, "34");
    }

    [Fact]
    public void UTC034_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.IdentityService), "VerifyTwoFactorCodeAsync", isProperty: false, parameterCount: null, utcNo: "34");
    }

    [Fact]
    public void UTC034_Return_Case_Set_Should_Be_Valid()
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
    public void UTC034_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC034_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "34");
    }

    [Fact]
    public void UTC034_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "VerifyTwoFactorCodeAsync", null, "34");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "34");
    }
}
