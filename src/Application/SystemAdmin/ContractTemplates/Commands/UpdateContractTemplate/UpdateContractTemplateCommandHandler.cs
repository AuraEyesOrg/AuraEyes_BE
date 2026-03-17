using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.ContractTemplates.Commands.CreateContractTemplate;
using Application.SystemAdmin.ContractTemplates.Common;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

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

        var effectiveDateUtc = NormalizeToUtc(request.EffectiveDate);

        template.Update(request.Title, request.Type, request.ContractVersion, request.ContentTemplate, effectiveDateUtc);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsVersionUniqueConstraintViolation(ex))
        {
            return Result<ContractTemplateDetailDto>.Conflict(
                $"A template of type '{request.Type}' with version '{request.ContractVersion}' already exists.");
        }

        _logger.LogInformation(
            "Contract template {Id} updated to version '{Version}'",
            template.Id, template.ContractVersion);

        return Result<ContractTemplateDetailDto>.Success(
            CreateContractTemplateCommandHandler.ToDetailDto(template));
    }

    private static DateTime? NormalizeToUtc(DateTime? value)
    {
        if (!value.HasValue)
            return null;

        return value.Value.Kind switch
        {
            DateTimeKind.Utc => value.Value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        };
    }

    private static bool IsVersionUniqueConstraintViolation(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("IX_ContractTemplates_Type_ContractVersion", StringComparison.OrdinalIgnoreCase) == true;
}
