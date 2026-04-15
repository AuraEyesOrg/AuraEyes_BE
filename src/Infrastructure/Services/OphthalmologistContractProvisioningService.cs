using Application.SystemAdmin.Ophthalmologists.Interfaces;
using Domain.Entities.Contracts;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class OphthalmologistContractProvisioningService : IOphthalmologistContractProvisioningService
{
    private readonly IContractTemplateRepository _templateRepository;
    private readonly ILogger<OphthalmologistContractProvisioningService> _logger;

    public OphthalmologistContractProvisioningService(
        IContractTemplateRepository templateRepository,
        ILogger<OphthalmologistContractProvisioningService> logger)
    {
        _templateRepository = templateRepository;
        _logger = logger;
    }

    public async Task<Contract?> CreatePendingContractForEmploymentTypeAsync(
        Ophthalmologist ophthalmologist,
        CancellationToken cancellationToken = default)
    {
        var (templates, _) = await _templateRepository.GetPagedAsync(
            type: ContractType.OphthalmologistContract,
            isActive: true,
            pageNumber: 1,
            pageSize: 50,
            cancellationToken: cancellationToken);

        var template = SelectTemplateByEmploymentType(templates, ophthalmologist.EmploymentType);
        if (template == null)
        {
            _logger.LogWarning(
                "No active ophthalmologist contract template found for employment type {EmploymentType}.",
                ophthalmologist.EmploymentType);
            return null;
        }

        var contractNumber = $"AURA-OPH-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

        var contract = new Contract(
            ophthalmologist.UserId,
            template.Id,
            contractNumber,
            aiQuotaLimit: 0,
            platformCommissionRate: 0m);

        contract.SendForSignature();
        return contract;
    }

    private static ContractTemplate? SelectTemplateByEmploymentType(
        IReadOnlyList<ContractTemplate> templates,
        OphthalmologistEmploymentType employmentType)
    {
        if (templates.Count == 0)
            return null;

        static string Normalize(string value) => value.ToLowerInvariant().Replace("-", string.Empty).Replace(" ", string.Empty);

        var expectedKeyword = employmentType == OphthalmologistEmploymentType.PartTime
            ? "parttime"
            : "fulltime";

        var matched = templates
            .Where(t => t.EmploymentType == employmentType)
            .OrderByDescending(t => t.EffectiveDate ?? DateTime.MinValue)
            .ThenByDescending(t => t.CreatedAt)
            .FirstOrDefault();

        if (matched != null)
            return matched;

        return templates
            .Where(t => Normalize(t.Title).Contains(expectedKeyword))
            .OrderByDescending(t => t.EffectiveDate ?? DateTime.MinValue)
            .ThenByDescending(t => t.CreatedAt)
            .FirstOrDefault();
    }
}
