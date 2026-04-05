using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F097_PayOSService_VerifyWebhookSignatureAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.PayOSService), "VerifyWebhookSignatureAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "VGFzay5Gcm9tUmVzdWx0KHRydWUp" };
            yield return new object[] { "VGFzay5Gcm9tUmVzdWx0KGZhbHNlKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "VmVyaWZ5aW5nIFBheU9TIHdlYmhvb2sgc2lnbmF0dXJl" };
            yield return new object[] { "RmFpbGVkIHRvIHZlcmlmeSB3ZWJob29rIHNpZ25hdHVyZQ==" };
    }

    [Fact]
    public void UTC097_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.PayOSService), "97");
    }

    [Fact]
    public void UTC097_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.PayOSService), "VerifyWebhookSignatureAsync", null, "97");
    }

    [Fact]
    public void UTC097_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.PayOSService), "VerifyWebhookSignatureAsync", isProperty: false, parameterCount: null, utcNo: "97");
    }

    [Fact]
    public void UTC097_Return_Case_Set_Should_Be_Valid()
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
    public void UTC097_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC097_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "97");
    }

    [Fact]
    public void UTC097_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.PayOSService), "VerifyWebhookSignatureAsync", null, "97");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "97");
    }
}
