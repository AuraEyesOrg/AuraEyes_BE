using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.Queries.GetOphthalmologists;

/// <summary>
/// Handler for GetOphthalmologistsQuery.
/// </summary>
public class GetOphthalmologistsQueryHandler : IQueryHandler<GetOphthalmologistsQuery, PagedResult<OphthalmologistListDto>>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IIdentityService _identityService;

    public GetOphthalmologistsQueryHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IIdentityService identityService)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<OphthalmologistListDto>>> Handle(
        GetOphthalmologistsQuery request, 
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _ophthalmologistRepository.GetPagedAsync(
            request.SearchTerm,
            request.IsVerified,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = new List<OphthalmologistListDto>();

        foreach (var ophthalmologist in items)
        {
            var user = await _identityService.GetUserByIdAsync(ophthalmologist.UserId, cancellationToken);
            
            var slots = await _appointmentSlotRepository.GetByOphthalmologistAsync(
                ophthalmologist.Id, 
                DateOnly.FromDateTime(DateTime.UtcNow), 
                null, 
                Domain.Enums.ScheduleStatus.Available, 
                cancellationToken);
                
            decimal? minPrice = slots.Any() ? slots.Min(s => s.Cost) : null;
            decimal? maxPrice = slots.Any() ? slots.Max(s => s.Cost) : null;

            dtoList.Add(new OphthalmologistListDto
            {
                Id = ophthalmologist.Id,
                UserId = ophthalmologist.UserId,
                UserFullName = user?.FullName,
                UserEmail = user?.Email,
                UserAvatarUrl = user?.AvatarUrl,
                Bio = ophthalmologist.Bio,
                YearsOfExperience = ophthalmologist.YearsOfExperience,
                IsVerified = ophthalmologist.IsVerified,
                CertificateCount = ophthalmologist.Certificates.Count,
                CreatedAt = ophthalmologist.CreatedAt,
                LicenseUrl = ophthalmologist.LicenseUrl,
                DegreeUrl = ophthalmologist.DegreeUrl,
                RatingAverage = ophthalmologist.RatingAverage,
                RatingCount = ophthalmologist.RatingCount,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            });
        }

        var pagedResult = new PagedResult<OphthalmologistListDto>(
            dtoList, 
            totalCount, 
            request.PageNumber, 
            request.PageSize);

        return Result<PagedResult<OphthalmologistListDto>>.Success(pagedResult);
    }
}
