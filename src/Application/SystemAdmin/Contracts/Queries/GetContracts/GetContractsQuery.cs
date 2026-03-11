using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Contracts.Common;
using Domain.Enums;

namespace Application.SystemAdmin.Contracts.Queries.GetContracts;

public record GetContractsQuery : IQuery<PagedResult<ContractDto>>
{
    public string? SearchTerm { get; init; }
    public Guid? UserId { get; init; }
    public ContractStatus? Status { get; init; }
    public ContractType? ContractType { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
