using Application.Common.Models;
using MediatR;

namespace Application.SystemAdmin.Ophthalmologists.Commands.DeleteFutureOphthalmologistSlots;

/// <summary>
/// Deletes future appointment slots for one ophthalmologist.
/// Past slots are preserved.
/// </summary>
public record DeleteFutureOphthalmologistSlotsCommand : IRequest<Result<DeleteFutureOphthalmologistSlotsResultDto>>
{
    public Guid OphthalmologistId { get; init; }
}

public record DeleteFutureOphthalmologistSlotsResultDto
{
    public Guid OphthalmologistId { get; init; }
    public DateOnly Today { get; init; }
    public TimeOnly CurrentTimeUtc { get; init; }
    public int MatchedFutureSlots { get; init; }
    public int DeletedSlots { get; init; }
    public int ProtectedSlots { get; init; }
}
