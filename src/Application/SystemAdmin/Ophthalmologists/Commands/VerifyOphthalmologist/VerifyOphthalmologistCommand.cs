using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.SystemAdmin.Ophthalmologists.Commands.VerifyOphthalmologist;

/// <summary>
/// Command to approve or reject an ophthalmologist's credential verification.
/// </summary>
public record VerifyOphthalmologistCommand : IRequest<Result<string>>
{
    public Guid OphthalmologistId { get; init; }
    public bool Approve { get; init; }
}
