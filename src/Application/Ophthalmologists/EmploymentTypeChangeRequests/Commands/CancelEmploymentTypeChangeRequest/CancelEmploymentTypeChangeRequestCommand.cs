using Application.Common.Interfaces;

namespace Application.Ophthalmologists.EmploymentTypeChangeRequests.Commands.CancelEmploymentTypeChangeRequest;

public record CancelEmploymentTypeChangeRequestCommand : ICommand
{
    public Guid RequestId { get; init; }
    public Guid OphthalmologistId { get; init; }
}
