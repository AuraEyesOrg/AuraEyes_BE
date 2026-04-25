using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.CarePlan.HealthRoadmaps.Commands.DeleteRoadmapStep;

public class DeleteRoadmapStepCommandHandler : ICommandHandler<DeleteRoadmapStepCommand>
{
    private readonly IHealthRoadmapRepository _roadmapRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoadmapStepCommandHandler(
        IHealthRoadmapRepository roadmapRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _roadmapRepository = roadmapRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteRoadmapStepCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole(Roles.Ophthalmologist))
            return Result.Forbidden("Only doctors can delete roadmap steps.");

        var step = await _roadmapRepository.GetStepByIdAsync(request.StepId, cancellationToken);
        if (step is null)
            return Result.NotFound("Roadmap step not found.");

        if (step.Status == RoadmapStepStatus.Completed)
            return Result.Conflict("Completed steps cannot be deleted.");

        _roadmapRepository.RemoveStep(step);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
