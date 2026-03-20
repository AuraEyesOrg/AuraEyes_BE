using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.ContractTemplates.Commands.CreateContractTemplate;
using Application.SystemAdmin.ContractTemplates.Common;
using Domain.Common;
using Domain.Entities.Contracts;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Application.SystemAdmin.ContractTemplates.Commands.DuplicateContractTemplate;

public class DuplicateContractTemplateCommandHandler
    : ICommandHandler<DuplicateContractTemplateCommand, ContractTemplateDetailDto>
{
    private readonly IContractTemplateRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DuplicateContractTemplateCommandHandler> _logger;

    public DuplicateContractTemplateCommandHandler(
        IContractTemplateRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DuplicateContractTemplateCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ContractTemplateDetailDto>> Handle(
        DuplicateContractTemplateCommand request,
        CancellationToken cancellationToken)
    {
        var source = await _repository.GetByIdAsync(request.SourceId, cancellationToken);
        if (source is null)
            return Result<ContractTemplateDetailDto>.NotFound(
                $"Source contract template {request.SourceId} not found.");

        // Build a unique version string: "v1.0" → "v1.0-copy", "v1.0-copy-2", …
        var baseVersion = source.ContractVersion;
        var candidateVersion = $"{baseVersion}-copy";
        var attempt = 1;
        while (await _repository.ExistsByTypeAndVersionAsync(
                   source.Type, candidateVersion, cancellationToken: cancellationToken))
        {
            attempt++;
            candidateVersion = $"{baseVersion}-copy-{attempt}";
        }

        var clone = new ContractTemplate(
            $"{source.Title} (Copy)",
            source.Type,
            candidateVersion,
            source.ContentTemplate,
            source.EffectiveDate,
            source.EmploymentType);

        await _repository.AddAsync(clone, cancellationToken);
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsVersionUniqueConstraintViolation(ex))
        {
            return Result<ContractTemplateDetailDto>.Conflict(
                $"A template of type '{source.Type}' with version '{candidateVersion}' already exists.");
        }

        _logger.LogInformation(
            "Contract template {SourceId} duplicated as {NewId} (v{Version})",
            request.SourceId, clone.Id, clone.ContractVersion);

        return Result<ContractTemplateDetailDto>.Success(
            CreateContractTemplateCommandHandler.ToDetailDto(clone));
    }

    private static bool IsVersionUniqueConstraintViolation(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("IX_ContractTemplates_Type_ContractVersion", StringComparison.OrdinalIgnoreCase) == true;
}
