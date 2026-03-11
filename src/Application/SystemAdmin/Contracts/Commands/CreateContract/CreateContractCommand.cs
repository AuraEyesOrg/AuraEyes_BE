using Application.Common.Interfaces;
using Application.SystemAdmin.Contracts.Common;

namespace Application.SystemAdmin.Contracts.Commands.CreateContract;

public record CreateContractCommand : ICommand<ContractDto>
{
    public Guid UserId { get; init; }
    public Guid TemplateId { get; init; }
    public string ContractNumber { get; init; } = string.Empty;
    public int AiQuotaLimit { get; init; }
    public decimal PlatformCommissionRate { get; init; }
}
