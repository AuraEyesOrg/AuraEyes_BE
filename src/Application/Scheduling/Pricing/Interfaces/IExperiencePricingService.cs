using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;

namespace Application.Scheduling.Pricing.Interfaces;

public interface IExperiencePricingService
{
    Task<Result<AllowedPriceRangeDto>> GetAllowedPriceRangeAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default);

    Task<Result> ValidatePartTimeCostAsync(
        Guid ophthalmologistId,
        decimal? cost,
        CancellationToken cancellationToken = default);
}