using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F104_SystemSettingService_GetAllSettingsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.SystemSettingService), "GetAllSettingsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "c2V0dGluZ3NMaXN0LlRvRGljdGlvbmFyeShzID0+IHMuS2V5LCBzID0+IHMuVmFsdWUp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC104_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.SystemSettingService), "104");
    }

    [Fact]
    public void UTC104_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.SystemSettingService), "GetAllSettingsAsync", null, "104");
    }

    [Fact]
    public void UTC104_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.SystemSettingService), "GetAllSettingsAsync", isProperty: false, parameterCount: null, utcNo: "104");
    }

    [Fact]
    public void UTC104_Return_Case_Set_Should_Be_Valid()
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
    public void UTC104_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC104_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "104");
    }

    [Fact]
    public void UTC104_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.SystemSettingService), "GetAllSettingsAsync", null, "104");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "104");
    }
}
