using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F011_AuthService_RefreshTokenAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "RefreshTokenAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uVW5hdXRob3JpemVkKCJJbnZhbGlkIGFjY2VzcyB0b2tlbiIp" };
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uVW5hdXRob3JpemVkKCJJbnZhbGlkIHJlZnJlc2ggdG9rZW4iKQ==" };
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uVW5hdXRob3JpemVkKCJUb2tlbiBoYXMgYmVlbiByZXZva2VkLiBQbGVhc2UgbG9naW4gYWdhaW4uIik=" };
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uVW5hdXRob3JpemVkKCJUb2tlbiBtaXNtYXRjaCIp" };
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uVW5hdXRob3JpemVkKCJVc2VyIG5vdCBmb3VuZCBvciBpbmFjdGl2ZSIp" };
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uU3VjY2VzcyhuZXcgQXV0aFJlc3BvbnNlIHsgU3VjY2VlZGVkID0gdHJ1ZSwgQWNjZXNzVG9rZW4gPSB0b2tlblJlc3VsdC5BY2Nlc3NUb2tlbiwgUmVmcmVzaFRva2VuID0gbmV3UmVmcmVzaFRva2VuLCBFeHBpcmVzQXQgPSB0b2tlblJlc3VsdC5FeHBpcmVzQXQsIFVzZXIgPSBuZXcgVXNlckluZm9SZXNwb25zZSB7IElkID0gdXNlci5JZCwgRW1haWwgPSB1c2VyLkVtYWlsISwgRnVsbE5hbWUgPSB1c2VyLkZ1bGxOYW1lLCBSb2xlcyA9IHJvbGVzLlRvQXJyYXkoKSwgRW1haWxDb25maXJtZWQgPSB1c2VyLkVtYWlsQ29uZmlybWVkLCBPcmdhbml6YXRpb25JZCA9IHVzZXIuT3JnYW5pemF0aW9uSWQsIFJvbGVJZCA9IHJvbGVJZCwgVHdvRmFjdG9yRW5hYmxlZCA9IGF3YWl0IF91c2VyTWFuYWdlci5HZXRUd29GYWN0b3JFbmFibGVkQXN5bmModXNlciksIElzVmVyaWZpZWQgPSBpc1ZlcmlmaWVkLCBWZXJpZmljYXRpb25TdGF0dXMgPSB2ZXJpZmljYXRpb25TdGF0dXMsIENvbnRyYWN0U3RhdHVzID0gY29udHJhY3RTdGF0dXMgfSB9KQ==" };
            yield return new object[] { "UmVzdWx0PEF1dGhSZXNwb25zZT4uRmFpbHVyZSgiQW4gZXJyb3Igb2NjdXJyZWQgd2hpbGUgcmVmcmVzaGluZyB0b2tlbiIp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "UmVmcmVzaCB0b2tlbiByZXVzZSBkZXRlY3RlZCBmb3IgdXNlciB7VXNlcklkfQ==" };
            yield return new object[] { "VG9rZW4gcmVmcmVzaGVkIGZvciB1c2VyOiB7VXNlcklkfQ==" };
            yield return new object[] { "RXJyb3IgcmVmcmVzaGluZyB0b2tlbg==" };
    }

    [Fact]
    public void UTC011_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "11");
    }

    [Fact]
    public void UTC011_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "RefreshTokenAsync", null, "11");
    }

    [Fact]
    public void UTC011_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "RefreshTokenAsync", isProperty: false, parameterCount: null, utcNo: "11");
    }

    [Fact]
    public void UTC011_Return_Case_Set_Should_Be_Valid()
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
    public void UTC011_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC011_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "11");
    }

    [Fact]
    public void UTC011_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "RefreshTokenAsync", null, "11");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "11");
    }
}
