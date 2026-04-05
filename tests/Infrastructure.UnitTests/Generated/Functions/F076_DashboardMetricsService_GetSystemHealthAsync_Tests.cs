using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F076_DashboardMetricsService_GetSystemHealthAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetSystemHealthAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "VGFzay5Gcm9tUmVzdWx0KG5ldyBTeXN0ZW1IZWFsdGhEdG8geyBBbGxTeXN0ZW1zT3BlcmF0aW9uYWwgPSB0cnVlLCBDb21wb25lbnRzID0gbmV3IExpc3Q8Q29tcG9uZW50SGVhbHRoRHRvPiB7IG5ldygpIHsgQ29tcG9uZW50TmFtZSA9ICJEYXRhYmFzZSIsIFN0YXR1cyA9ICJDb25uZWN0ZWQiLCBJc0hlYWx0aHkgPSB0cnVlLCBMYXRlbmN5TXMgPSAyNSwgVXB0aW1lUGVyY2VudGFnZSA9IDEwMCwgTGFzdENoZWNrZWRBdCA9IERhdGVUaW1lLlV0Y05vdyB9LCBuZXcoKSB7IENvbXBvbmVudE5hbWUgPSAiQUkgU2VydmljZSIsIFN0YXR1cyA9ICJPbmxpbmUiLCBJc0hlYWx0aHkgPSB0cnVlLCBMYXRlbmN5TXMgPSAxMjAsIFVwdGltZVBlcmNlbnRhZ2UgPSAxMDAsIExhc3RDaGVja2VkQXQgPSBEYXRlVGltZS5VdGNOb3cgfSwgbmV3KCkgeyBDb21wb25lbnROYW1lID0gIk5vdGlmaWNhdGlvbnMiLCBTdGF0dXMgPSAiT3BlcmF0aW9uYWwiLCBJc0hlYWx0aHkgPSB0cnVlLCBMYXRlbmN5TXMgPSA0MCwgVXB0aW1lUGVyY2VudGFnZSA9IDEwMCwgTGFzdENoZWNrZWRBdCA9IERhdGVUaW1lLlV0Y05vdyB9IH0gfSk=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC076_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.DashboardMetricsService), "76");
    }

    [Fact]
    public void UTC076_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetSystemHealthAsync", null, "76");
    }

    [Fact]
    public void UTC076_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetSystemHealthAsync", isProperty: false, parameterCount: null, utcNo: "76");
    }

    [Fact]
    public void UTC076_Return_Case_Set_Should_Be_Valid()
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
    public void UTC076_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC076_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "76");
    }

    [Fact]
    public void UTC076_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetSystemHealthAsync", null, "76");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "76");
    }
}
