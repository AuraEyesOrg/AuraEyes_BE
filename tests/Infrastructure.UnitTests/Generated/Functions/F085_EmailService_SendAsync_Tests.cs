using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F085_EmailService_SendAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.EmailService), "SendAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield break;
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "RmFpbGVkIHRvIHNlbmQgZW1haWwgLSBUbzoge1RvfSwgU3ViamVjdDoge1N1YmplY3R9LCBFcnJvcjoge0Vycm9yfQ==" };
    }

    [Fact]
    public void UTC085_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.EmailService), "85");
    }

    [Fact]
    public void UTC085_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.EmailService), "SendAsync", null, "85");
    }

    [Fact]
    public void UTC085_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.EmailService), "SendAsync", isProperty: false, parameterCount: null, utcNo: "85");
    }

    [Fact]
    public void UTC085_Return_Case_Set_Should_Be_Valid()
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
    public void UTC085_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC085_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "85");
    }

    [Fact]
    public void UTC085_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.EmailService), "SendAsync", null, "85");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "85");
    }
}
