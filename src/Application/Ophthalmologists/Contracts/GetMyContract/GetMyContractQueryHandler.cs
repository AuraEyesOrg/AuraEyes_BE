using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Contracts.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.Contracts.GetMyContract;

public class GetMyContractQueryHandler : IQueryHandler<GetMyContractQuery, ContractDetailDto>
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractTemplateRepository _templateRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public GetMyContractQueryHandler(
        IContractRepository contractRepository,
        IContractTemplateRepository templateRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _contractRepository = contractRepository;
        _templateRepository = templateRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
    }

    public async Task<Result<ContractDetailDto>> Handle(
        GetMyContractQuery request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (contract is null)
            return Result<ContractDetailDto>.NotFound("No contract found for this user.");

        var template = await _templateRepository.GetByIdAsync(contract.TemplateId, cancellationToken);
        var user = await _identityService.GetUserByIdAsync(contract.UserId, cancellationToken);
        var ophthalmologist = await _ophthalmologistRepository.GetByUserIdAsync(contract.UserId, cancellationToken);

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
            MonthlyQuotaLimit = contract.MonthlyQuotaLimit,
            PlatformCommissionRate = contract.PlatformCommissionRate,
            CommissionRate = ophthalmologist?.CommissionRate,
            ActualMonthlySalary = ophthalmologist?.ActualMonthlySalary,
            SignedDate = contract.SignedDate,
            ScannedDocumentUrl = contract.ScannedDocumentUrl,
            SignedContent = template?.ContentTemplate,
            CreatedAt = contract.CreatedAt,
            UpdatedAt = contract.UpdatedAt
        };

        return Result<ContractDetailDto>.Success(dto);
    }
}
