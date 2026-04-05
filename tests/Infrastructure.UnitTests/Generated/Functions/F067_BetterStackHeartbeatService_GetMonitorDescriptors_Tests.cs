using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F067_BetterStackHeartbeatService_GetMonitorDescriptors_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.BetterStackHeartbeatService), "GetMonitorDescriptors", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "RW51bS5HZXRWYWx1ZXM8QmV0dGVyU3RhY2tNb25pdG9yPigpIC5TZWxlY3QobW9uaXRvciA9PiB7IHZhciBtZXRhID0gTW9uaXRvck1ldGFbbW9uaXRvcl0=" };
            yield return new object[] { "bmV3IEJldHRlclN0YWNrTW9uaXRvckRlc2NyaXB0b3IoIG1vbml0b3IsIG1ldGEuS2V5LCBtZXRhLk5hbWUsIG1ldGEuQ2F0ZWdvcnksIFJlc29sdmVFbmRwb2ludChtb25pdG9yKS5Jc0NvbmZpZ3VyZWQp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC067_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.BetterStackHeartbeatService), "67");
    }

    [Fact]
    public void UTC067_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.BetterStackHeartbeatService), "GetMonitorDescriptors", null, "67");
    }

    [Fact]
    public void UTC067_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.BetterStackHeartbeatService), "GetMonitorDescriptors", isProperty: false, parameterCount: null, utcNo: "67");
    }

    [Fact]
    public void UTC067_Return_Case_Set_Should_Be_Valid()
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
    public void UTC067_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC067_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "67");
    }

    [Fact]
    public void UTC067_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.BetterStackHeartbeatService), "GetMonitorDescriptors", null, "67");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "67");
    }
}
