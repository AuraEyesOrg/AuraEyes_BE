using Application.Common.Interfaces;

namespace Application.OphthalmologistScreenings.Queries.GetOphthalmologistScreeningDetail;

/// <summary>
/// Query to retrieve detailed screening information for an ophthalmologist.
/// </summary>
public record GetOphthalmologistScreeningDetailQuery : IQuery<OphthalmologistScreeningDetailDto?>
{
    public required Guid OphthalmologistProfileId { get; init; }
    public required Guid ScreeningId { get; init; }
}
