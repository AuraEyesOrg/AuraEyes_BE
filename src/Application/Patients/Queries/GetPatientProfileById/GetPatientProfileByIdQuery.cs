using Application.Common.Interfaces;
using Application.Patients.Common;

namespace Application.Patients.Queries.GetPatientProfileById;

/// <summary>
/// Query to get a specific patient's profile by their Patient ID.
/// </summary>
public record GetPatientProfileByIdQuery(Guid PatientId) : IQuery<PatientProfileDto>;
