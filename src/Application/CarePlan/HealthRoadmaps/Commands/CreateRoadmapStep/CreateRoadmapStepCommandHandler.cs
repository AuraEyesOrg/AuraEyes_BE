using Application.CarePlan.HealthRoadmaps.Common;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.CarePlan;
using Domain.Entities.Scheduling;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.CarePlan.HealthRoadmaps.Commands.CreateRoadmapStep;

public class CreateRoadmapStepCommandHandler
    : ICommandHandler<CreateRoadmapStepCommand, HealthRoadmapStepDto>
{
    private readonly IHealthRoadmapRepository _roadmapRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IPatientVisitRepository _visitRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoadmapStepCommandHandler(
        IHealthRoadmapRepository roadmapRepository,
        IRepository<Patient> patientRepository,
        IPatientVisitRepository visitRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _roadmapRepository = roadmapRepository;
        _patientRepository = patientRepository;
        _visitRepository = visitRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HealthRoadmapStepDto>> Handle(
        CreateRoadmapStepCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsInRole(Roles.Ophthalmologist))
            return Result<HealthRoadmapStepDto>.Forbidden("Only doctors can create roadmap steps.");

        if (!_currentUser.ProfileId.HasValue)
            return Result<HealthRoadmapStepDto>.Unauthorized("Doctor profile is required.");

        var doctorId = _currentUser.ProfileId.Value;

        // Verify the patient exists.
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result<HealthRoadmapStepDto>.NotFound("Patient not found.");

        // Optional visit must exist and belong to this patient.
        if (request.CreatedFromVisitId.HasValue)
        {
            var visit = await _visitRepository.GetByIdAsync(
                request.CreatedFromVisitId.Value, cancellationToken);
            if (visit is null)
                return Result<HealthRoadmapStepDto>.NotFound("Source patient visit not found.");
            if (visit.PatientId != request.PatientId)
                return Result<HealthRoadmapStepDto>.Failure(
                    "Source visit does not belong to the specified patient.");
        }

        // Get-or-create the roadmap (one per patient).
        var roadmap = await _roadmapRepository.GetByPatientWithStepsAsync(
            request.PatientId, cancellationToken);

        var isNewRoadmap = roadmap is null;
        if (roadmap is null)
        {
            roadmap = HealthRoadmap.CreateForPatient(request.PatientId);
            await _roadmapRepository.AddAsync(roadmap, cancellationToken);
        }

        var step = HealthRoadmapStep.Create(
            roadmapId: roadmap.Id,
            title: request.Title,
            description: request.Description,
            stepType: request.StepType,
            plannedDate: request.PlannedDate,
            createdByDoctorId: doctorId,
            createdFromVisitId: request.CreatedFromVisitId,
            orderIndex: request.OrderIndex);

        roadmap.AddStep(step);

        if (!isNewRoadmap)
        {
            await _roadmapRepository.UpdateAsync(roadmap, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return Result<HealthRoadmapStepDto>.Success(HealthRoadmapMapper.ToDto(step, today));
    }
}
