using Application.Common.Interfaces;

namespace Application.SystemAdmin.LeavePolicies.Commands.DeleteLeavePolicy;

public record DeleteLeavePolicyCommand : ICommand
{
    public Guid PolicyId { get; init; }
}
