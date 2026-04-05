using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F037_IdentityService_GenerateAuthenticatorUri_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.IdentityService), "GenerateAuthenticatorUri", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "JCJvdHBhdXRoOi8vdG90cC97VXJsRW5jb2Rlci5EZWZhdWx0LkVuY29kZShBdXRoZW50aWNhdG9ySXNzdWVyKX06e1VybEVuY29kZXIuRGVmYXVsdC5FbmNvZGUoZW1haWwpfSIgKyAkIj9zZWNyZXQ9e3NoYXJlZEtleX0iICsgJCImaXNzdWVyPXtVcmxFbmNvZGVyLkRlZmF1bHQuRW5jb2RlKEF1dGhlbnRpY2F0b3JJc3N1ZXIpfSIgKyAiJmRpZ2l0cz02Ig==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC037_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.IdentityService), "37");
    }

    [Fact]
    public void UTC037_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GenerateAuthenticatorUri", null, "37");
    }

    [Fact]
    public void UTC037_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.IdentityService), "GenerateAuthenticatorUri", isProperty: false, parameterCount: null, utcNo: "37");
    }

    [Fact]
    public void UTC037_Return_Case_Set_Should_Be_Valid()
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
    public void UTC037_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC037_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "37");
    }

    [Fact]
    public void UTC037_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GenerateAuthenticatorUri", null, "37");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "37");
    }
}
