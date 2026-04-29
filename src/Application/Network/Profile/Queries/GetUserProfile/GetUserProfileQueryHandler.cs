using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Network.Profile.Queries.GetUserProfile;

/// <summary>
/// Handler for GetUserProfileQuery.
/// Aggregates ApplicationUser info, Ophthalmologist profile, and post count.
/// </summary>
public class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IIdentityService _identityService;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IPostRepository _postRepository;

    public GetUserProfileQueryHandler(
        IIdentityService identityService,
        IOphthalmologistRepository ophthalmologistRepository,
        IPostRepository postRepository)
    {
        _identityService = identityService;
        _ophthalmologistRepository = ophthalmologistRepository;
        _postRepository = postRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<UserProfileDto>.Failure("User not found.");

        // Ophthalmologist profile is optional (user may not have one yet)
        var ophthalmologist = await _ophthalmologistRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        string? bio = ophthalmologist?.Bio;
        // In Digital Clinic model there is no separate Organisation entity

        var postCount = await _postRepository.GetPostCountByAuthorAsync(request.UserId, cancellationToken);

        var dto = new UserProfileDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            Bio = bio,
            PostCount = postCount,
            Certificates = ophthalmologist?.Certificates
                .Select(c => new UserProfileCertificateDto
                {
                    Id = c.Id,
                    Type = c.Type.ToString(),
                    DegreeLevel = c.DegreeLevel?.ToString(),
                    Name = c.Name,
                    IssuingAuthority = c.IssuingAuthority,
                    IssuedDate = c.IssuedDate,
                    ExpiryDate = c.ExpiryDate,
                    CertificateUrl = c.CertificateUrl
                })
                .OrderByDescending(c => c.IssuedDate)
                .ToList() ?? new List<UserProfileCertificateDto>()
        };

        return Result<UserProfileDto>.Success(dto);
    }
}
