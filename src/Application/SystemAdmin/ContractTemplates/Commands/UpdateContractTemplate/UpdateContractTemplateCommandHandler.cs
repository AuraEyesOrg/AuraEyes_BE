using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.ContractTemplates.Commands.CreateContractTemplate;
using Application.SystemAdmin.ContractTemplates.Common;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.ContractTemplates.Commands.UpdateContractTemplate;

public class UpdateContractTemplateCommandHandler
    : ICommandHandler<UpdateContractTemplateCommand, ContractTemplateDetailDto>
{
    private readonly IContractTemplateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateContractTemplateCommandHandler> _logger;

    public UpdateContractTemplateCommandHandler(
        IContractTemplateRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateContractTemplateCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ContractTemplateDetailDto>> Handle(
        UpdateContractTemplateCommand request,
        CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (template is null)
            return Result<ContractTemplateDetailDto>.NotFound($"Contract template {request.Id} not found.");

        // Check version uniqueness (exclude current template)
        var versionConflict = await _repository.ExistsByTypeAndVersionAsync(
            request.Type, request.ContractVersion, request.Id, cancellationToken);

        if (versionConflict)
            return Result<ContractTemplateDetailDto>.Conflict(
                $"A template of type '{request.Type}' with version '{request.ContractVersion}' already exists.");

        template.Update(request.Title, request.Type, request.ContractVersion, request.ContentTemplate, request.EffectiveDate);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Contract template {Id} updated to version '{Version}'",
            template.Id, template.ContractVersion);

        return Result<ContractTemplateDetailDto>.Success(
            CreateContractTemplateCommandHandler.ToDetailDto(template));
    }
}
