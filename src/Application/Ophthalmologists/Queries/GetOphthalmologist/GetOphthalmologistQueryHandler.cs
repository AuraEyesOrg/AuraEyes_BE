using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.Queries.GetOphthalmologist;

/// <summary>
/// Handler for GetOphthalmologistQuery.
/// </summary>
public class GetOphthalmologistQueryHandler : IQueryHandler<GetOphthalmologistQuery, OphthalmologistDto>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public GetOphthalmologistQueryHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
    }

    public async Task<Result<OphthalmologistDto>> Handle(GetOphthalmologistQuery request, CancellationToken cancellationToken)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdWithCertificatesAsync(request.Id, cancellationToken);

        if (ophthalmologist is null)
        {
            return Result<OphthalmologistDto>.NotFound($"Ophthalmologist with ID '{request.Id}' was not found.");
        }

        // Get user information
        var user = await _identityService.GetUserByIdAsync(ophthalmologist.UserId, cancellationToken);
        var userDetails = await _identityService.GetUserDetailsAsync(ophthalmologist.UserId, cancellationToken);

        var dto = new OphthalmologistDto
        {
            Id = ophthalmologist.Id,
            UserId = ophthalmologist.UserId,
            UserFullName = user?.FullName,
            UserEmail = user?.Email,
            UserPhoneNumber = userDetails?.PhoneNumber,
            UserAddress = userDetails?.Address,
            Bio = ophthalmologist.Bio,
            YearsOfExperience = ophthalmologist.YearsOfExperience,
            IsVerified = ophthalmologist.IsVerified,
            CreatedAt = ophthalmologist.CreatedAt,
            UpdatedAt = ophthalmologist.UpdatedAt,
            Certificates = ophthalmologist.Certificates.Select(c => new CertificateDto
            {
                Id = c.Id,
                Name = c.Name,
                IssuingAuthority = c.IssuingAuthority,
                IssuedDate = c.IssuedDate,
                ExpiryDate = c.ExpiryDate,
                CertificateUrl = c.CertificateUrl,
                IsExpired = c.IsExpired
            }).ToList()
        };

        return Result<OphthalmologistDto>.Success(dto);
    }
}
