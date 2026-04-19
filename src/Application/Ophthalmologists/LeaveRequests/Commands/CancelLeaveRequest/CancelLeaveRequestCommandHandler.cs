using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.LeaveRequests.Commands.CancelLeaveRequest;

public class CancelLeaveRequestCommandHandler : ICommandHandler<CancelLeaveRequestCommand>
{
    private readonly IOphthalmologistLeaveRequestRepository _leaveRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelLeaveRequestCommandHandler(
        IOphthalmologistLeaveRequestRepository leaveRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.LeaveRequestId, cancellationToken);
        if (leaveRequest is null)
        {
            return Result.NotFound($"Leave request '{request.LeaveRequestId}' was not found.");
        }

        if (leaveRequest.OphthalmologistId != request.OphthalmologistId)
        {
            return Result.Forbidden("You can only cancel your own leave request.");
        }

        try
        {
            leaveRequest.Cancel(request.OphthalmologistId);
            await _leaveRequestRepository.UpdateAsync(leaveRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }
    }
}
