using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F033_IdentityService_GetOrCreateAuthenticatorKeyAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.IdentityService), "GetOrCreateAuthenticatorKeyAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "a2V5ID8/IHRocm93IG5ldyBJbnZhbGlkT3BlcmF0aW9uRXhjZXB0aW9uKCJGYWlsZWQgdG8gZ2VuZXJhdGUgYXV0aGVudGljYXRvciBrZXkiKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC033_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.IdentityService), "33");
    }

    [Fact]
    public void UTC033_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetOrCreateAuthenticatorKeyAsync", null, "33");
    }

    [Fact]
    public void UTC033_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.IdentityService), "GetOrCreateAuthenticatorKeyAsync", isProperty: false, parameterCount: null, utcNo: "33");
    }

    [Fact]
    public void UTC033_Return_Case_Set_Should_Be_Valid()
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
    public void UTC033_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC033_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "33");
    }

    [Fact]
    public void UTC033_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetOrCreateAuthenticatorKeyAsync", null, "33");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "33");
    }
}
