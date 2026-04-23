using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Application.Scheduling.Pricing.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAllowedPriceRange;

public class GetAllowedPriceRangeQueryHandler : IQueryHandler<GetAllowedPriceRangeQuery, AllowedPriceRangeDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IExperiencePricingService _experiencePricingService;

    public GetAllowedPriceRangeQueryHandler(
        ICurrentUserService currentUser,
        IExperiencePricingService experiencePricingService)
    {
        _currentUser = currentUser;
        _experiencePricingService = experiencePricingService;
    }

    public async Task<Result<AllowedPriceRangeDto>> Handle(
        GetAllowedPriceRangeQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.IsInRole(Roles.Ophthalmologist))
        {
            if (!_currentUser.ProfileId.HasValue)
            {
                return Result<AllowedPriceRangeDto>.Forbidden(
                    "Unable to resolve ophthalmologist profile from current token.");
            }

            if (_currentUser.ProfileId.Value != request.OphthalmologistId)
            {
                return Result<AllowedPriceRangeDto>.Forbidden(
                    "You are not authorized to access pricing range of another ophthalmologist.");
            }
        }
        else if (!_currentUser.IsInRole(Roles.ClinicStaff) && !_currentUser.IsInRole(Roles.SystemAdmin))
        {
            return Result<AllowedPriceRangeDto>.Forbidden("You are not authorized to access this resource.");
        }

        return await _experiencePricingService.GetAllowedPriceRangeAsync(
            request.OphthalmologistId,
            cancellationToken);
    }
}