using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Application.Scheduling.Pricing.Interfaces;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Pricing.Services;

public class ExperiencePricingService : IExperiencePricingService
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IExperiencePricingRuleRepository _experiencePricingRuleRepository;

    public ExperiencePricingService(
        IOphthalmologistRepository ophthalmologistRepository,
        IExperiencePricingRuleRepository experiencePricingRuleRepository)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _experiencePricingRuleRepository = experiencePricingRuleRepository;
    }

    public async Task<Result<AllowedPriceRangeDto>> GetAllowedPriceRangeAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default)
    {
        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(ophthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result<AllowedPriceRangeDto>.NotFound($"Ophthalmologist '{ophthalmologistId}' not found.");
        }

        if (ophthalmologist.EmploymentType != OphthalmologistEmploymentType.PartTime)
        {
            return Result<AllowedPriceRangeDto>.Forbidden(
                "Experience-based pricing rules only apply to part-time ophthalmologists.");
        }

        var rule = await _experiencePricingRuleRepository.GetByYearsOfExperienceAsync(
            ophthalmologist.YearsOfExperience,
            cancellationToken);

        if (rule is null)
        {
            return Result<AllowedPriceRangeDto>.Failure(
                $"No active experience pricing rule found for {ophthalmologist.YearsOfExperience} years of experience.");
        }

        return Result<AllowedPriceRangeDto>.Success(new AllowedPriceRangeDto
        {
            OphthalmologistId = ophthalmologist.Id,
            YearsOfExperience = ophthalmologist.YearsOfExperience,
            MinPrice = rule.MinPrice,
            MaxPrice = rule.MaxPrice
        });
    }

    public async Task<Result> ValidatePartTimeCostAsync(
        Guid ophthalmologistId,
        decimal? cost,
        CancellationToken cancellationToken = default)
    {
        if (!cost.HasValue)
        {
            return Result.Failure("Cost is required for part-time ophthalmologists.");
        }

        if (cost.Value <= 0)
        {
            return Result.Failure("Cost must be a positive value.");
        }

        if (decimal.Truncate(cost.Value) != cost.Value)
        {
            return Result.Failure("Cost must be an integer value.");
        }

        var rangeResult = await GetAllowedPriceRangeAsync(ophthalmologistId, cancellationToken);
        if (!rangeResult.IsSuccess || rangeResult.Data is null)
        {
            return rangeResult.IsNotFound
                ? Result.NotFound(rangeResult.ErrorMessage)
                : rangeResult.IsForbidden
                    ? Result.Forbidden(rangeResult.ErrorMessage)
                    : Result.Failure(rangeResult.ErrorMessage);
        }

        var range = rangeResult.Data;
        if (cost.Value < range.MinPrice || cost.Value > range.MaxPrice)
        {
            return Result.Failure(
                $"Cost must be between {range.MinPrice:0} and {range.MaxPrice:0} VND for {range.YearsOfExperience} years of experience.");
        }

        return Result.Success();
    }
}