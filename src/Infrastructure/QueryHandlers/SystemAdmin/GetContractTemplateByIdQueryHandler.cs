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
            VariableCount = 0,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };

        return Result<ContractTemplateDetailDto>.Success(dto);
    }
}
