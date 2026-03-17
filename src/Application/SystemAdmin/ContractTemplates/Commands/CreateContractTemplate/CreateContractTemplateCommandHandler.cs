using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.ContractTemplates.Common;
using Domain.Common;
using Domain.Entities.Contracts;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

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

        var effectiveDateUtc = NormalizeToUtc(request.EffectiveDate);

        var template = new ContractTemplate(
            request.Title,
            request.Type,
            request.ContractVersion,
            request.ContentTemplate,
            effectiveDateUtc);

        await _repository.AddAsync(template, cancellationToken);
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
            "Contract template '{Title}' (v{Version}) created with ID {Id}",
            template.Title, template.ContractVersion, template.Id);

        return Result<ContractTemplateDetailDto>.Success(ToDetailDto(template));
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
