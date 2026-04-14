using Application.Common.Interfaces;

namespace Application.SystemAdmin.Ophthalmologists.Commands.ApproveLeaveRequest;

public record ApproveLeaveRequestCommand : ICommand<ApproveLeaveRequestResultDto>
{
    public Guid LeaveRequestId { get; init; }
    public Guid ReviewedByAdminUserId { get; init; }
    public string? AdminNote { get; init; }
}

public record ApproveLeaveRequestResultDto
{
    public Guid LeaveRequestId { get; init; }
    public int CancelledConsultationSessions { get; init; }
    public int CancelledAppointments { get; init; }
    public int BlockedSlots { get; init; }
}
