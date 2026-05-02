using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.SystemAdmin.LeavePolicies.Commands.DeleteLeavePolicy;

public class DeleteLeavePolicyCommandHandler : ICommandHandler<DeleteLeavePolicyCommand>
{
    private readonly ILeavePolicyRepository _leavePolicyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLeavePolicyCommandHandler(
        ILeavePolicyRepository leavePolicyRepository,
        IUnitOfWork unitOfWork)
    {
        _leavePolicyRepository = leavePolicyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteLeavePolicyCommand request,
        CancellationToken cancellationToken)
    {
        var policy = await _leavePolicyRepository.GetByIdAsync(request.PolicyId, cancellationToken);

        if (policy is null)
            return Result.Failure("Leave policy not found.");

        policy.SoftDelete();

        await _leavePolicyRepository.UpdateAsync(policy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
