using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F094_PatientRoadmapGenerationService_GenerateFromDiagnosisAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.PatientRoadmapGenerationService), "GenerateFromDiagnosisAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "UmVzdWx0PEdlbmVyYXRlZFBhdGllbnRSb2FkbWFwPi5GYWlsdXJlKCJQYXRpZW50IElEIGlzIHJlcXVpcmVkIGZvciByb2FkbWFwIGdlbmVyYXRpb24uIik=" };
            yield return new object[] { "UmVzdWx0PEdlbmVyYXRlZFBhdGllbnRSb2FkbWFwPi5GYWlsdXJlKCJTY3JlZW5pbmcgSUQgaXMgcmVxdWlyZWQgZm9yIHJvYWRtYXAgZ2VuZXJhdGlvbi4iKQ==" };
            yield return new object[] { "UmVzdWx0PEdlbmVyYXRlZFBhdGllbnRSb2FkbWFwPi5GYWlsdXJlKCJBSSBzY3JlZW5pbmcgcmVzdWx0IGlzIHJlcXVpcmVkIGZvciByb2FkbWFwIGdlbmVyYXRpb24uIik=" };
            yield return new object[] { "UmVzdWx0PEdlbmVyYXRlZFBhdGllbnRSb2FkbWFwPi5GYWlsdXJlKCJHb29nbGUgQUkgU3R1ZGlvIEFQSSBrZXkgaXMgbm90IGNvbmZpZ3VyZWQuIik=" };
            yield return new object[] { "UmVzdWx0PEdlbmVyYXRlZFBhdGllbnRSb2FkbWFwPi5GYWlsdXJlKCAiQUkgcmV0dXJuZWQgYW4gaW52YWxpZCByb2FkbWFwIGZvcm1hdCBhZnRlciByZXRyaWVzLiIp" };
            yield return new object[] { "UmVzdWx0PEdlbmVyYXRlZFBhdGllbnRSb2FkbWFwPi5TdWNjZXNzKHJvYWRtYXAhKQ==" };
            yield return new object[] { "UmVzdWx0PEdlbmVyYXRlZFBhdGllbnRSb2FkbWFwPi5GYWlsdXJlKCAiVW5hYmxlIHRvIGdlbmVyYXRlIHBhdGllbnQgcm9hZG1hcCBmcm9tIEFJIGF0IHRoaXMgdGltZS4iKQ==" };
            yield return new object[] { "UmVzdWx0PEdlbmVyYXRlZFBhdGllbnRSb2FkbWFwPi5GYWlsdXJlKCJVbmFibGUgdG8gZ2VuZXJhdGUgcGF0aWVudCByb2FkbWFwLiIp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "UGF0aWVudCByb2FkbWFwIGdlbmVyYXRpb24gYXR0ZW1wdCB7QXR0ZW1wdH0ve01heEF0dGVtcHRzfSBmYWlsZWQ=" };
    }

    [Fact]
    public void UTC094_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.PatientRoadmapGenerationService), "94");
    }

    [Fact]
    public void UTC094_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.PatientRoadmapGenerationService), "GenerateFromDiagnosisAsync", null, "94");
    }

    [Fact]
    public void UTC094_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.PatientRoadmapGenerationService), "GenerateFromDiagnosisAsync", isProperty: false, parameterCount: null, utcNo: "94");
    }

    [Fact]
    public void UTC094_Return_Case_Set_Should_Be_Valid()
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
    public void UTC094_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC094_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "94");
    }

    [Fact]
    public void UTC094_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.PatientRoadmapGenerationService), "GenerateFromDiagnosisAsync", null, "94");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "94");
    }
}
