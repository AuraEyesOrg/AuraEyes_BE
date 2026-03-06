using Application.Common.Interfaces;
using Application.Patients.Common;

namespace Application.Patients.Queries.GetPatientProfile;

/// <summary>
/// Query to get the current patient's profile.
/// </summary>
public record GetPatientProfileQuery(Guid UserId) : IQuery<PatientProfileDto>;
