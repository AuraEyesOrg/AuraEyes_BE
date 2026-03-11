using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.ContractTemplates.Common;
using Application.SystemAdmin.ContractTemplates.Queries.GetContractTemplateById;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers.SystemAdmin;

public class GetContractTemplateByIdQueryHandler
    : IQueryHandler<GetContractTemplateByIdQuery, ContractTemplateDetailDto>
{
    private readonly ApplicationDbContext _context;

    public GetContractTemplateByIdQueryHandler(ApplicationDbContext context) => _context = context;

    public async Task<Result<ContractTemplateDetailDto>> Handle(
        GetContractTemplateByIdQuery request,
        CancellationToken cancellationToken)
    {
        var template = await _context.ContractTemplates
            .AsNoTracking()
            .Include(t => t.Variables.Where(v => !v.IsDeleted))
            .Where(t => t.Id == request.Id && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (template is null)
            return Result<ContractTemplateDetailDto>.NotFound($"Contract template {request.Id} not found.");

        var dto = new ContractTemplateDetailDto
        {
            Id = template.Id,
            Title = template.Title,
            Type = template.Type.ToString(),
            ContractVersion = template.ContractVersion,
            IsActive = template.IsActive,
            ContentTemplate = template.ContentTemplate,
            EffectiveDate = template.EffectiveDate,
            VariableCount = template.Variables.Count,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt,
            Variables = template.Variables
                .OrderBy(v => v.SortOrder)
                .Select(v => new ContractTemplateVariableDto
                {
                    Id = v.Id,
                    Key = v.Key,
                    Label = v.Label,
                    VariableType = v.VariableType.ToString(),
                    Description = v.Description,
                    DefaultValue = v.DefaultValue,
                    SelectOptions = v.SelectOptions,
                    Unit = v.Unit,
                    IsRequired = v.IsRequired,
                    SortOrder = v.SortOrder
                })
                .ToList()
        };

        return Result<ContractTemplateDetailDto>.Success(dto);
    }
}
