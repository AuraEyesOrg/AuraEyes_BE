using Application.CarePlan.HealthRoadmaps.Common;
using Application.Common.Interfaces;

namespace Application.CarePlan.HealthRoadmaps.Queries.GetPatientHealthRoadmap;

/// <summary>Returns the patient's healthcare roadmap (with steps sorted by planned date).</summary>
public record GetPatientHealthRoadmapQuery(Guid PatientId) : IQuery<HealthRoadmapDto>;
