using Application.Common.Interfaces;
using Domain.Entities.Screening;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Queries;

public sealed class AiScreeningQuery : IAiScreeningQuery
{
    private readonly ApplicationDbContext _context;

    public AiScreeningQuery(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<int> CountRetinalImagesForScreeningAsync(
        Guid screeningId,
        CancellationToken cancellationToken = default)
    {
        return _context.RetinalImages
            .AsNoTracking()
            .CountAsync(r => r.AiScreeningId == screeningId, cancellationToken);
    }
}
