using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Ophthalmologist aggregate root.
/// </summary>
public class OphthalmologistRepository : Repository<Ophthalmologist>, IOphthalmologistRepository
{
    public OphthalmologistRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Ophthalmologist?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Certificates)
            .FirstOrDefaultAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<Ophthalmologist?> GetByIdWithCertificatesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.Certificates)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Ophthalmologist> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(o => o.Certificates)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm}%";
            query = query
                .Join(_context.Users, o => o.UserId, u => u.Id, (o, u) => new { o, u })
                .Where(x => (x.u.FullName != null && EF.Functions.ILike(x.u.FullName, pattern)) ||
                            (x.u.Email != null && EF.Functions.ILike(x.u.Email, pattern)) ||
                            (x.o.Phone != null && EF.Functions.ILike(x.o.Phone, pattern)))
                .Select(x => x.o);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(o => o.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, string>> GetDisplayNamesByIdsAsync(
        IReadOnlyCollection<Guid> ophthalmologistIds,
        CancellationToken cancellationToken = default)
    {
        if (ophthalmologistIds.Count == 0)
            return new Dictionary<Guid, string>();

        var uniqueIds = ophthalmologistIds.Distinct().ToArray();

        var rows = await (
            from ophthalmologist in _dbSet
            join user in _context.Users on ophthalmologist.UserId equals user.Id
            where uniqueIds.Contains(ophthalmologist.Id) && !user.IsDeleted
            select new
            {
                ophthalmologist.Id,
                user.FullName
            })
            .ToListAsync(cancellationToken);

        return rows
            .Where(x => !string.IsNullOrWhiteSpace(x.FullName))
            .ToDictionary(x => x.Id, x => x.FullName);
    }

    public async Task<IReadOnlyDictionary<Guid, (string FullName, string? AvatarUrl)>> GetDoctorDetailsByIdsAsync(
        IReadOnlyCollection<Guid> ophthalmologistIds,
        CancellationToken cancellationToken = default)
    {
        if (ophthalmologistIds.Count == 0)
            return new Dictionary<Guid, (string FullName, string? AvatarUrl)>();

        var uniqueIds = ophthalmologistIds.Distinct().ToArray();

        var rows = await (
            from ophthalmologist in _dbSet
            join user in _context.Users on ophthalmologist.UserId equals user.Id
            where uniqueIds.Contains(ophthalmologist.Id) && !user.IsDeleted
            select new
            {
                ophthalmologist.Id,
                user.FullName,
                user.AvatarUrl
            })
            .ToListAsync(cancellationToken);

        return rows
            .ToDictionary(
                x => x.Id,
                x => (FullName: x.FullName ?? "Unknown", AvatarUrl: (string?)x.AvatarUrl));
    }
    public async Task<IReadOnlyDictionary<Guid, EnhancedDoctorDetail>> GetEnhancedDoctorDetailsByIdsAsync(
        IReadOnlyCollection<Guid> ophthalmologistIds,
        CancellationToken cancellationToken = default)
    {
        if (ophthalmologistIds.Count == 0)
            return new Dictionary<Guid, EnhancedDoctorDetail>();

        var uniqueIds = ophthalmologistIds.Distinct().ToArray();

        var query = from ophthalmologist in _dbSet.AsNoTracking().Include(o => o.Certificates)
                    join user in _context.Users on ophthalmologist.UserId equals user.Id
                    where uniqueIds.Contains(ophthalmologist.Id) && !user.IsDeleted
                    select new
                    {
                        ophthalmologist.Id,
                        user.FullName,
                        user.AvatarUrl,
                        ophthalmologist.Bio,
                        ophthalmologist.RatingAverage,
                        ophthalmologist.RatingCount,
                        Certificates = ophthalmologist.Certificates.ToList()
                    };

        var rows = await query.ToListAsync(cancellationToken);

        return rows.ToDictionary(
            x => x.Id,
            x => new EnhancedDoctorDetail(
                x.FullName ?? "Unknown",
                x.AvatarUrl,
                x.Bio,
                x.RatingAverage,
                x.RatingCount,
                x.Certificates));
    }
    public async Task<List<ConsiliumDoctorDetail>> GetAvailableDoctorsForConsiliumAsync(
        DateTime windowStart,
        DateTime windowEnd,
        CancellationToken cancellationToken = default)
    {
        var startDay = DateOnly.FromDateTime(windowStart);
        var startTime = TimeOnly.FromDateTime(windowStart);
        var endDay = DateOnly.FromDateTime(windowEnd);
        var endTime = TimeOnly.FromDateTime(windowEnd);

        // Perform single-query optimization projecting directly to DTO
        var query = from ophthal in _dbSet
                    join user in _context.Users on ophthal.UserId equals user.Id
                    where user.IsActive && !user.IsDeleted
                    // Check Approved Leave Requests
                    && !_context.OphthalmologistLeaveRequests.Any(lr =>
                        lr.OphthalmologistId == ophthal.Id &&
                        lr.Status == Domain.Enums.OphthalmologistLeaveRequestStatus.Approved &&
                        lr.StartDate <= startDay && lr.EndDate >= startDay)
                    // Check Overlapping Busy Appointment Slots
                    && !_context.AppointmentSlots.Any(slot =>
                        slot.OphthalId == ophthal.Id &&
                        slot.Date == startDay &&
                        slot.StartTime < endTime &&
                        slot.EndTime > startTime &&
                        (slot.Status == Domain.Enums.ScheduleStatus.Blocked || slot.BookedCount > 0))
                    select new ConsiliumDoctorDetail(
                        ophthal.Id,
                        user.FullName ?? "Doctor",
                        user.AvatarUrl,
                        ophthal.Certificates
                            .Where(c => c.DegreeLevel != null)
                            .OrderByDescending(c => c.DegreeLevel)
                            .Select(c => c.DegreeLevel.ToString())
                            .FirstOrDefault()
                    );

        return await query
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
