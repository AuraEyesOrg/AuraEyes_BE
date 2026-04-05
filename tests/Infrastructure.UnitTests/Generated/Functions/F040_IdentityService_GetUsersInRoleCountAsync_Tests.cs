using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F040_IdentityService_GetUsersInRoleCountAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.IdentityService), "GetUsersInRoleCountAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "YWN0aXZlT25seSA/IHVzZXJzSW5Sb2xlLkNvdW50KHUgPT4gdS5Jc0FjdGl2ZSAmJiAhdS5Jc0RlbGV0ZWQpIDogdXNlcnNJblJvbGUuQ291bnQodSA9PiAhdS5Jc0RlbGV0ZWQp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC040_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.IdentityService), "40");
    }

    [Fact]
    public void UTC040_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetUsersInRoleCountAsync", null, "40");
    }

    [Fact]
    public void UTC040_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.IdentityService), "GetUsersInRoleCountAsync", isProperty: false, parameterCount: null, utcNo: "40");
    }

    [Fact]
    public void UTC040_Return_Case_Set_Should_Be_Valid()
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
    public void UTC040_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC040_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "40");
    }

    [Fact]
    public void UTC040_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.IdentityService), "GetUsersInRoleCountAsync", null, "40");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "40");
    }
}
