using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F057_TokenService_GetJtiFromToken_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.TokenService), "GetJtiFromToken", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "cHJpbmNpcGFsPy5GaW5kRmlyc3QoSnd0UmVnaXN0ZXJlZENsYWltTmFtZXMuSnRpKT8uVmFsdWU=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC057_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.TokenService), "57");
    }

    [Fact]
    public void UTC057_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.TokenService), "GetJtiFromToken", null, "57");
    }

    [Fact]
    public void UTC057_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.TokenService), "GetJtiFromToken", isProperty: false, parameterCount: null, utcNo: "57");
    }

    [Fact]
    public void UTC057_Return_Case_Set_Should_Be_Valid()
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
    public void UTC057_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC057_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "57");
    }

    [Fact]
    public void UTC057_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.TokenService), "GetJtiFromToken", null, "57");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "57");
    }
}
