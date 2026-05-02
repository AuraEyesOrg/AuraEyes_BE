using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.LeavePolicies.Common;
using Domain.Common;
using Domain.Entities.Platform;
using Domain.Repositories;

namespace Application.SystemAdmin.LeavePolicies.Commands.CreateLeavePolicy;

public class CreateLeavePolicyCommandHandler : ICommandHandler<CreateLeavePolicyCommand, LeavePolicyDto>
{
    private readonly ILeavePolicyRepository _leavePolicyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLeavePolicyCommandHandler(
        ILeavePolicyRepository leavePolicyRepository,
        IUnitOfWork unitOfWork)
    {
        _leavePolicyRepository = leavePolicyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LeavePolicyDto>> Handle(
        CreateLeavePolicyCommand request,
        CancellationToken cancellationToken)
    {
        var policy = LeavePolicy.Create(request.Name, request.AdditionalDays, request.Description);

        await _leavePolicyRepository.AddAsync(policy, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LeavePolicyDto>.Success(new LeavePolicyDto
        {
            Id = policy.Id,
            Name = policy.Name,
            AdditionalDays = policy.AdditionalDays,
            Description = policy.Description,
            CreatedAt = policy.CreatedAt,
            UpdatedAt = policy.UpdatedAt
        });
    }
}
