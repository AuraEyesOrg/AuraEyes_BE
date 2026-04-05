using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F075_DashboardMetricsService_GetPopulationRiskAnalysisAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetPopulationRiskAnalysisAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IFBvcHVsYXRpb25SaXNrQW5hbHlzaXNEdG8geyBUb3RhbFBhdGllbnRzID0gdG90YWxQYXRpZW50cywgUmlza0NhdGVnb3JpZXMgPSBncm91cGVkIC5XaGVyZShpdGVtID0+IGl0ZW0uS2V5ICE9IFJpc2tMZXZlbC5Ob25lKSAuT3JkZXJCeShpdGVtID0+IGl0ZW0uS2V5KSAuU2VsZWN0KGl0ZW0gPT4gbmV3IFJpc2tDYXRlZ29yeUR0byB7IFJpc2tMZXZlbCA9IGl0ZW0uS2V5LlRvU3RyaW5nKCksIENvdW50ID0gaXRlbS5Db3VudCwgUGVyY2VudGFnZSA9IHRvdGFsUmVzdWx0cyA9PSAwID8gMCA6IE1hdGguUm91bmQoKGRlY2ltYWwpaXRlbS5Db3VudCAvIHRvdGFsUmVzdWx0cyAqIDEwMG0sIDEpIH0pIC5Ub0xpc3QoKSB9" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC075_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.DashboardMetricsService), "75");
    }

    [Fact]
    public void UTC075_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetPopulationRiskAnalysisAsync", null, "75");
    }

    [Fact]
    public void UTC075_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetPopulationRiskAnalysisAsync", isProperty: false, parameterCount: null, utcNo: "75");
    }

    [Fact]
    public void UTC075_Return_Case_Set_Should_Be_Valid()
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
    public void UTC075_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC075_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "75");
    }

    [Fact]
    public void UTC075_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetPopulationRiskAnalysisAsync", null, "75");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "75");
    }
}
