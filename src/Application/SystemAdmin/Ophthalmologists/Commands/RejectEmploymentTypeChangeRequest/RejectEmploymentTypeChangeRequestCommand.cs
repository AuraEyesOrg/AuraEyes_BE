using Application.Common.Interfaces;

namespace Application.SystemAdmin.Ophthalmologists.Commands.RejectEmploymentTypeChangeRequest;

public record RejectEmploymentTypeChangeRequestCommand : ICommand
{
    public Guid RequestId { get; init; }
    public Guid ReviewedByAdminUserId { get; init; }
    public string? AdminNote { get; init; }
}
