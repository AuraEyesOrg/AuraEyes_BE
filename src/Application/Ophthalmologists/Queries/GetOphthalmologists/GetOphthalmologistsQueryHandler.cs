using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.Queries.GetOphthalmologists;

/// <summary>
/// Handler for GetOphthalmologistsQuery.
/// </summary>
public class GetOphthalmologistsQueryHandler : IQueryHandler<GetOphthalmologistsQuery, PagedResult<OphthalmologistListDto>>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public GetOphthalmologistsQueryHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<OphthalmologistListDto>>> Handle(
        GetOphthalmologistsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _ophthalmologistRepository.GetPagedAsync(
            request.SearchTerm,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = new List<OphthalmologistListDto>();

        foreach (var ophthalmologist in items)
        {
            var user = await _identityService.GetUserByIdAsync(ophthalmologist.UserId, cancellationToken);

            dtoList.Add(new OphthalmologistListDto
            {
                Id = ophthalmologist.Id,
                UserId = ophthalmologist.UserId,
                UserFullName = user?.FullName,
                UserEmail = user?.Email,
                UserAvatarUrl = user?.AvatarUrl,
                Bio = ophthalmologist.Bio,
                CertificateCount = ophthalmologist.Certificates.Count,
                CreatedAt = ophthalmologist.CreatedAt,
                LicenseUrl = ophthalmologist.LicenseUrl,
                DegreeUrl = ophthalmologist.DegreeUrl,
                RatingAverage = ophthalmologist.RatingAverage,
                RatingCount = ophthalmologist.RatingCount,
                ConsultationFee = ophthalmologist.ConsultationFee,
                MinPrice = null,
                MaxPrice = null,
                Degrees = ophthalmologist.Certificates
                    .Where(c => c.Type == CertificateType.Degree)
                    .OrderByDescending(c => c.IssuedDate)
                    .Select(c => new DegreeDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        DegreeLevel = c.DegreeLevel?.ToString(),
                        IssuingAuthority = c.IssuingAuthority,
                        IssuedDate = c.IssuedDate,
                        DegreeUrl = c.CertificateUrl,
                        Title = GetDegreeTitle(c.DegreeLevel),
                        Abbreviation = GetDegreeAbbreviation(c.DegreeLevel)
                    })
                    .ToList(),
                Certificates = ophthalmologist.Certificates
                    .Where(c => c.Type == CertificateType.License)
                    .OrderByDescending(c => c.IssuedDate)
                    .Select(c => new CertificateDto
                    {
                        Id = c.Id,
                        Type = c.Type.ToString(),
                        Name = c.Name,
                        DegreeLevel = c.DegreeLevel?.ToString(),
                        IssuingAuthority = c.IssuingAuthority,
                        IssuedDate = c.IssuedDate,
                        ExpiryDate = c.ExpiryDate,
                        CertificateUrl = c.CertificateUrl,
                        IsExpired = c.IsExpired
                    })
                    .ToList()
            });
        }

        var pagedResult = new PagedResult<OphthalmologistListDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<OphthalmologistListDto>>.Success(pagedResult);
    }

    private static string? GetDegreeTitle(DegreeLevel? level)
    {
        return level switch
        {
            DegreeLevel.Bachelor => "Bachelor",
            DegreeLevel.Master => "Master",
            DegreeLevel.Doctor => "Doctor (PhD)",
            DegreeLevel.AssociateProfessor => "Associate Professor",
            DegreeLevel.Professor => "Professor",
            _ => null
        };
    }

    private static string? GetDegreeAbbreviation(DegreeLevel? level)
    {
        return level switch
        {
            DegreeLevel.Bachelor => "B.S.",
            DegreeLevel.Master => "M.S.",
            DegreeLevel.Doctor => "Ph.D.",
            DegreeLevel.AssociateProfessor => "Assoc. Prof.",
            DegreeLevel.Professor => "Prof.",
            _ => null
        };
    }
}
