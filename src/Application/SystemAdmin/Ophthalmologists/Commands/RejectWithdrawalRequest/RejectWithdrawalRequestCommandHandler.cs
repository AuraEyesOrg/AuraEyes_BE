using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.SystemAdmin.Ophthalmologists.Commands.RejectWithdrawalRequest;

public class RejectWithdrawalRequestCommandHandler : ICommandHandler<RejectWithdrawalRequestCommand>
{
    private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectWithdrawalRequestCommandHandler(
        IWithdrawalRequestRepository withdrawalRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _withdrawalRequestRepository = withdrawalRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RejectWithdrawalRequestCommand request,
        CancellationToken cancellationToken)
    {
        var withdrawalRequest = await _withdrawalRequestRepository.GetByIdAsync(request.WithdrawalRequestId, cancellationToken);
        if (withdrawalRequest is null)
        {
            return Result.NotFound("Withdrawal request not found.");
        }

        if (withdrawalRequest.Status != PaymentStatus.Pending && withdrawalRequest.Status != PaymentStatus.Processing)
        {
            return Result.Conflict("Only pending withdrawal requests can be rejected.");
        }

        withdrawalRequest.MarkRejected(request.AdminUserId, request.Reason);

        await _withdrawalRequestRepository.UpdateAsync(withdrawalRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
