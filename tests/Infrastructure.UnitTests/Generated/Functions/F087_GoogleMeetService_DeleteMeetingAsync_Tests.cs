using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F087_GoogleMeetService_DeleteMeetingAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.GoogleMeetService), "DeleteMeetingAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield break;
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "RGVsZXRlZCBDYWxlbmRhciBldmVudCB7RXZlbnRJZH0=" };
            yield return new object[] { "Q2FsZW5kYXIgZXZlbnQge0V2ZW50SWR9IGFscmVhZHkgZGVsZXRlZA==" };
    }

    [Fact]
    public void UTC087_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.GoogleMeetService), "87");
    }

    [Fact]
    public void UTC087_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.GoogleMeetService), "DeleteMeetingAsync", null, "87");
    }

    [Fact]
    public void UTC087_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.GoogleMeetService), "DeleteMeetingAsync", isProperty: false, parameterCount: null, utcNo: "87");
    }

    [Fact]
    public void UTC087_Return_Case_Set_Should_Be_Valid()
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
    public void UTC087_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC087_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "87");
    }

    [Fact]
    public void UTC087_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.GoogleMeetService), "DeleteMeetingAsync", null, "87");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "87");
    }
}
