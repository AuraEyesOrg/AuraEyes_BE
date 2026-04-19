using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.SystemAdmin.Ophthalmologists.Commands.ApproveEmploymentTypeChangeRequest;

public record ApproveEmploymentTypeChangeRequestCommand : ICommand<ApproveEmploymentTypeChangeRequestResultDto>
{
    public Guid RequestId { get; init; }
    public Guid ReviewedByAdminUserId { get; init; }
    public string? AdminNote { get; init; }
}

public record ApproveEmploymentTypeChangeRequestResultDto
{
    public Guid RequestId { get; init; }
    public OphthalmologistEmploymentType PreviousEmploymentType { get; init; }
    public OphthalmologistEmploymentType TargetEmploymentType { get; init; }
    public Guid? ExpiredContractId { get; init; }
    public Guid? NewPendingContractId { get; init; }
}
