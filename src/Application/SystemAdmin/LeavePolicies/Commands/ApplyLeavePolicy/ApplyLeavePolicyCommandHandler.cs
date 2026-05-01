using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.SystemAdmin.LeavePolicies.Commands.ApplyLeavePolicy;

public class ApplyLeavePolicyCommandHandler : ICommandHandler<ApplyLeavePolicyCommand>
{
    private readonly ILeavePolicyRepository _leavePolicyRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyLeavePolicyCommandHandler(
        ILeavePolicyRepository leavePolicyRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IUnitOfWork unitOfWork)
    {
        _leavePolicyRepository = leavePolicyRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ApplyLeavePolicyCommand request,
        CancellationToken cancellationToken)
    {
        var policy = await _leavePolicyRepository.GetByIdAsync(request.PolicyId, cancellationToken);
        if (policy is null)
            return Result.Failure("Leave policy not found.");

        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
            return Result.Failure("Ophthalmologist not found.");

        ophthalmologist.AddLeaveDays(policy.AdditionalDays);

        await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
