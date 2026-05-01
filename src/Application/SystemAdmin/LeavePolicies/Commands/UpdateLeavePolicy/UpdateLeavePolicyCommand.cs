using Application.Common.Interfaces;

namespace Application.SystemAdmin.LeavePolicies.Commands.UpdateLeavePolicy;

public record UpdateLeavePolicyCommand : ICommand
{
    public Guid PolicyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal AdditionalDays { get; init; }
    public string? Description { get; init; }
}
