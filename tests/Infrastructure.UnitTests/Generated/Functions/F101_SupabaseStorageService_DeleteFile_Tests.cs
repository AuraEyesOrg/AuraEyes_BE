using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F101_SupabaseStorageService_DeleteFile_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.SupabaseStorageService), "DeleteFile", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "ZmFsc2U=" };
            yield return new object[] { "dHJ1ZQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "RGVsZXRlZCBmaWxlIGZyb20gU3VwYWJhc2UgU3RvcmFnZToge1BhdGh9" };
            yield return new object[] { "RmFpbGVkIHRvIGRlbGV0ZSBmaWxlOiB7UGF0aH0=" };
    }

    [Fact]
    public void UTC101_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.SupabaseStorageService), "101");
    }

    [Fact]
    public void UTC101_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.SupabaseStorageService), "DeleteFile", null, "101");
    }

    [Fact]
    public void UTC101_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.SupabaseStorageService), "DeleteFile", isProperty: false, parameterCount: null, utcNo: "101");
    }

    [Fact]
    public void UTC101_Return_Case_Set_Should_Be_Valid()
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
    public void UTC101_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC101_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "101");
    }

    [Fact]
    public void UTC101_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.SupabaseStorageService), "DeleteFile", null, "101");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "101");
    }
}
