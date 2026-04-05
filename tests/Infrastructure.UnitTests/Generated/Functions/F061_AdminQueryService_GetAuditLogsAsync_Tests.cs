using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F061_AdminQueryService_GetAuditLogsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.AdminQueryService), "GetAuditLogsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IFBhZ2VkUmVzdWx0PEF1ZGl0TG9nRHRvPihpdGVtcywgdG90YWxDb3VudCwgcGFnZU51bWJlciwgcGFnZVNpemUp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC061_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.AdminQueryService), "61");
    }

    [Fact]
    public void UTC061_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.AdminQueryService), "GetAuditLogsAsync", null, "61");
    }

    [Fact]
    public void UTC061_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.AdminQueryService), "GetAuditLogsAsync", isProperty: false, parameterCount: null, utcNo: "61");
    }

    [Fact]
    public void UTC061_Return_Case_Set_Should_Be_Valid()
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
    public void UTC061_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC061_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "61");
    }

    [Fact]
    public void UTC061_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.AdminQueryService), "GetAuditLogsAsync", null, "61");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "61");
    }
}
