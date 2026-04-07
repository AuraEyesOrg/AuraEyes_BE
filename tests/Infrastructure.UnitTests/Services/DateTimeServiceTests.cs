using FluentAssertions;
using Infrastructure.Services;

namespace Infrastructure.UnitTests.Services;

public class DateTimeServiceTests
{
    [Fact]
    public void Now_ShouldReturnLocalTime_CloseToSystemNow()
    {
        var service = new DateTimeService();
        var before = DateTime.Now;
        var actual = service.Now;
        var after = DateTime.Now;

        actual.Should().BeOnOrAfter(before);
        actual.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void UtcNow_ShouldReturnUtcTime_CloseToSystemUtcNow()
    {
        var service = new DateTimeService();
        var before = DateTime.UtcNow;
        var actual = service.UtcNow;
        var after = DateTime.UtcNow;

        actual.Should().BeOnOrAfter(before);
        actual.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void UtcNow_ShouldHaveUtcKind()
    {
        var service = new DateTimeService();

        service.UtcNow.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Now_ShouldHaveLocalOrUnspecifiedKind()
    {
        var service = new DateTimeService();
        var now = service.Now;

        now.Kind.Should().BeOneOf(DateTimeKind.Local, DateTimeKind.Unspecified);
    }

    [Fact]
    public void ConsecutiveUtcNowCalls_ShouldBeNonDecreasing()
    {
        var service = new DateTimeService();
        var first = service.UtcNow;
        var second = service.UtcNow;

        second.Should().BeOnOrAfter(first);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void ConsecutiveUtcNowCalls_MultipleSamples_ShouldBeNonDecreasing(int sampleCount)
    {
        var service = new DateTimeService();
        var values = new List<DateTime>();
        for (var i = 0; i < sampleCount; i++)
        {
            values.Add(service.UtcNow);
        }

        for (var i = 1; i < values.Count; i++)
        {
            values[i].Should().BeOnOrAfter(values[i - 1]);
        }
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void Now_Kind_AcrossMultipleReads_ShouldBeLocalOrUnspecified(int sampleCount)
    {
        var service = new DateTimeService();

        for (var i = 0; i < sampleCount; i++)
        {
            service.Now.Kind.Should().BeOneOf(DateTimeKind.Local, DateTimeKind.Unspecified);
        }
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void Now_AcrossMultipleReads_ShouldBeNonDecreasing(int sampleCount)
    {
        var service = new DateTimeService();
        var values = new List<DateTime>();
        for (var i = 0; i < sampleCount; i++)
        {
            values.Add(service.Now);
        }

        for (var i = 1; i < values.Count; i++)
        {
            values[i].Should().BeOnOrAfter(values[i - 1]);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void UtcNow_ManySamples_ShouldAlwaysBeUtc(int sampleCount)
    {
        var service = new DateTimeService();

        for (var i = 0; i < sampleCount; i++)
        {
            service.UtcNow.Kind.Should().Be(DateTimeKind.Utc);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void Now_ManySamples_ShouldNeverBeUtc(int sampleCount)
    {
        var service = new DateTimeService();

        for (var i = 0; i < sampleCount; i++)
        {
            service.Now.Kind.Should().NotBe(DateTimeKind.Utc);
        }
    }
}
