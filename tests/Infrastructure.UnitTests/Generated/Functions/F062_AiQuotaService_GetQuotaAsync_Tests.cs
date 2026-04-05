using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F062_AiQuotaService_GetQuotaAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.AiQuotaService), "GetQuotaAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "YXdhaXQgR2V0UGF0aWVudFF1b3RhQXN5bmModXNlcklkLCBjYW5jZWxsYXRpb25Ub2tlbik=" };
            yield return new object[] { "YXdhaXQgR2V0T3JnUXVvdGFBc3luYyh1c2VySWQsIGNhbmNlbGxhdGlvblRva2VuKQ==" };
            yield return new object[] { "bmV3IEFpUXVvdGFEdG8geyBUb3RhbFF1b3RhID0gMCwgVXNlZFF1b3RhID0gMCwgUmVtYWluaW5nUXVvdGEgPSAwLCBRdW90YVNvdXJjZSA9ICJOb25lIiB9" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "W0FpUXVvdGFTZXJ2aWNlXSBHZXRRdW90YUFzeW5jIGNhbGxlZCDDouKCrOKAnSBVc2VySWQ6IHtVc2VySWR9LCBSb2xlOiB7Um9sZX0=" };
            yield return new object[] { "W0FpUXVvdGFTZXJ2aWNlXSBVbnJlY29nbml6ZWQgcm9sZSAne1JvbGV9JyBmb3IgdXNlciB7VXNlcklkfSDDouKCrOKAnSByZXR1cm5pbmcgTm9uZSBxdW90YQ==" };
    }

    [Fact]
    public void UTC062_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.AiQuotaService), "62");
    }

    [Fact]
    public void UTC062_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.AiQuotaService), "GetQuotaAsync", null, "62");
    }

    [Fact]
    public void UTC062_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.AiQuotaService), "GetQuotaAsync", isProperty: false, parameterCount: null, utcNo: "62");
    }

    [Fact]
    public void UTC062_Return_Case_Set_Should_Be_Valid()
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
    public void UTC062_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC062_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "62");
    }

    [Fact]
    public void UTC062_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.AiQuotaService), "GetQuotaAsync", null, "62");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "62");
    }
}
