using Application.ClinicStaffs.Common;
using Application.Common.Interfaces;

namespace Application.ClinicStaffs.Queries.GetClinicStaffById;

/// <summary>Query to retrieve a single ClinicStaff by its domain entity ID.</summary>
public record GetClinicStaffByIdQuery : IQuery<ClinicStaffResponse>
{
    public Guid StaffId { get; init; }
}
