using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F053_TokenService_GenerateAccessTokenAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.TokenService), "GenerateAccessTokenAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "VGFzay5Gcm9tUmVzdWx0KG5ldyBUb2tlblJlc3VsdChhY2Nlc3NUb2tlbiwganRpLCBleHBpcmVzQXQpKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC053_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.TokenService), "53");
    }

    [Fact]
    public void UTC053_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.TokenService), "GenerateAccessTokenAsync", null, "53");
    }

    [Fact]
    public void UTC053_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.TokenService), "GenerateAccessTokenAsync", isProperty: false, parameterCount: null, utcNo: "53");
    }

    [Fact]
    public void UTC053_Return_Case_Set_Should_Be_Valid()
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
    public void UTC053_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC053_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "53");
    }

    [Fact]
    public void UTC053_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.TokenService), "GenerateAccessTokenAsync", null, "53");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "53");
    }
}
