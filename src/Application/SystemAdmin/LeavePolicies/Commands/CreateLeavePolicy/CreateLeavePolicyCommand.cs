using Application.Common.Interfaces;
using Application.SystemAdmin.LeavePolicies.Common;

namespace Application.SystemAdmin.LeavePolicies.Commands.CreateLeavePolicy;

public record CreateLeavePolicyCommand : ICommand<LeavePolicyDto>
{
    public string Name { get; init; } = string.Empty;
    public decimal AdditionalDays { get; init; }
    public string? Description { get; init; }
}
