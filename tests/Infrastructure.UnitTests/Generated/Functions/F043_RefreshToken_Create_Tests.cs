using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F043_RefreshToken_Create_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Identity.RefreshToken), "Create", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IFJlZnJlc2hUb2tlbiB7IFVzZXJJZCA9IHVzZXJJZCwgVG9rZW5IYXNoID0gdG9rZW5IYXNoLCBKd3RJZCA9IGp3dElkLCBFeHBpcmVzQXQgPSBEYXRlVGltZS5VdGNOb3cuQWRkRGF5cyhleHBpcnlEYXlzKSwgRGV2aWNlSW5mbyA9IGRldmljZUluZm8sIElwQWRkcmVzcyA9IGlwQWRkcmVzcyB9" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC043_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Identity.RefreshToken), "43");
    }

    [Fact]
    public void UTC043_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.RefreshToken), "Create", null, "43");
    }

    [Fact]
    public void UTC043_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Identity.RefreshToken), "Create", isProperty: false, parameterCount: null, utcNo: "43");
    }

    [Fact]
    public void UTC043_Return_Case_Set_Should_Be_Valid()
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
    public void UTC043_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC043_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "43");
    }

    [Fact]
    public void UTC043_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Identity.RefreshToken), "Create", null, "43");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "43");
    }
}
