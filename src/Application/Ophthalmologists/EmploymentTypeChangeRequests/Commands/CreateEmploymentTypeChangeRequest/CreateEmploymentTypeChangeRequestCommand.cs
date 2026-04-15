using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Ophthalmologists.EmploymentTypeChangeRequests.Commands.CreateEmploymentTypeChangeRequest;

public record CreateEmploymentTypeChangeRequestCommand : ICommand<Guid>
{
    public Guid OphthalmologistId { get; init; }
    public OphthalmologistEmploymentType TargetEmploymentType { get; init; }
    public string Reason { get; init; } = string.Empty;
}
