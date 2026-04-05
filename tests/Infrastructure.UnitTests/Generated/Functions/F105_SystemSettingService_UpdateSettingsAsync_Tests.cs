using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F105_SystemSettingService_UpdateSettingsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.SystemSettingService), "UpdateSettingsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield break;
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "U3lzdGVtIHNldHRpbmdzIHVwZGF0ZWQgZm9yIGtleXM6IHtLZXlzfQ==" };
    }

    [Fact]
    public void UTC105_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.SystemSettingService), "105");
    }

    [Fact]
    public void UTC105_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.SystemSettingService), "UpdateSettingsAsync", null, "105");
    }

    [Fact]
    public void UTC105_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.SystemSettingService), "UpdateSettingsAsync", isProperty: false, parameterCount: null, utcNo: "105");
    }

    [Fact]
    public void UTC105_Return_Case_Set_Should_Be_Valid()
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
    public void UTC105_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC105_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "105");
    }

    [Fact]
    public void UTC105_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.SystemSettingService), "UpdateSettingsAsync", null, "105");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "105");
    }
}
