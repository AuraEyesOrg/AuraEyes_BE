using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F063_AiQuotaService_HasAvailableQuotaAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.AiQuotaService), "HasAvailableQuotaAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "cXVvdGEuUmVtYWluaW5nUXVvdGEgPiAw" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC063_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.AiQuotaService), "63");
    }

    [Fact]
    public void UTC063_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.AiQuotaService), "HasAvailableQuotaAsync", null, "63");
    }

    [Fact]
    public void UTC063_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.AiQuotaService), "HasAvailableQuotaAsync", isProperty: false, parameterCount: null, utcNo: "63");
    }

    [Fact]
    public void UTC063_Return_Case_Set_Should_Be_Valid()
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
    public void UTC063_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC063_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "63");
    }

    [Fact]
    public void UTC063_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.AiQuotaService), "HasAvailableQuotaAsync", null, "63");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "63");
    }
}
