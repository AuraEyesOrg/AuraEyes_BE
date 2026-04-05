using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F079_DashboardMetricsService_GetPatientMetricsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetPatientMetricsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IFBhdGllbnREYXNoYm9hcmRNZXRyaWNzRHRvKCk=" };
            yield return new object[] { "bmV3IFBhdGllbnREYXNoYm9hcmRNZXRyaWNzRHRvIHsgQ29tcGxldGVkU2NyZWVuaW5ncyA9IGF3YWl0IF9jb250ZXh0LkFpU2NyZWVuaW5ncy5Db3VudEFzeW5jKHNjcmVlbmluZyA9PiBzY3JlZW5pbmcuUGF0aWVudElkID09IHBhdGllbnQuSWQgJiYgc2NyZWVuaW5nLlByb2Nlc3NlZEF0Lkhhc1ZhbHVlLCBjYW5jZWxsYXRpb25Ub2tlbiksIFRvdGFsUmVwb3J0cyA9IGF3YWl0IChmcm9tIHNjcmVlbmluZyBpbiBfY29udGV4dC5BaVNjcmVlbmluZ3Mgam9pbiByZXBvcnQgaW4gX2NvbnRleHQuU2NyZWVuaW5nUmVzdWx0cyBvbiBzY3JlZW5pbmcuSWQgZXF1YWxzIHJlcG9ydC5BaVNjcmVlbmluZ0lkIHdoZXJlIHNjcmVlbmluZy5QYXRpZW50SWQgPT0gcGF0aWVudC5JZCBzZWxlY3QgcmVwb3J0LklkKS5Db3VudEFzeW5jKGNhbmNlbGxhdGlvblRva2VuKSwgVXBjb21pbmdBcHBvaW50bWVudHMgPSBhd2FpdCBfY29udGV4dC5BcHBvaW50bWVudHMuQ291bnRBc3luYyhhcHBvaW50bWVudCA9PiBhcHBvaW50bWVudC5QYXRpZW50SWQgPT0gcGF0aWVudC5JZCAmJiAoYXBwb2ludG1lbnQuU3RhdHVzID09IEFwcG9pbnRtZW50U3RhdHVzLlBlbmRpbmcgfHwgYXBwb2ludG1lbnQuU3RhdHVzID09IEFwcG9pbnRtZW50U3RhdHVzLkNvbmZpcm1lZCksIGNhbmNlbGxhdGlvblRva2VuKSwgUmVtYWluaW5nUXVvdGEgPSBxdW90YS5SZW1haW5pbmdRdW90YSB9" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC079_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.DashboardMetricsService), "79");
    }

    [Fact]
    public void UTC079_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetPatientMetricsAsync", null, "79");
    }

    [Fact]
    public void UTC079_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetPatientMetricsAsync", isProperty: false, parameterCount: null, utcNo: "79");
    }

    [Fact]
    public void UTC079_Return_Case_Set_Should_Be_Valid()
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
    public void UTC079_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC079_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "79");
    }

    [Fact]
    public void UTC079_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetPatientMetricsAsync", null, "79");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "79");
    }
}
