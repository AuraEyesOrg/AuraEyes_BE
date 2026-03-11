using Domain.Common;
using FluentAssertions;

namespace Domain.UnitTests.Common;

public class DomainEventTests
{
    private class TestDomainEvent : DomainEvent { }

    [Fact]
    public void DomainEvent_ShouldSetOccurredOnToUtcNow()
    {
        var before = DateTime.UtcNow;

        var domainEvent = new TestDomainEvent();

        domainEvent.OccurredOn.Should().BeOnOrAfter(before);
        domainEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void DomainEvent_ShouldGenerateUniqueEventId()
    {
        var event1 = new TestDomainEvent();
        var event2 = new TestDomainEvent();

        event1.EventId.Should().NotBeEmpty();
        event2.EventId.Should().NotBeEmpty();
        event1.EventId.Should().NotBe(event2.EventId);
    }
}
