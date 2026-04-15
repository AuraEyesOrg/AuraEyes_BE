using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.EmploymentTypeChangeRequests.Commands.CancelEmploymentTypeChangeRequest;

public class CancelEmploymentTypeChangeRequestCommandHandler : ICommandHandler<CancelEmploymentTypeChangeRequestCommand>
{
    private readonly IOphthalmologistEmploymentTypeChangeRequestRepository _requestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelEmploymentTypeChangeRequestCommandHandler(
        IOphthalmologistEmploymentTypeChangeRequestRepository requestRepository,
        IUnitOfWork unitOfWork)
    {
        _requestRepository = requestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelEmploymentTypeChangeRequestCommand request, CancellationToken cancellationToken)
    {
        var changeRequest = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (changeRequest is null)
        {
            return Result.NotFound($"Employment type change request '{request.RequestId}' was not found.");
        }

        if (changeRequest.OphthalmologistId != request.OphthalmologistId)
        {
            return Result.Forbidden("You can only cancel your own employment type change request.");
        }

        try
        {
            changeRequest.Cancel(request.OphthalmologistId);
            await _requestRepository.UpdateAsync(changeRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }
    }
}
