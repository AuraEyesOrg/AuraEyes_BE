using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F015_AuthService_ForgotPasswordAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "ForgotPasswordAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "c3VjY2VzcyB0byBwcmV2ZW50IGVtYWlsIGVudW1lcmF0aW9uIHJldHVybiBSZXN1bHQuU3VjY2Vzcygp" };
            yield return new object[] { "UmVzdWx0LkZhaWx1cmUoIkFuIGVycm9yIG9jY3VycmVkIHdoaWxlIHByb2Nlc3NpbmcgeW91ciByZXF1ZXN0Iik=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "UGFzc3dvcmQgcmVzZXQgcmVxdWVzdGVkIGZvcjoge0VtYWlsfQ==" };
            yield return new object[] { "UGFzc3dvcmQgcmVzZXQgcmVxdWVzdGVkIGZvciBub24tZXhpc3RlbnQgZW1haWw6IHtFbWFpbH0=" };
            yield return new object[] { "RXJyb3IgZHVyaW5nIGZvcmdvdCBwYXNzd29yZDoge0VtYWlsfQ==" };
    }

    [Fact]
    public void UTC015_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "15");
    }

    [Fact]
    public void UTC015_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "ForgotPasswordAsync", null, "15");
    }

    [Fact]
    public void UTC015_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "ForgotPasswordAsync", isProperty: false, parameterCount: null, utcNo: "15");
    }

    [Fact]
    public void UTC015_Return_Case_Set_Should_Be_Valid()
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
    public void UTC015_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC015_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "15");
    }

    [Fact]
    public void UTC015_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "ForgotPasswordAsync", null, "15");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "15");
    }
}
