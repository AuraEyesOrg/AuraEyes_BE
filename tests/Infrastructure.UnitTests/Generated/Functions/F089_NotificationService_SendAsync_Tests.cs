using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F089_NotificationService_SendAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.NotificationService), "SendAsync", isProperty: false, parameterCount: 7);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield break;
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "RmFpbGVkIHRvIHNlbmQgbm90aWZpY2F0aW9uIHRvIFVzZXIge1VzZXJJZH06IHtNZXNzYWdlfQ==" };
    }

    [Fact]
    public void UTC089_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.NotificationService), "89");
    }

    [Fact]
    public void UTC089_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.NotificationService), "SendAsync", 7, "89");
    }

    [Fact]
    public void UTC089_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.NotificationService), "SendAsync", isProperty: false, parameterCount: 7, utcNo: "89");
    }

    [Fact]
    public void UTC089_Return_Case_Set_Should_Be_Valid()
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
    public void UTC089_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC089_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "89");
    }

    [Fact]
    public void UTC089_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.NotificationService), "SendAsync", 7, "89");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "89");
    }
}
