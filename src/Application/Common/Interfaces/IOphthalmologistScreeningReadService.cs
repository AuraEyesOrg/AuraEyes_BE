using Application.OphthalmologistScreenings;

namespace Application.Common.Interfaces;

/// <summary>
/// Read-only queries for ophthalmologist screening list/detail (linked via consultation sessions).
/// </summary>
public interface IOphthalmologistScreeningReadService
{
    Task<IReadOnlyList<OphthalmologistScreeningListItemDto>> ListForOphthalmologistAsync(
        Guid ophthalmologistProfileId,
        CancellationToken cancellationToken = default);

    Task<OphthalmologistScreeningDetailDto?> GetDetailForOphthalmologistAsync(
        Guid ophthalmologistProfileId,
        Guid screeningId,
        CancellationToken cancellationToken = default);
}
