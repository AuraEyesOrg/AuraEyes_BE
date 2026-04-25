using Application.CarePlan.HealthRoadmaps.Common;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.CarePlan.HealthRoadmaps.Queries.GetPatientHealthRoadmap;

public class GetPatientHealthRoadmapQueryHandler
    : IQueryHandler<GetPatientHealthRoadmapQuery, HealthRoadmapDto>
{
    private readonly IHealthRoadmapRepository _roadmapRepository;
    private readonly ICurrentUserService _currentUser;

    public GetPatientHealthRoadmapQueryHandler(
        IHealthRoadmapRepository roadmapRepository,
        ICurrentUserService currentUser)
    {
        _roadmapRepository = roadmapRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<HealthRoadmapDto>> Handle(
        GetPatientHealthRoadmapQuery request,
        CancellationToken cancellationToken)
    {
        // Authorization:
        //  - Patients may only view their own roadmap.
        //  - Ophthalmologists / ClinicStaff / SystemAdmin can view any patient's roadmap.
        if (_currentUser.IsInRole(Roles.Patient))
        {
            if (!_currentUser.ProfileId.HasValue || _currentUser.ProfileId.Value != request.PatientId)
            {
                return Result<HealthRoadmapDto>.Forbidden(
                    "You can only view your own healthcare roadmap.");
            }
        }
        else if (!_currentUser.IsInRole(Roles.Ophthalmologist)
                 && !_currentUser.IsInRole(Roles.ClinicStaff)
                 && !_currentUser.IsInRole(Roles.SystemAdmin))
        {
            return Result<HealthRoadmapDto>.Forbidden(
                "You are not authorized to view this roadmap.");
        }

        var roadmap = await _roadmapRepository.GetByPatientWithStepsAsync(
            request.PatientId, cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dto = HealthRoadmapMapper.ToDto(roadmap, request.PatientId, today);
        return Result<HealthRoadmapDto>.Success(dto);
    }
}
