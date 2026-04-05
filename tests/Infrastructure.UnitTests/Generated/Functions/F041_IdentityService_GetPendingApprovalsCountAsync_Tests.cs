using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F041_IdentityService_GetPendingApprovalsCountAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.IdentityService), "GetPendingApprovalsCountAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "YXdhaXQgX3VzZXJNYW5hZ2VyLlVzZXJzIC5Db3VudEFzeW5jKHUgPT4gIXUuSXNEZWxldGVkICYmICghdS5FbWFpbENvbmZpcm1lZCB8fCAhdS5Jc0FjdGl2ZSksIGNhbmNlbGxhdGlvblRva2VuKQ==" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC041_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.IdentityService), "41");
    }

    [Fact]
    public void UTC041_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetPendingApprovalsCountAsync", null, "41");
    }

    [Fact]
    public void UTC041_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.IdentityService), "GetPendingApprovalsCountAsync", isProperty: false, parameterCount: null, utcNo: "41");
    }

    [Fact]
    public void UTC041_Return_Case_Set_Should_Be_Valid()
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
    public void UTC041_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC041_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "41");
    }

    [Fact]
    public void UTC041_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetPendingApprovalsCountAsync", null, "41");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "41");
    }
}
