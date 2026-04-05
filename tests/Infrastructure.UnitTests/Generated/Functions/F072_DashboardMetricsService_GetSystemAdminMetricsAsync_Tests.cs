using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F072_DashboardMetricsService_GetSystemAdminMetricsAsync_Tests
{
    private static string LoadSource()
    {
        return FunctionAssertionHelper.GetMemberSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetSystemAdminMetricsAsync", isProperty: false, parameterCount: null);
    }

    public static IEnumerable<object[]> ReturnCases()
    {
            yield return new object[] { "bmV3IFBheW1lbnRNZXRob2RSZXZlbnVlRHRvIHsgUGF5bWVudE1ldGhvZCA9IGl0ZW0uUGF5bWVudE1ldGhvZC5Ub1N0cmluZygpLCBBbW91bnQgPSBhbW91bnQsIFBlcmNlbnRhZ2UgPSB0b3RhbERlcG9zaXRBbW91bnQgPD0gMG0gPyAwbSA6IE1hdGguUm91bmQoYW1vdW50IC8gdG90YWxEZXBvc2l0QW1vdW50ICogMTAwbSwgMSkgfQ==" };
            yield return new object[] { "bmV3IERhc2hib2FyZE1ldHJpY3NEdG8geyBEb2N0b3JzID0gbmV3IFVzZXJHcm93dGhNZXRyaWNEdG8geyBUb3RhbCA9IGRvY3RvclRvdGFsLCBDdXJyZW50TW9udGggPSBkb2N0b3JDdXJyZW50TW9udGgsIFByZXZpb3VzTW9udGggPSBkb2N0b3JQcmV2aW91c01vbnRoLCBHcm93dGhQZXJjZW50YWdlID0gQ2FsY3VsYXRlR3Jvd3RoUGVyY2VudGFnZShkb2N0b3JDdXJyZW50TW9udGgsIGRvY3RvclByZXZpb3VzTW9udGgpIH0sIE9yZ2FuaXNhdGlvbnMgPSBuZXcgVXNlckdyb3d0aE1ldHJpY0R0byB7IFRvdGFsID0gb3JnYW5pc2F0aW9uVG90YWwsIEN1cnJlbnRNb250aCA9IG9yZ2FuaXNhdGlvbkN1cnJlbnRNb250aCwgUHJldmlvdXNNb250aCA9IG9yZ2FuaXNhdGlvblByZXZpb3VzTW9udGgsIEdyb3d0aFBlcmNlbnRhZ2UgPSBDYWxjdWxhdGVHcm93dGhQZXJjZW50YWdlKG9yZ2FuaXNhdGlvbkN1cnJlbnRNb250aCwgb3JnYW5pc2F0aW9uUHJldmlvdXNNb250aCkgfSwgUGF0aWVudHMgPSBuZXcgVXNlckdyb3d0aE1ldHJpY0R0byB7IFRvdGFsID0gcGF0aWVudFRvdGFsLCBDdXJyZW50TW9udGggPSBwYXRpZW50Q3VycmVudE1vbnRoLCBQcmV2aW91c01vbnRoID0gcGF0aWVudFByZXZpb3VzTW9udGgsIEdyb3d0aFBlcmNlbnRhZ2UgPSBDYWxjdWxhdGVHcm93dGhQZXJjZW50YWdlKHBhdGllbnRDdXJyZW50TW9udGgsIHBhdGllbnRQcmV2aW91c01vbnRoKSB9LCBQYXltZW50TWV0aG9kQnJlYWtkb3duID0gcGF5bWVudE1ldGhvZEJyZWFrZG93biwgTW9udGhseVJldmVudWUgPSBtb250aGx5UmV2ZW51ZSwgRGFpbHlSZXZlbnVlID0gZGFpbHlSZXZlbnVlLCBUb3RhbERlcG9zaXRSZXZlbnVlWWVhciA9IHRvdGFsRGVwb3NpdFJldmVudWVZZWFyLCBUb3RhbFBsYXRmb3JtQ29tbWlzc2lvblllYXIgPSB0b3RhbFBsYXRmb3JtQ29tbWlzc2lvblllYXIsIE1vbnRobHlQbGF0Zm9ybUNvbW1pc3Npb24gPSBtb250aGx5UGxhdGZvcm1Db21taXNzaW9uLCBEYWlseVBsYXRmb3JtQ29tbWlzc2lvbiA9IGRhaWx5UGxhdGZvcm1Db21taXNzaW9uLCBNb250aGx5TmV3RG9jdG9yQ291bnRzID0gbW9udGhseU5ld0RvY3RvckNvdW50cywgTW9udGhseU5ld09yZ2FuaXNhdGlvbkNvdW50cyA9IG1vbnRobHlOZXdPcmdhbmlzYXRpb25Db3VudHMsIE1vbnRobHlOZXdQYXRpZW50Q291bnRzID0gbW9udGhseU5ld1BhdGllbnRDb3VudHMsIFBlbmRpbmdBY3Rpb25zID0gbmV3IERhc2hib2FyZFBlbmRpbmdBY3Rpb25zRHRvIHsgUGVuZGluZ09waHRoYWxtb2xvZ2lzdFZlcmlmaWNhdGlvbnMgPSBwZW5kaW5nRG9jdG9yVmVyaWZpY2F0aW9ucywgUGVuZGluZ1dpdGhkcmF3YWxSZXF1ZXN0cyA9IHBlbmRpbmdXaXRoZHJhd2FscywgUGVuZGluZ09yZ2FuaXNhdGlvbk9uYm9hcmRpbmcgPSBwZW5kaW5nT25ib2FyZGluZyB9LCBTeXN0ZW1TdGF0dXMgPSBuZXcgRGFzaGJvYXJkU3lzdGVtU3RhdHVzRHRvIHsgTGl2ZUNvbnN1bHRhdGlvblNlc3Npb25zID0gbGl2ZUNvbnN1bHRhdGlvbnMsIEFwaUhlYWx0aHkgPSB0cnVlLCBEYXRhYmFzZUhlYWx0aHkgPSBkYXRhYmFzZUhlYWx0aHkgfSwgQmV0dGVyU3RhY2sgPSBuZXcgRGFzaGJvYXJkQmV0dGVyU3RhY2tEdG8geyBFbmFibGVkID0gbW9uaXRvckRlc2NyaXB0b3JzLkFueShpdGVtID0+IGl0ZW0uQ29uZmlndXJlZCksIEVtYmVkVXJsID0gX2JldHRlclN0YWNrSGVhcnRiZWF0U2VydmljZS5HZXRFbWJlZFVybCgpLCBNb25pdG9ycyA9IG1vbml0b3JEZXNjcmlwdG9ycyAuU2VsZWN0KGl0ZW0gPT4gbmV3IERhc2hib2FyZEJhY2tncm91bmRNb25pdG9yRHRvIHsgS2V5ID0gaXRlbS5LZXksIE5hbWUgPSBpdGVtLkRpc3BsYXlOYW1lLCBDYXRlZ29yeSA9IGl0ZW0uQ2F0ZWdvcnksIENvbmZpZ3VyZWQgPSBpdGVtLkNvbmZpZ3VyZWQgfSkgLlRvTGlzdCgpIH0sIFRvcERvY3RvcnNCeUNvbnN1bHRhdGlvblJldmVudWUgPSB0b3BEb2N0b3JSb3dzLCBUb3BPcmdhbmlzYXRpb25zQnlSYXRpbmcgPSB0b3BPcmdSb3dzIH0=" };
    }

    public static IEnumerable<object[]> LogCases()
    {
            yield break;
    }

    [Fact]
    public void UTC072_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.DashboardMetricsService), "72");
    }

    [Fact]
    public void UTC072_Method_Should_Exist()
    {
        FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetSystemAdminMetricsAsync", null, "72");
    }

    [Fact]
    public void UTC072_Source_Should_Contain_Method_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.DashboardMetricsService), "GetSystemAdminMetricsAsync", isProperty: false, parameterCount: null, utcNo: "72");
    }

    [Fact]
    public void UTC072_Return_Case_Set_Should_Be_Valid()
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
    public void UTC072_Log_Message_Case_Set_Should_Be_Valid()
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
    public void UTC072_When_Logger_Used_Should_Follow_Log_Message_Convention()
    {
        FunctionAssertionHelper.AssertLogMessageConventionIfPresent(LoadSource(), "72");
    }

    [Fact]
    public void UTC072_When_Result_Response_Used_Should_Follow_Response_Convention()
    {
        var method = FunctionAssertionHelper.AssertMethodExists(typeof(Infrastructure.Services.DashboardMetricsService), "GetSystemAdminMetricsAsync", null, "72");
        FunctionAssertionHelper.AssertResponseConventionIfApplicable(LoadSource(), method, "72");
    }
}
