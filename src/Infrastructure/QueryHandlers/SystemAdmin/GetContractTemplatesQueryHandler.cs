using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.ContractTemplates.Common;
using Application.SystemAdmin.ContractTemplates.Queries.GetContractTemplates;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers.SystemAdmin;

public class GetContractTemplatesQueryHandler
    : IQueryHandler<GetContractTemplatesQuery, PagedResult<ContractTemplateDto>>
{
    private readonly ApplicationDbContext _context;

    public GetContractTemplatesQueryHandler(ApplicationDbContext context) => _context = context;

    public async Task<Result<PagedResult<ContractTemplateDto>>> Handle(
        GetContractTemplatesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.ContractTemplates
            .AsNoTracking()
            .Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(term));
        }

        if (request.Type.HasValue)
            query = query.Where(t => t.Type == request.Type.Value);

        if (request.IsActive.HasValue)
            query = query.Where(t => t.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new ContractTemplateDto
            {
                Id = t.Id,
                Title = t.Title,
                Type = t.Type.ToString(),
                ContractVersion = t.ContractVersion,
                IsActive = t.IsActive,
                EffectiveDate = t.EffectiveDate,
                VariableCount = _context.ContractTemplateVariables
                    .Count(v => v.TemplateId == t.Id && !v.IsDeleted),
                UsageCount = _context.Contracts
                    .Count(c => c.TemplateId == t.Id && !c.IsDeleted),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ContractTemplateDto>>.Success(
            new PagedResult<ContractTemplateDto>(items, totalCount, request.PageNumber, request.PageSize));
    }
}
