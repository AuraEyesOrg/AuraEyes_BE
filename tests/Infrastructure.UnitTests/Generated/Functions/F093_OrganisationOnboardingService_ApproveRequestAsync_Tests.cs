using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F093_OrganisationOnboardingService_ApproveRequestAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.OrganisationOnboardingService), "ApproveRequestAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0PEFwcHJvdmVPcmdhbmlzYXRpb25PbmJvYXJkaW5nUmVzdWx0Pi5Ob3RGb3VuZCggIk9yZ2FuaXNhdGlvbiBvbmJvYXJkaW5nIHJlcXVlc3Qgbm90IGZvdW5kLiIp" };
            yield return new object[] { "UmVzdWx0PEFwcHJvdmVPcmdhbmlzYXRpb25PbmJvYXJkaW5nUmVzdWx0Pi5GYWlsdXJlKCAiVGhpcyBvbmJvYXJkaW5nIHJlcXVlc3QgaGFzIGFscmVhZHkgYmVlbiBwcm9jZXNzZWQuIik=" };
            yield return new object[] { "UmVzdWx0PEFwcHJvdmVPcmdhbmlzYXRpb25PbmJvYXJkaW5nUmVzdWx0Pi5Db25mbGljdCggIkEgdXNlciB3aXRoIHRoaXMgb3JnYW5pc2F0aW9uIGNvbnRhY3QgZW1haWwgYWxyZWFkeSBleGlzdHMuIik=" };
            yield return new object[] { "UmVzdWx0PEFwcHJvdmVPcmdhbmlzYXRpb25PbmJvYXJkaW5nUmVzdWx0Pi5GYWlsdXJlKCBjcmVhdGVSZXN1bHQuRXJyb3JzLlNlbGVjdChlID0+IGUuRGVzY3JpcHRpb24pKQ==" };
            yield return new object[] { "UmVzdWx0PEFwcHJvdmVPcmdhbmlzYXRpb25PbmJvYXJkaW5nUmVzdWx0Pi5TdWNjZXNzKG5ldyBBcHByb3ZlT3JnYW5pc2F0aW9uT25ib2FyZGluZ1Jlc3VsdCB7IFJlcXVlc3RJZCA9IHJlcXVlc3QuSWQsIE9yZ2FuaXNhdGlvbklkID0gb3JnYW5pc2F0aW9uLklkLCBPcmdBZG1pblVzZXJJZCA9IG9yZ0FkbWluLklkLCBPcmdBZG1pbkVtYWlsID0gcmVxdWVzdC5Db250YWN0RW1haWwsIFRlbXBvcmFyeVBhc3N3b3JkID0gdGVtcG9yYXJ5UGFzc3dvcmQgfSk=" };
            yield return new object[] { "UmVzdWx0PEFwcHJvdmVPcmdhbmlzYXRpb25PbmJvYXJkaW5nUmVzdWx0Pi5GYWlsdXJlKCAiRmFpbGVkIHRvIGFwcHJvdmUgb3JnYW5pc2F0aW9uIG9uYm9hcmRpbmcgcmVxdWVzdC4iKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "RmFpbGVkIHRvIGFwcHJvdmUgb3JnYW5pc2F0aW9uIG9uYm9hcmRpbmcgcmVxdWVzdCB7UmVxdWVzdElkfQ==" };
    }

    [Fact]
    public void UTC093_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.OrganisationOnboardingService), "93");
    }

    [Fact]
    public void UTC093_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.OrganisationOnboardingService), "ApproveRequestAsync", null, "93");
    }

    [Fact]
    public void UTC093_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.OrganisationOnboardingService), "ApproveRequestAsync", isProperty: false, parameterCount: null, utcNo: "93");
    }

    [Fact]
    public void UTC093_Return_Case_Set_Should_Be_Valid()
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
    public void UTC093_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC093_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "93");
    }

    [Fact]
    public void UTC093_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.OrganisationOnboardingService), "ApproveRequestAsync", null, "93");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "93");
    }
}
