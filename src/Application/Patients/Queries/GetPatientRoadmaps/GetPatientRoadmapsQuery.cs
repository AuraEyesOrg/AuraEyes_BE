using Application.Common.Interfaces;
using Application.Patients.Common;

namespace Application.Patients.Queries.GetPatientRoadmaps;

public record GetPatientRoadmapsQuery(Guid UserId) : IQuery<IReadOnlyList<PatientRoadmapDto>>;
