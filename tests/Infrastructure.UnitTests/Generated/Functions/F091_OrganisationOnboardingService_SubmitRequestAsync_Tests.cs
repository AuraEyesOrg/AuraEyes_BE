using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F091_OrganisationOnboardingService_SubmitRequestAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.OrganisationOnboardingService), "SubmitRequestAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0PE9yZ2FuaXNhdGlvblJlZ2lzdHJhdGlvblJlc3BvbnNlPi5Db25mbGljdCggIkEgcGVuZGluZyBvcmdhbmlzYXRpb24gb25ib2FyZGluZyByZXF1ZXN0IGFscmVhZHkgZXhpc3RzIGZvciB0aGlzIGVtYWlsLiIp" };
            yield return new object[] { "UmVzdWx0PE9yZ2FuaXNhdGlvblJlZ2lzdHJhdGlvblJlc3BvbnNlPi5TdWNjZXNzKG5ldyBPcmdhbmlzYXRpb25SZWdpc3RyYXRpb25SZXNwb25zZSB7IFJlcXVlc3RJZCA9IG9uYm9hcmRpbmdSZXF1ZXN0LklkLCBFbWFpbCA9IG9uYm9hcmRpbmdSZXF1ZXN0LkNvbnRhY3RFbWFpbCwgTWVzc2FnZSA9ICJPcmdhbmlzYXRpb24gcmVnaXN0cmF0aW9uIHN1Ym1pdHRlZCBzdWNjZXNzZnVsbHkuIFN5c3RlbSBBZG1pbiB3aWxsIHJldmlldyBhbmQgc2VuZCBhY2NvdW50IGRldGFpbHMgYnkgZW1haWwuIiB9KQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC091_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.OrganisationOnboardingService), "91");
    }

    [Fact]
    public void UTC091_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.OrganisationOnboardingService), "SubmitRequestAsync", null, "91");
    }

    [Fact]
    public void UTC091_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.OrganisationOnboardingService), "SubmitRequestAsync", isProperty: false, parameterCount: null, utcNo: "91");
    }

    [Fact]
    public void UTC091_Return_Case_Set_Should_Be_Valid()
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
    public void UTC091_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC091_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "91");
    }

    [Fact]
    public void UTC091_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.OrganisationOnboardingService), "SubmitRequestAsync", null, "91");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "91");
    }
}
