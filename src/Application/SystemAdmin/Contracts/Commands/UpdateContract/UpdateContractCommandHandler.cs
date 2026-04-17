using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Contracts.Commands.CreateContract;
using Application.SystemAdmin.Contracts.Common;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Contracts.Commands.UpdateContract;

public class UpdateContractCommandHandler : ICommandHandler<UpdateContractCommand, ContractDto>
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractTemplateRepository _templateRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateContractCommandHandler> _logger;

    public UpdateContractCommandHandler(
        IContractRepository contractRepository,
        IContractTemplateRepository templateRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        ILogger<UpdateContractCommandHandler> logger)
    {
        _contractRepository = contractRepository;
        _templateRepository = templateRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ContractDto>> Handle(
        UpdateContractCommand request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.Id, cancellationToken);
        if (contract is null)
            return Result<ContractDto>.NotFound($"Contract {request.Id} not found.");

        var template = await _templateRepository.GetByIdAsync(request.TemplateId, cancellationToken);
        if (template is null)
            return Result<ContractDto>.NotFound($"Contract template {request.TemplateId} not found.");

        contract.Update(request.TemplateId, request.AiQuotaLimit, request.PlatformCommissionRate, request.MonthlyQuotaLimit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var user = await _identityService.GetUserByIdAsync(contract.UserId, cancellationToken);

        _logger.LogInformation("Contract {Id} updated.", contract.Id);

        return Result<ContractDto>.Success(CreateContractCommandHandler.ToDto(
            contract,
            template.Title,
            template.Type.ToString(),
            user?.FullName ?? string.Empty,
            user?.Email ?? string.Empty));
    }
}
