using Xunit;

namespace Infrastructure.UnitTests.Generated.Functions;

public class F080_DateTimeService_Now_Tests
{
    [Fact]
    public void UTC080_Type_Should_Exist()
    {
        FunctionAssertionHelper.AssertPublicTypeExists(typeof(Infrastructure.Services.DateTimeService), "80");
    }

    [Fact]
    public void UTC080_Property_Should_Exist()
    {
        FunctionAssertionHelper.AssertPropertyExists(typeof(Infrastructure.Services.DateTimeService), "Now", "80");
    }

    [Fact]
    public void UTC080_Source_Should_Contain_Property_Declaration()
    {
        FunctionAssertionHelper.AssertMemberDeclaredInSource(typeof(Infrastructure.Services.DateTimeService), "Now", isProperty: true, parameterCount: null, utcNo: "80");
    }

    [Fact]
    public void UTC080_Property_Should_Be_Readable()
    {
        var property = FunctionAssertionHelper.AssertPropertyExists(typeof(Infrastructure.Services.DateTimeService), "Now", "80");
        Assert.True(property.CanRead, $"UTC 80: Property DateTimeService.Now should be readable.");
    }
}
