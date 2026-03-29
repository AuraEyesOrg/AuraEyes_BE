using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Contracts.Common;
using Domain.Common;
using Domain.Entities.Contracts;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Contracts.Commands.CreateContract;

public class CreateContractCommandHandler : ICommandHandler<CreateContractCommand, ContractDto>
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractTemplateRepository _templateRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateContractCommandHandler> _logger;

    public CreateContractCommandHandler(
        IContractRepository contractRepository,
        IContractTemplateRepository templateRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        ILogger<CreateContractCommandHandler> logger)
    {
        _contractRepository = contractRepository;
        _templateRepository = templateRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ContractDto>> Handle(
        CreateContractCommand request,
        CancellationToken cancellationToken)
    {
        // Validate user exists
        var user = await _identityService.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<ContractDto>.NotFound($"User {request.UserId} not found.");

        // Validate template exists
        var template = await _templateRepository.GetByIdAsync(request.TemplateId, cancellationToken);
        if (template is null)
            return Result<ContractDto>.NotFound($"Contract template {request.TemplateId} not found.");

        // Ensure contract number is unique
        var numberConflict = await _contractRepository.ExistsByContractNumberAsync(
            request.ContractNumber, cancellationToken: cancellationToken);
        if (numberConflict)
            return Result<ContractDto>.Conflict($"Contract number '{request.ContractNumber}' is already in use.");

        var contract = new Contract(
            request.UserId,
            request.TemplateId,
            request.ContractNumber,
            request.AiQuotaLimit,
            request.PlatformCommissionRate);

        await _contractRepository.AddAsync(contract, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contract {Number} created with ID {Id}", contract.ContractNumber, contract.Id);

        return Result<ContractDto>.Success(ToDto(contract, template.Title, template.Type.ToString(), user.FullName, user.Email));
    }

    internal static ContractDto ToDto(
        Contract c,
        string templateTitle,
        string contractType,
        string userFullName,
        string userEmail,
        decimal? commissionRate = null,
        decimal? actualMonthlySalary = null) => new()
        {
            Id = c.Id,
            ContractNumber = c.ContractNumber,
            Status = c.Status.ToString(),
            TemplateId = c.TemplateId,
            TemplateTitle = templateTitle,
            ContractType = contractType,
            UserId = c.UserId,
            UserFullName = userFullName,
            UserEmail = userEmail,
            AiQuotaLimit = c.AiQuotaLimit,
            PlatformCommissionRate = c.PlatformCommissionRate,
            CommissionRate = commissionRate,
            ActualMonthlySalary = actualMonthlySalary,
            SignedDate = c.SignedDate,
            ScannedDocumentUrl = c.ScannedDocumentUrl,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
}
