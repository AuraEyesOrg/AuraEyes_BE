using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F029_IdentityService_GetUserIdsByRoleAndOrganizationAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.IdentityService), "GetUserIdsByRoleAndOrganizationAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "dXNlcnNJblJvbGUgLldoZXJlKHUgPT4gdS5Pcmdhbml6YXRpb25JZCA9PSBvcmdhbml6YXRpb25JZCAmJiB1LklzQWN0aXZlICYmICF1LklzRGVsZXRlZCkgLlNlbGVjdCh1ID0+IHUuSWQpIC5Ub0xpc3QoKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC029_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.IdentityService), "29");
    }

    [Fact]
    public void UTC029_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetUserIdsByRoleAndOrganizationAsync", null, "29");
    }

    [Fact]
    public void UTC029_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.IdentityService), "GetUserIdsByRoleAndOrganizationAsync", isProperty: false, parameterCount: null, utcNo: "29");
    }

    [Fact]
    public void UTC029_Return_Case_Set_Should_Be_Valid()
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
    public void UTC029_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC029_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "29");
    }

    [Fact]
    public void UTC029_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetUserIdsByRoleAndOrganizationAsync", null, "29");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "29");
    }
}
