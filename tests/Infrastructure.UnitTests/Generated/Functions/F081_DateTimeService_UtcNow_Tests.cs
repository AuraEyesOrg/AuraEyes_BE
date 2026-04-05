using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F081_DateTimeService_UtcNow_Tests
{
    [Fact]
    public void UTC081_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.DateTimeService), "81");
    }

    [Fact]
    public void UTC081_Property_Should_Exist()
    {
        FunctionAssertionHelper.AssertPropertyExists(typeof(Infrastructure.Services.DateTimeService), "UtcNow", "81");
    }

    [Fact]
    public void UTC081_Source_Should_Contain_Property_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.DateTimeService), "UtcNow", isProperty: true, parameterCount: null, utcNo: "81");
    }

    [Fact]
    public void UTC081_Property_Should_Be_Readable()
    {
        var property = FunctionAssertionHelper.AssertPropertyExists(typeof(Infrastructure.Services.DateTimeService), "UtcNow", "81");
        Assert.True(property.CanRead, $"UTC 81: Property DateTimeService.UtcNow should be readable.");
    }
}
