using Application.Common.Interfaces;

namespace Application.Ophthalmologists.LeaveRequests.Commands.CreateLeaveRequest;

public record CreateLeaveRequestCommand : ICommand<Guid>
{
    public Guid OphthalmologistId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public string Reason { get; init; } = string.Empty;
}
