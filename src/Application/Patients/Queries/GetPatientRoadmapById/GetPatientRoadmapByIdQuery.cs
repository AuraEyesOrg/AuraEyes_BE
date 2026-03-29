using Application.Common.Interfaces;
using Application.Patients.Common;

namespace Application.Patients.Queries.GetPatientRoadmapById;

public record GetPatientRoadmapByIdQuery(Guid UserId, Guid RoadmapId) : IQuery<PatientRoadmapDto>;
