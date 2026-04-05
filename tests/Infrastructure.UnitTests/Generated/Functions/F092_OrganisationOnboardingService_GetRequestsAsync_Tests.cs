using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F092_OrganisationOnboardingService_GetRequestsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.OrganisationOnboardingService), "GetRequestsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0PElSZWFkT25seUxpc3Q8T3JnYW5pc2F0aW9uT25ib2FyZGluZ1JlcXVlc3REdG8+Pi5TdWNjZXNzKGl0ZW1zKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC092_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.OrganisationOnboardingService), "92");
    }

    [Fact]
    public void UTC092_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.OrganisationOnboardingService), "GetRequestsAsync", null, "92");
    }

    [Fact]
    public void UTC092_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.OrganisationOnboardingService), "GetRequestsAsync", isProperty: false, parameterCount: null, utcNo: "92");
    }

    [Fact]
    public void UTC092_Return_Case_Set_Should_Be_Valid()
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
    public void UTC092_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC092_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "92");
    }

    [Fact]
    public void UTC092_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.OrganisationOnboardingService), "GetRequestsAsync", null, "92");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "92");
    }
}
