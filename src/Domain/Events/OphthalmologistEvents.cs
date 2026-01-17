using Domain.Common;

namespace Domain.Events;

/// <summary>
/// Domain events for Ophthalmologist aggregate
/// Used to communicate state changes within the domain
/// </summary>

public class OphthalmologistCreatedEvent : DomainEvent
{
    public Guid OphthalmologistId { get; }
    public Guid UserId { get; }

    public OphthalmologistCreatedEvent(Guid ophthalmologistId, Guid userId)
    {
        OphthalmologistId = ophthalmologistId;
        UserId = userId;
    }
}

public class OphthalmologistUpdatedEvent : DomainEvent
{
    public Guid OphthalmologistId { get; }
    public Guid UserId { get; }

    public OphthalmologistUpdatedEvent(Guid ophthalmologistId, Guid userId)
    {
        OphthalmologistId = ophthalmologistId;
        UserId = userId;
    }
}

public class OphthalmologistVerifiedEvent : DomainEvent
{
    public Guid OphthalmologistId { get; }
    public Guid UserId { get; }

    public OphthalmologistVerifiedEvent(Guid ophthalmologistId, Guid userId)
    {
        OphthalmologistId = ophthalmologistId;
        UserId = userId;
    }
}

public class OphthalmologistUnverifiedEvent : DomainEvent
{
    public Guid OphthalmologistId { get; }
    public Guid UserId { get; }

    public OphthalmologistUnverifiedEvent(Guid ophthalmologistId, Guid userId)
    {
        OphthalmologistId = ophthalmologistId;
        UserId = userId;
    }
}
