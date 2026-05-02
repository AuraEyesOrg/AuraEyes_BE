using Application.Common.Interfaces;

namespace Application.SystemAdmin.LeavePolicies.Commands.ApplyLeavePolicy;

public record ApplyLeavePolicyCommand : ICommand
{
    public Guid PolicyId { get; init; }
    public Guid OphthalmologistId { get; init; }
}
