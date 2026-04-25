using Application.CarePlan.HealthRoadmaps.Common;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.CarePlan.HealthRoadmaps.Commands.UpdateRoadmapStep;

public class UpdateRoadmapStepCommandHandler
    : ICommandHandler<UpdateRoadmapStepCommand, HealthRoadmapStepDto>
{
    private readonly IHealthRoadmapRepository _roadmapRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoadmapStepCommandHandler(
        IHealthRoadmapRepository roadmapRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _roadmapRepository = roadmapRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HealthRoadmapStepDto>> Handle(
        UpdateRoadmapStepCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole(Roles.Ophthalmologist))
            return Result<HealthRoadmapStepDto>.Forbidden("Only doctors can update roadmap steps.");

        var step = await _roadmapRepository.GetStepByIdAsync(request.StepId, cancellationToken);
        if (step is null)
            return Result<HealthRoadmapStepDto>.NotFound("Roadmap step not found.");

        if (step.Status == RoadmapStepStatus.Completed)
            return Result<HealthRoadmapStepDto>.Conflict("Completed steps cannot be edited.");

        try
        {
            step.UpdateDetails(
                title: request.Title,
                description: request.Description,
                stepType: request.StepType,
                plannedDate: request.PlannedDate,
                orderIndex: request.OrderIndex);
        }
        catch (InvalidOperationException ex)
        {
            return Result<HealthRoadmapStepDto>.Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Result<HealthRoadmapStepDto>.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return Result<HealthRoadmapStepDto>.Success(HealthRoadmapMapper.ToDto(step, today));
    }
}
