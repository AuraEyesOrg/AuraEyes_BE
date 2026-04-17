using Application.Common.Interfaces;

namespace Application.Ophthalmologists.LeaveRequests.Commands.CancelLeaveRequest;

public record CancelLeaveRequestCommand : ICommand
{
    public Guid LeaveRequestId { get; init; }
    public Guid OphthalmologistId { get; init; }
}
