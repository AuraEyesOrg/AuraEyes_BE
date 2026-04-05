using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F018_AuthService_ResendConfirmationAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "ResendConfirmationAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0LlN1Y2Nlc3MoKQ==" };
            yield return new object[] { "UmVzdWx0LkZhaWx1cmUoIkFuIGVycm9yIG9jY3VycmVkIHdoaWxlIHByb2Nlc3NpbmcgeW91ciByZXF1ZXN0Iik=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "Q29uZmlybWF0aW9uIGVtYWlsIHJlc2VudCB0bzoge0VtYWlsfQ==" };
            yield return new object[] { "RXJyb3IgcmVzZW5kaW5nIGNvbmZpcm1hdGlvbiBlbWFpbDoge0VtYWlsfQ==" };
    }

    [Fact]
    public void UTC018_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "18");
    }

    [Fact]
    public void UTC018_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "ResendConfirmationAsync", null, "18");
    }

    [Fact]
    public void UTC018_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "ResendConfirmationAsync", isProperty: false, parameterCount: null, utcNo: "18");
    }

    [Fact]
    public void UTC018_Return_Case_Set_Should_Be_Valid()
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
    public void UTC018_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC018_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "18");
    }

    [Fact]
    public void UTC018_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "ResendConfirmationAsync", null, "18");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "18");
    }
}
