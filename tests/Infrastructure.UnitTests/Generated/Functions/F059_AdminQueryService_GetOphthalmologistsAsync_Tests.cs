using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F059_AdminQueryService_GetOphthalmologistsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.AdminQueryService), "GetOphthalmologistsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IE9waHRoYWxtb2xvZ2lzdExpc3REdG8geyBJZCA9IHJvdy5PcGh0aGFsbW9sb2dpc3RJZCwgVXNlcklkID0gcm93LlVzZXJJZCwgRnVsbE5hbWUgPSByb3cuRnVsbE5hbWUsIEVtYWlsID0gcm93LkVtYWlsLCBQaG9uZSA9IHJvdy5QaG9uZSwgQmlvID0gcm93LkJpbywgWWVhcnNPZkV4cGVyaWVuY2UgPSByb3cuWWVhcnNPZkV4cGVyaWVuY2UsIEVtcGxveW1lbnRUeXBlID0gcm93LkVtcGxveW1lbnRUeXBlLCBXb3JraW5nSG91cnNQZXJXZWVrID0gcm93LldvcmtpbmdIb3Vyc1BlcldlZWssIEV4cGVjdGVkTW9udGhseVNhbGFyeSA9IHJvdy5FeHBlY3RlZE1vbnRobHlTYWxhcnksIENvbW1pc3Npb25SYXRlID0gcm93LkNvbW1pc3Npb25SYXRlLCBBY3R1YWxNb250aGx5U2FsYXJ5ID0gcm93LkFjdHVhbE1vbnRobHlTYWxhcnksIFZlcmlmaWNhdGlvblN0YXR1cyA9IHJvdy5WZXJpZmljYXRpb25TdGF0dXMsIElzVmVyaWZpZWQgPSByb3cuSXNWZXJpZmllZCwgTGljZW5zZVVybCA9IHJvdy5MaWNlbnNlVXJsLCBEZWdyZWVVcmwgPSByb3cuRGVncmVlVXJsLCBMaWNlbnNlcyA9IGxpY2Vuc2VzLCBEZWdyZWVzID0gZGVncmVlcywgUmVqZWN0aW9uUmVhc29uID0gcm93LlJlamVjdGlvblJlYXNvbiwgT3JnYW5pc2F0aW9uTmFtZSA9IHJvdy5PcmdhbmlzYXRpb25OYW1lLCBJc0FjdGl2ZSA9IHJvdy5Jc0FjdGl2ZSwgQ3JlYXRlZEF0ID0gcm93LkNyZWF0ZWRBdCB9" };
            yield return new object[] { "bmV3IFBhZ2VkUmVzdWx0PE9waHRoYWxtb2xvZ2lzdExpc3REdG8+KCBpdGVtcywgdG90YWxDb3VudCwgcGFnZU51bWJlciwgcGFnZVNpemUp" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC059_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.AdminQueryService), "59");
    }

    [Fact]
    public void UTC059_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.AdminQueryService), "GetOphthalmologistsAsync", null, "59");
    }

    [Fact]
    public void UTC059_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.AdminQueryService), "GetOphthalmologistsAsync", isProperty: false, parameterCount: null, utcNo: "59");
    }

    [Fact]
    public void UTC059_Return_Case_Set_Should_Be_Valid()
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
    public void UTC059_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC059_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "59");
    }

    [Fact]
    public void UTC059_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.AdminQueryService), "GetOphthalmologistsAsync", null, "59");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "59");
    }
}
