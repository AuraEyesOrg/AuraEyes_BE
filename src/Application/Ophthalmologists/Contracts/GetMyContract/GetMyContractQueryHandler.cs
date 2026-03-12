using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Contracts.Common;
using Domain.Entities.Contracts;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Application.Ophthalmologists.Contracts.GetMyContract;

public class GetMyContractQueryHandler : IQueryHandler<GetMyContractQuery, ContractDetailDto>
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractTemplateRepository _templateRepository;
    private readonly IIdentityService _identityService;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly ILogger<GetMyContractQueryHandler> _logger;

    public GetMyContractQueryHandler(
        IContractRepository contractRepository,
        IContractTemplateRepository templateRepository,
        IIdentityService identityService,
        IOphthalmologistRepository ophthalmologistRepository,
        ILogger<GetMyContractQueryHandler> logger)
    {
        _contractRepository = contractRepository;
        _templateRepository = templateRepository;
        _identityService = identityService;
        _ophthalmologistRepository = ophthalmologistRepository;
        _logger = logger;
    }

    public async Task<Result<ContractDetailDto>> Handle(
        GetMyContractQuery request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (contract is null)
            return Result<ContractDetailDto>.NotFound("No contract found for this user.");

        var template = await _templateRepository.GetByIdWithVariablesAsync(contract.TemplateId, cancellationToken);
        var user = await _identityService.GetUserByIdAsync(contract.UserId, cancellationToken);
        var userDetails = await _identityService.GetUserDetailsAsync(contract.UserId, cancellationToken);
        var ophthalmologist = await _ophthalmologistRepository.GetByUserIdAsync(contract.UserId, cancellationToken);

        var resolvedContent = template?.ContentTemplate is not null
            ? SubstituteVariables(template.ContentTemplate, contract, user, userDetails, ophthalmologist)
            : null;

        var dto = new ContractDetailDto
        {
            Id = contract.Id,
            ContractNumber = contract.ContractNumber,
            Status = contract.Status.ToString(),
            TemplateId = contract.TemplateId,
            TemplateTitle = template?.Title ?? string.Empty,
            ContractType = template?.Type.ToString() ?? string.Empty,
            UserId = contract.UserId,
            UserFullName = user?.FullName ?? string.Empty,
            UserEmail = user?.Email ?? string.Empty,
            AiQuotaLimit = contract.AiQuotaLimit,
            PlatformCommissionRate = contract.PlatformCommissionRate,
            SignedDate = contract.SignedDate,
            ScannedDocumentUrl = contract.ScannedDocumentUrl,
            SignedContent = resolvedContent,
            CreatedAt = contract.CreatedAt,
            UpdatedAt = contract.UpdatedAt
        };

        return Result<ContractDetailDto>.Success(dto);
    }

    private static string SubstituteVariables(
        string html,
        Contract contract,
        UserDto? user,
        UserDetailsDto? userDetails,
        Ophthalmologist? ophthalmologist)
    {
        var createdAt = contract.CreatedAt;
        var phone = ophthalmologist?.Phone ?? userDetails?.PhoneNumber ?? string.Empty;
        var address = userDetails?.Address ?? string.Empty;
        var dob = userDetails?.DateOfBirth?.ToString("dd/MM/yyyy") ?? string.Empty;
        var commissionRate = (contract.PlatformCommissionRate * 100).ToString("0.##");

        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["doctor_name"]      = user?.FullName ?? string.Empty,
            ["email"]            = user?.Email ?? string.Empty,
            ["contract_code"]    = contract.ContractNumber,
            ["commission_rate"]  = commissionRate,
            ["day"]              = createdAt.Day.ToString(),
            ["month"]            = createdAt.Month.ToString(),
            ["year"]             = createdAt.Year.ToString(),
            ["issue_date"]       = createdAt.ToString("dd/MM/yyyy"),
            ["phone"]            = phone,
            ["dob"]              = dob,
            ["permanent_address"]= address,
            // Fields filled in manually on the printed copy
            ["citizen_id"]       = string.Empty,
            ["bank_name"]        = string.Empty,
            ["bank_account"]     = string.Empty,
            ["license_number"]   = string.Empty,
            ["signature_url"]    = contract.ScannedDocumentUrl ?? string.Empty,
        };

        return Regex.Replace(html, @"\{\{(\w+)\}\}", m =>
            values.TryGetValue(m.Groups[1].Value, out var v) ? v : m.Value);
    }
}
