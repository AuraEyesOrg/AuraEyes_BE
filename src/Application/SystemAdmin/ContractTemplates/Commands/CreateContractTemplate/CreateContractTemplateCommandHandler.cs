using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.ContractTemplates.Common;
using Domain.Common;
using Domain.Entities.Contracts;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.ContractTemplates.Commands.CreateContractTemplate;

public class CreateContractTemplateCommandHandler
    : ICommandHandler<CreateContractTemplateCommand, ContractTemplateDetailDto>
{
    private readonly IContractTemplateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateContractTemplateCommandHandler> _logger;

    public CreateContractTemplateCommandHandler(
        IContractTemplateRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateContractTemplateCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ContractTemplateDetailDto>> Handle(
        CreateContractTemplateCommand request,
        CancellationToken cancellationToken)
    {
        var versionConflict = await _repository.ExistsByTypeAndVersionAsync(
            request.Type, request.ContractVersion, cancellationToken: cancellationToken);

        if (versionConflict)
            return Result<ContractTemplateDetailDto>.Conflict(
                $"A template of type '{request.Type}' with version '{request.ContractVersion}' already exists.");

        var template = new ContractTemplate(
            request.Title,
            request.Type,
            request.ContractVersion,
            request.ContentTemplate,
            request.EffectiveDate);

        await _repository.AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Contract template '{Title}' (v{Version}) created with ID {Id}",
            template.Title, template.ContractVersion, template.Id);

        return Result<ContractTemplateDetailDto>.Success(ToDetailDto(template));
    }

    internal static ContractTemplateDetailDto ToDetailDto(ContractTemplate t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Type = t.Type.ToString(),
        ContractVersion = t.ContractVersion,
        IsActive = t.IsActive,
        ContentTemplate = t.ContentTemplate,
        EffectiveDate = t.EffectiveDate,
        VariableCount = 0,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    };
}
