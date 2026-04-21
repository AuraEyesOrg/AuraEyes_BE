using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.Ophthalmologists.Queries.GetOphthalmologistDisplayNamesByIds;

public class GetOphthalmologistDisplayNamesByIdsQueryHandler
    : IQueryHandler<GetOphthalmologistDisplayNamesByIdsQuery, IReadOnlyDictionary<Guid, string>>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public GetOphthalmologistDisplayNamesByIdsQueryHandler(IOphthalmologistRepository ophthalmologistRepository)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<IReadOnlyDictionary<Guid, string>>> Handle(
        GetOphthalmologistDisplayNamesByIdsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Ids.Count == 0)
        {
            return Result<IReadOnlyDictionary<Guid, string>>.Success(new Dictionary<Guid, string>());
        }

        var namesById = await _ophthalmologistRepository.GetDisplayNamesByIdsAsync(request.Ids, cancellationToken);
        return Result<IReadOnlyDictionary<Guid, string>>.Success(namesById);
    }
}
