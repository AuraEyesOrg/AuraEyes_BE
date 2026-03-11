using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Contracts.Commands.CreateContract;
using Application.SystemAdmin.Contracts.Common;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Contracts.Commands.SignContract;

public class SignContractCommandHandler : ICommandHandler<SignContractCommand, ContractDto>
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractTemplateRepository _templateRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SignContractCommandHandler> _logger;

    public SignContractCommandHandler(
        IContractRepository contractRepository,
        IContractTemplateRepository templateRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        ILogger<SignContractCommandHandler> logger)
    {
        _contractRepository = contractRepository;
        _templateRepository = templateRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ContractDto>> Handle(
        SignContractCommand request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.Id, cancellationToken);
        if (contract is null)
            return Result<ContractDto>.NotFound($"Contract {request.Id} not found.");

        try
        {
            contract.Sign(request.SignedContent, request.ScannedDocumentUrl);
        }
        catch (InvalidOperationException ex)
        {
            return Result<ContractDto>.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var template = await _templateRepository.GetByIdAsync(contract.TemplateId, cancellationToken);
        var user = await _identityService.GetUserByIdAsync(contract.UserId, cancellationToken);

        _logger.LogInformation("Contract {Id} signed and activated.", contract.Id);

        return Result<ContractDto>.Success(CreateContractCommandHandler.ToDto(
            contract,
            template?.Title ?? string.Empty,
            template?.Type.ToString() ?? string.Empty,
            user?.FullName ?? string.Empty,
            user?.Email ?? string.Empty));
    }
}
