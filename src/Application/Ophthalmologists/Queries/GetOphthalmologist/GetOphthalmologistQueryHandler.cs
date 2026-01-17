using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.Ophthalmologists.Queries.GetOphthalmologist;

public class GetOphthalmologistQueryHandler : IQueryHandler<GetOphthalmologistQuery, OphthalmologistDto>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public GetOphthalmologistQueryHandler(IOphthalmologistRepository ophthalmologistRepository)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<OphthalmologistDto>> Handle(GetOphthalmologistQuery request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.Id, cancellationToken);

        if (ophthalmologist == null)
        {
            return Result<OphthalmologistDto>.Failure($"Ophthalmologist with ID {request.Id} not found");
        }

        var ophthalmologistDto = new OphthalmologistDto
        {
            Id = ophthalmologist.Id,
            UserId = ophthalmologist.UserId,
            Bio = ophthalmologist.Bio,
            YearsOfExperience = ophthalmologist.YearsOfExperience,
            IsVerified = ophthalmologist.IsVerified,
            CreatedAt = ophthalmologist.CreatedAt,
            UpdatedAt = ophthalmologist.UpdatedAt
        };

        return Result<OphthalmologistDto>.Success(ophthalmologistDto);
    }
}
