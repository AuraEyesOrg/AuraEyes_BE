using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.ContractTemplates.Common;
using Domain.Enums;

namespace Application.SystemAdmin.ContractTemplates.Queries.GetContractTemplates;

public record GetContractTemplatesQuery : IQuery<PagedResult<ContractTemplateDto>>
{
    public string? SearchTerm { get; init; }
    public ContractType? Type { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
