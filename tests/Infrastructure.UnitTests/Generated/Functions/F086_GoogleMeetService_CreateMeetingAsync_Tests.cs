using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F086_GoogleMeetService_CreateMeetingAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.GoogleMeetService), "CreateMeetingAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IE1lZXRpbmdJbmZvKG1lZXRMaW5rLCBjcmVhdGVkRXZlbnQuSWQp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC086_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.GoogleMeetService), "86");
    }

    [Fact]
    public void UTC086_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.GoogleMeetService), "CreateMeetingAsync", null, "86");
    }

    [Fact]
    public void UTC086_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.GoogleMeetService), "CreateMeetingAsync", isProperty: false, parameterCount: null, utcNo: "86");
    }

    [Fact]
    public void UTC086_Return_Case_Set_Should_Be_Valid()
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
    public void UTC086_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC086_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "86");
    }

    [Fact]
    public void UTC086_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.GoogleMeetService), "CreateMeetingAsync", null, "86");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "86");
    }
}
