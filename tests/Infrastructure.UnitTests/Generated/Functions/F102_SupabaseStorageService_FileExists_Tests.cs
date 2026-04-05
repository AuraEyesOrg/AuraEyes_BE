using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F102_SupabaseStorageService_FileExists_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.SupabaseStorageService), "FileExists", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "ZmFsc2U=" };
            yield return new object[] { "cmVzcG9uc2UuSXNTdWNjZXNzU3RhdHVzQ29kZQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "RXJyb3IgY2hlY2tpbmcgZmlsZSBleGlzdGVuY2U6IHtQYXRofQ==" };
    }

    [Fact]
    public void UTC102_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.SupabaseStorageService), "102");
    }

    [Fact]
    public void UTC102_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.SupabaseStorageService), "FileExists", null, "102");
    }

    [Fact]
    public void UTC102_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.SupabaseStorageService), "FileExists", isProperty: false, parameterCount: null, utcNo: "102");
    }

    [Fact]
    public void UTC102_Return_Case_Set_Should_Be_Valid()
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
    public void UTC102_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC102_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "102");
    }

    [Fact]
    public void UTC102_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.SupabaseStorageService), "FileExists", null, "102");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "102");
    }
}
