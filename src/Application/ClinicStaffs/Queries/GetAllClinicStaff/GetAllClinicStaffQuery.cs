using Application.ClinicStaffs.Common;
using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.ClinicStaffs.Queries.GetAllClinicStaff;

/// <summary>Query to list all active clinic staff, optionally filtered by sub-role.</summary>
public record GetAllClinicStaffQuery : IQuery<IReadOnlyList<ClinicStaffResponse>>
{
    public ClinicStaffRole? SubRole { get; init; }
}
