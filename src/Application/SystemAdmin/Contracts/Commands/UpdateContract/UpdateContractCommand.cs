using Application.Common.Interfaces;
using Application.SystemAdmin.Contracts.Common;

namespace Application.SystemAdmin.Contracts.Commands.UpdateContract;

/// <summary>Update commercial terms of a Draft contract.</summary>
public record UpdateContractCommand : ICommand<ContractDto>
{
    public Guid Id { get; init; }
    public Guid TemplateId { get; init; }
    public int AiQuotaLimit { get; init; }
    public decimal PlatformCommissionRate { get; init; }
}
