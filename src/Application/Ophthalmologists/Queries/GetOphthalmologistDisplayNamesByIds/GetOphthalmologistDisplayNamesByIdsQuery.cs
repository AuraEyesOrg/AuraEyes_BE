using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Queries.GetOphthalmologistDisplayNamesByIds;

public record GetOphthalmologistDisplayNamesByIdsQuery : IQuery<IReadOnlyDictionary<Guid, string>>
{
    public IReadOnlyCollection<Guid> Ids { get; init; } = Array.Empty<Guid>();
}
