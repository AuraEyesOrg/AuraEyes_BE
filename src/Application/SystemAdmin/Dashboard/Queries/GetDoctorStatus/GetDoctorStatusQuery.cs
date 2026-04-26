using Application.Common.Interfaces;

namespace Application.SystemAdmin.Dashboard.Queries.GetDoctorStatus;

public record GetDoctorStatusQuery : IQuery<IReadOnlyList<DoctorStatusDto>>
{
}
