using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F060_AdminQueryService_GetPatientsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.AdminQueryService), "GetPatientsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IFBhZ2VkUmVzdWx0PFBhdGllbnRMaXN0RHRvPiggaXRlbXMsIHRvdGFsQ291bnQsIHBhZ2VOdW1iZXIsIHBhZ2VTaXplKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC060_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.AdminQueryService), "60");
    }

    [Fact]
    public void UTC060_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.AdminQueryService), "GetPatientsAsync", null, "60");
    }

    [Fact]
    public void UTC060_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.AdminQueryService), "GetPatientsAsync", isProperty: false, parameterCount: null, utcNo: "60");
    }

    [Fact]
    public void UTC060_Return_Case_Set_Should_Be_Valid()
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
    public void UTC060_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC060_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "60");
    }

    [Fact]
    public void UTC060_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.AdminQueryService), "GetPatientsAsync", null, "60");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "60");
    }
}
