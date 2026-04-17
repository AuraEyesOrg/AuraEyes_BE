using Application.Common.Interfaces;

namespace Application.SystemAdmin.Ophthalmologists.Commands.RejectLeaveRequest;

public record RejectLeaveRequestCommand : ICommand
{
    public Guid LeaveRequestId { get; init; }
    public Guid ReviewedByAdminUserId { get; init; }
    public string? AdminNote { get; init; }
}
