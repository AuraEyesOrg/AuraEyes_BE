using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F005_AuthService_RegisterPatientAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.AuthService), "RegisterPatientAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0PFJlZ2lzdGVyUmVzcG9uc2U+LkZhaWx1cmUoIkEgdXNlciB3aXRoIHRoaXMgZW1haWwgYWxyZWFkeSBleGlzdHMiKQ==" };
            yield return new object[] { "UmVzdWx0PFJlZ2lzdGVyUmVzcG9uc2U+LkZhaWx1cmUoY3JlYXRlUmVzdWx0LkVycm9ycy5TZWxlY3QoZSA9PiBlLkRlc2NyaXB0aW9uKSk=" };
            yield return new object[] { "UmVzdWx0PFJlZ2lzdGVyUmVzcG9uc2U+LlN1Y2Nlc3MobmV3IFJlZ2lzdGVyUmVzcG9uc2UgeyBVc2VySWQgPSB1c2VyLklkLCBFbWFpbCA9IHVzZXIuRW1haWwhLCBNZXNzYWdlID0gIlJlZ2lzdHJhdGlvbiBzdWNjZXNzZnVsLiBQbGVhc2UgY2hlY2sgeW91ciBlbWFpbCB0byBjb25maXJtIHlvdXIgYWNjb3VudC4iIH0p" };
            yield return new object[] { "UmVzdWx0PFJlZ2lzdGVyUmVzcG9uc2U+LkZhaWx1cmUoIkFuIGVycm9yIG9jY3VycmVkIGR1cmluZyByZWdpc3RyYXRpb24iKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "UGF0aWVudCByZWdpc3RlcmVkOiB7RW1haWx9" };
            yield return new object[] { "RXJyb3IgcmVnaXN0ZXJpbmcgcGF0aWVudDoge0VtYWlsfQ==" };
    }

    [Fact]
    public void UTC005_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.AuthService), "5");
    }

    [Fact]
    public void UTC005_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "RegisterPatientAsync", null, "5");
    }

    [Fact]
    public void UTC005_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.AuthService), "RegisterPatientAsync", isProperty: false, parameterCount: null, utcNo: "5");
    }

    [Fact]
    public void UTC005_Return_Case_Set_Should_Be_Valid()
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
    public void UTC005_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC005_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "5");
    }

    [Fact]
    public void UTC005_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.AuthService), "RegisterPatientAsync", null, "5");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "5");
    }
}
