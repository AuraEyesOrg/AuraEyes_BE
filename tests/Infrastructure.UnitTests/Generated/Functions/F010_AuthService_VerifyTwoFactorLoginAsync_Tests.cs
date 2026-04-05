using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F010_AuthService_VerifyTwoFactorLoginAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "VerifyTwoFactorLoginAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uVW5hdXRob3JpemVkKCJVc2VyIG5vdCBmb3VuZCBvciBpbmFjdGl2ZSIp" };
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uRmFpbHVyZSgiVHdvLWZhY3RvciBhdXRoZW50aWNhdGlvbiBpcyBub3QgZW5hYmxlZCBmb3IgdGhpcyBhY2NvdW50Iik=" };
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uVW5hdXRob3JpemVkKCJJbnZhbGlkIHZlcmlmaWNhdGlvbiBjb2RlIik=" };
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uU3VjY2VzcyhhdXRoUmVzcG9uc2Up" };
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uRmFpbHVyZSgiQW4gZXJyb3Igb2NjdXJyZWQgZHVyaW5nIHZlcmlmaWNhdGlvbiIp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "UmVjb3ZlcnkgY29kZSB1c2VkIGZvciB1c2VyOiB7VXNlcklkfQ==" };
            yield return new object[] { "SW52YWxpZCAyRkEgY29kZSBmb3IgdXNlcjoge1VzZXJJZH0=" };
            yield return new object[] { "RXJyb3IgZHVyaW5nIDJGQSB2ZXJpZmljYXRpb24gZm9yIHVzZXI6IHtVc2VySWR9" };
    }

    [Fact]
    public void UTC010_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "10");
    }

    [Fact]
    public void UTC010_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "VerifyTwoFactorLoginAsync", null, "10");
    }

    [Fact]
    public void UTC010_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "VerifyTwoFactorLoginAsync", isProperty: false, parameterCount: null, utcNo: "10");
    }

    [Fact]
    public void UTC010_Return_Case_Set_Should_Be_Valid()
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
    public void UTC010_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC010_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "10");
    }

    [Fact]
    public void UTC010_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "VerifyTwoFactorLoginAsync", null, "10");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "10");
    }
}
