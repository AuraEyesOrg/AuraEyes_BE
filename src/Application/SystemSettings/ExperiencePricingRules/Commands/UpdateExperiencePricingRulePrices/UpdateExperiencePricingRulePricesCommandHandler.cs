using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemSettings.ExperiencePricingRules.Common;
using Domain.Common;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.SystemSettings.ExperiencePricingRules.Commands.UpdateExperiencePricingRulePrices;

public class UpdateExperiencePricingRulePricesCommandHandler
    : ICommandHandler<UpdateExperiencePricingRulePricesCommand, IReadOnlyList<ExperiencePricingRuleDto>>
{
    private readonly IExperiencePricingRuleRepository _experiencePricingRuleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExperiencePricingRulePricesCommandHandler(
        IExperiencePricingRuleRepository experiencePricingRuleRepository,
        IUnitOfWork unitOfWork)
    {
        _experiencePricingRuleRepository = experiencePricingRuleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<ExperiencePricingRuleDto>>> Handle(
        UpdateExperiencePricingRulePricesCommand request,
        CancellationToken cancellationToken)
    {
        var requestedIds = request.Rules.Select(r => r.Id).Distinct().ToList();

        var rules = await _experiencePricingRuleRepository.Query()
            .Where(r => requestedIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        if (rules.Count != requestedIds.Count)
        {
            return Result<IReadOnlyList<ExperiencePricingRuleDto>>.NotFound(
                "One or more experience pricing rules were not found.");
        }

        foreach (var updateItem in request.Rules)
        {
            var rule = rules.First(r => r.Id == updateItem.Id);
            rule.Update(
                rule.MinYearsExperience,
                rule.MaxYearsExperience,
                updateItem.MinPrice,
                updateItem.MaxPrice);

            await _experiencePricingRuleRepository.UpdateAsync(rule, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = rules
            .OrderBy(r => r.MinYearsExperience)
            .Select(r => new ExperiencePricingRuleDto
            {
                Id = r.Id,
                MinYearsExperience = r.MinYearsExperience,
                MaxYearsExperience = r.MaxYearsExperience,
                MinPrice = r.MinPrice,
                MaxPrice = r.MaxPrice,
                IsActive = r.IsActive
            })
            .ToList();

        return Result<IReadOnlyList<ExperiencePricingRuleDto>>.Success(updated);
    }
}