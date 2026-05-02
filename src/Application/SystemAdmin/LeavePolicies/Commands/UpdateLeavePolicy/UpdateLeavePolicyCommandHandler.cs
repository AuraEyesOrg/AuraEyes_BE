using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.SystemAdmin.LeavePolicies.Commands.UpdateLeavePolicy;

public class UpdateLeavePolicyCommandHandler : ICommandHandler<UpdateLeavePolicyCommand>
{
    private readonly ILeavePolicyRepository _leavePolicyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLeavePolicyCommandHandler(
        ILeavePolicyRepository leavePolicyRepository,
        IUnitOfWork unitOfWork)
    {
        _leavePolicyRepository = leavePolicyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateLeavePolicyCommand request,
        CancellationToken cancellationToken)
    {
        var policy = await _leavePolicyRepository.GetByIdAsync(request.PolicyId, cancellationToken);

        if (policy is null)
            return Result.Failure("Leave policy not found.");

        policy.Update(request.Name, request.AdditionalDays, request.Description);

        await _leavePolicyRepository.UpdateAsync(policy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
