using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.Ophthalmologists.Queries.GetOphthalmologists;

public class GetOphthalmologistsQueryHandler : IQueryHandler<GetOphthalmologistsQuery, PagedResult<OphthalmologistListDto>>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public GetOphthalmologistsQueryHandler(IOphthalmologistRepository ophthalmologistRepository)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<PagedResult<OphthalmologistListDto>>> Handle(GetOphthalmologistsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var allOphthalmologists = await _ophthalmologistRepository.GetAllAsync(cancellationToken);

            // Apply filters
            var filteredOphthalmologists = allOphthalmologists.AsEnumerable();

            if (request.IsVerified.HasValue)
            {
                filteredOphthalmologists = filteredOphthalmologists.Where(o => o.IsVerified == request.IsVerified.Value);
            }

            if (request.MinYearsOfExperience.HasValue)
            {
                filteredOphthalmologists = filteredOphthalmologists.Where(o => o.YearsOfExperience >= request.MinYearsOfExperience.Value);
            }

            if (request.MaxYearsOfExperience.HasValue)
            {
                filteredOphthalmologists = filteredOphthalmologists.Where(o => o.YearsOfExperience <= request.MaxYearsOfExperience.Value);
            }

            var totalCount = filteredOphthalmologists.Count();

            // Apply pagination and sorting
            var ophthalmologistDtos = filteredOphthalmologists
                .OrderByDescending(o => o.YearsOfExperience)
                .ThenBy(o => o.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OphthalmologistListDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    Bio = o.Bio,
                    YearsOfExperience = o.YearsOfExperience,
                    IsVerified = o.IsVerified,
                    CreatedAt = o.CreatedAt
                })
                .ToList();

            var pagedResult = new PagedResult<OphthalmologistListDto>(
                ophthalmologistDtos,
                totalCount,
                request.PageNumber,
                request.PageSize
            );

            return Result<PagedResult<OphthalmologistListDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<OphthalmologistListDto>>.Failure($"Error retrieving ophthalmologists: {ex.Message}");
        }
    }
}
