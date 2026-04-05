using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F078_DashboardMetricsService_GetOrganisationMetricsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetOrganisationMetricsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IE9yZ2FuaXNhdGlvbkRhc2hib2FyZE1ldHJpY3NEdG8oKQ==" };
            yield return new object[] { "bmV3IE9yZ2FuaXNhdGlvbkRhc2hib2FyZE1ldHJpY3NEdG8geyBVdGlsaXphdGlvblJhdGVQZXJjZW50ID0gdXRpbGl6YXRpb25SYXRlLCBSZW1haW5pbmdBaVF1b3RhID0gcXVvdGEuUmVtYWluaW5nUXVvdGEsIFRvdGFsQXBwb2ludG1lbnRzID0gdG90YWxBcHBvaW50bWVudHMsIEFwcG9pbnRtZW50U3RhdHVzID0gbmV3IE9yZ2FuaXNhdGlvbkFwcG9pbnRtZW50U3RhdHVzQnJlYWtkb3duRHRvIHsgUGVuZGluZyA9IHBlbmRpbmdDb3VudCwgQ29uZmlybWVkID0gY29uZmlybWVkQ291bnQsIENvbXBsZXRlZCA9IGNvbXBsZXRlZENvdW50LCBDYW5jZWxsZWQgPSBjYW5jZWxsZWRDb3VudCwgTm9TaG93ID0gbm9TaG93Q291bnQgfSB9" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC078_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.DashboardMetricsService), "78");
    }

    [Fact]
    public void UTC078_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetOrganisationMetricsAsync", null, "78");
    }

    [Fact]
    public void UTC078_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetOrganisationMetricsAsync", isProperty: false, parameterCount: null, utcNo: "78");
    }

    [Fact]
    public void UTC078_Return_Case_Set_Should_Be_Valid()
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
    public void UTC078_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC078_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "78");
    }

    [Fact]
    public void UTC078_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetOrganisationMetricsAsync", null, "78");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "78");
    }
}
