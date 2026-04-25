using Application.ClinicStaffs.Common;
using Application.Common.Interfaces;

namespace Application.ClinicStaffs.Queries.GetClinicStaffProfile;

/// <summary>
/// Query for retrieving current clinic staff profile by authenticated user ID.
/// </summary>
public record GetClinicStaffProfileQuery(Guid UserId) : IQuery<ClinicStaffProfileDto>;

