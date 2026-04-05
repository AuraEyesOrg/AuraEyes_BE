using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F077_DashboardMetricsService_GetOphthalmologistMetricsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetOphthalmologistMetricsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IE9waHRoYWxtb2xvZ2lzdERhc2hib2FyZE1ldHJpY3NEdG8oKQ==" };
            yield return new object[] { "bmV3IE9waHRoYWxtb2xvZ2lzdERhc2hib2FyZE1ldHJpY3NEdG8geyBQZW5kaW5nUmV2aWV3cyA9IHBlbmRpbmdSZXZpZXdzLCBVcmdlbnRDYXNlcyA9IHVyZ2VudENhc2VzLCBDb21wbGV0ZWRUb2RheSA9IGNvbXBsZXRlZFRvZGF5LCBPcGVuU2xvdHNUb2RheSA9IG9wZW5TbG90c1RvZGF5LCBVcmdlbnRDYXNlTGlzdCA9IHVyZ2VudENhc2VMaXN0IH0=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC077_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.DashboardMetricsService), "77");
    }

    [Fact]
    public void UTC077_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetOphthalmologistMetricsAsync", null, "77");
    }

    [Fact]
    public void UTC077_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetOphthalmologistMetricsAsync", isProperty: false, parameterCount: null, utcNo: "77");
    }

    [Fact]
    public void UTC077_Return_Case_Set_Should_Be_Valid()
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
    public void UTC077_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC077_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "77");
    }

    [Fact]
    public void UTC077_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetOphthalmologistMetricsAsync", null, "77");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "77");
    }
}
