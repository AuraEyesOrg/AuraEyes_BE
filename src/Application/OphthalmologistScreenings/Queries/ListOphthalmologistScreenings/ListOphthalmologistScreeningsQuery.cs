using Application.Common.Interfaces;

namespace Application.OphthalmologistScreenings.Queries.ListOphthalmologistScreenings;

/// <summary>
/// Query to retrieve all AI screenings visible to an ophthalmologist via linked consultation sessions.
/// </summary>
public record ListOphthalmologistScreeningsQuery : IQuery<IReadOnlyList<OphthalmologistScreeningListItemDto>>
{
    public required Guid OphthalmologistProfileId { get; init; }
}
