using Application.CarePlan.HealthRoadmaps.Common;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.CarePlan.HealthRoadmaps.Commands.CompleteRoadmapStep;

public class CompleteRoadmapStepCommandHandler
    : ICommandHandler<CompleteRoadmapStepCommand, HealthRoadmapStepDto>
{
    private readonly IHealthRoadmapRepository _roadmapRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteRoadmapStepCommandHandler(
        IHealthRoadmapRepository roadmapRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _roadmapRepository = roadmapRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HealthRoadmapStepDto>> Handle(
        CompleteRoadmapStepCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole(Roles.Ophthalmologist))
            return Result<HealthRoadmapStepDto>.Forbidden("Only doctors can complete roadmap steps.");

        var step = await _roadmapRepository.GetStepByIdAsync(request.StepId, cancellationToken);
        if (step is null)
            return Result<HealthRoadmapStepDto>.NotFound("Roadmap step not found.");

        try
        {
            step.MarkCompleted();
        }
        catch (InvalidOperationException ex)
        {
            return Result<HealthRoadmapStepDto>.Conflict(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return Result<HealthRoadmapStepDto>.Success(HealthRoadmapMapper.ToDto(step, today));
    }
}
