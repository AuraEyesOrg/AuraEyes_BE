using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F035_IdentityService_GenerateNewRecoveryCodesAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.IdentityService), "GenerateNewRecoveryCodesAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "Y29kZXM/LlRvQXJyYXkoKSA/PyBBcnJheS5FbXB0eTxzdHJpbmc+KCk=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC035_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.IdentityService), "35");
    }

    [Fact]
    public void UTC035_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GenerateNewRecoveryCodesAsync", null, "35");
    }

    [Fact]
    public void UTC035_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.IdentityService), "GenerateNewRecoveryCodesAsync", isProperty: false, parameterCount: null, utcNo: "35");
    }

    [Fact]
    public void UTC035_Return_Case_Set_Should_Be_Valid()
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
    public void UTC035_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC035_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "35");
    }

    [Fact]
    public void UTC035_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GenerateNewRecoveryCodesAsync", null, "35");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "35");
    }
}
