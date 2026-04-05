using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F074_DashboardMetricsService_GetScreeningVolumeTrendsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetScreeningVolumeTrendsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "ZGF0ZS5EYXRlLkFkZERheXMoLW9mZnNldCk=" };
            yield return new object[] { "bmV3IFZvbHVtZVRyZW5kRGF0YVBvaW50IHsgRGF0ZSA9IGRhdGUsIExhYmVsID0gZGF0ZS5Ub1N0cmluZygiTU1NIHl5eXkiKSwgQ291bnQgPSBpdGVtLkNvdW50IH0=" };
            yield return new object[] { "bmV3IFNjcmVlbmluZ1ZvbHVtZVRyZW5kc0R0byB7IFRpbWVSYW5nZSA9IG5vcm1hbGl6ZWRUaW1lUmFuZ2UsIERhdGFQb2ludHMgPSBkYXRhUG9pbnRzLCBUb3RhbFNjcmVlbmluZ3MgPSBkYXRhUG9pbnRzLlN1bShpdGVtID0+IGl0ZW0uQ291bnQpLCBBdmVyYWdlUGVyUGVyaW9kID0gZGF0YVBvaW50cy5Db3VudCA9PSAwID8gMCA6IE1hdGguUm91bmQoKGRlY2ltYWwpZGF0YVBvaW50cy5TdW0oaXRlbSA9PiBpdGVtLkNvdW50KSAvIGRhdGFQb2ludHMuQ291bnQsIDEpIH0=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC074_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.DashboardMetricsService), "74");
    }

    [Fact]
    public void UTC074_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetScreeningVolumeTrendsAsync", null, "74");
    }

    [Fact]
    public void UTC074_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetScreeningVolumeTrendsAsync", isProperty: false, parameterCount: null, utcNo: "74");
    }

    [Fact]
    public void UTC074_Return_Case_Set_Should_Be_Valid()
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
    public void UTC074_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC074_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "74");
    }

    [Fact]
    public void UTC074_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetScreeningVolumeTrendsAsync", null, "74");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "74");
    }
}
