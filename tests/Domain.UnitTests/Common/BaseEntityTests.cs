using Domain.Common;
using FluentAssertions;

namespace Domain.UnitTests.Common;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity
    {
        public TestEntity() : base() { }
        public TestEntity(Guid id) : base(id) { }
    }

    private class TestDomainEvent : DomainEvent { }

    [Fact]
    public void Constructor_Default_ShouldGenerateId()
    {
        var entity = new TestEntity();

        entity.Id.Should().NotBeEmpty();
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        entity.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithId_ShouldUseProvidedId()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);

        entity.Id.Should().Be(id);
    }

    [Fact]
    public void AddDomainEvent_ShouldAddToCollection()
    {
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent();

        entity.AddDomainEvent(domainEvent);

        entity.DomainEvents.Should().ContainSingle()
            .Which.Should().Be(domainEvent);
    }

    [Fact]
    public void RemoveDomainEvent_ShouldRemoveFromCollection()
    {
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent();
        entity.AddDomainEvent(domainEvent);

        entity.RemoveDomainEvent(domainEvent);

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        var entity = new TestEntity();
        entity.AddDomainEvent(new TestDomainEvent());
        entity.AddDomainEvent(new TestDomainEvent());

        entity.ClearDomainEvents();

        entity.DomainEvents.Should().BeEmpty();
    }
}
