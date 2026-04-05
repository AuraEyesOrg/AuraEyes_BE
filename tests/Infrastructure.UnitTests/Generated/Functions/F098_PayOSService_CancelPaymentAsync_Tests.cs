using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F098_PayOSService_CancelPaymentAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.PayOSService), "CancelPaymentAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "dHJ1ZQ==" };
            yield return new object[] { "ZmFsc2U=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield return new object[] { "Q2FuY2VsbGluZyBQYXlPUyBwYXltZW50OiBPcmRlckNvZGU9e09yZGVyQ29kZX0=" };
            yield return new object[] { "UGF5T1MgcGF5bWVudCBjYW5jZWxsZWQgc3VjY2Vzc2Z1bGx5OiBPcmRlckNvZGU9e09yZGVyQ29kZX0=" };
            yield return new object[] { "RmFpbGVkIHRvIGNhbmNlbCBQYXlPUyBwYXltZW50OiBPcmRlckNvZGU9e09yZGVyQ29kZX0=" };
    }

    [Fact]
    public void UTC098_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.PayOSService), "98");
    }

    [Fact]
    public void UTC098_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.PayOSService), "CancelPaymentAsync", null, "98");
    }

    [Fact]
    public void UTC098_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.PayOSService), "CancelPaymentAsync", isProperty: false, parameterCount: null, utcNo: "98");
    }

    [Fact]
    public void UTC098_Return_Case_Set_Should_Be_Valid()
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
    public void UTC098_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC098_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "98");
    }

    [Fact]
    public void UTC098_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.PayOSService), "CancelPaymentAsync", null, "98");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "98");
    }
}
