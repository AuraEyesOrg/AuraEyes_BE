using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for SlotAssignment aggregate root.
/// </summary>
public class SlotAssignmentRepository : Repository<SlotAssignment>, ISlotAssignmentRepository
{
    public SlotAssignmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<SlotAssignment>> GetBySlotIdAsync(
        Guid appointmentSlotId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(sa => sa.AppointmentSlotId == appointmentSlotId)
            .OrderBy(sa => sa.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SlotAssignment>> GetByStaffIdAsync(
        Guid staffId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(sa => sa.AppointmentSlot)
            .Where(sa => sa.StaffId == staffId);

        if (fromDate.HasValue)
            query = query.Where(sa => sa.AppointmentSlot != null && sa.AppointmentSlot.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(sa => sa.AppointmentSlot != null && sa.AppointmentSlot.Date <= toDate.Value);

        return await query
            .OrderBy(sa => sa.AppointmentSlot!.Date)
            .ThenBy(sa => sa.AppointmentSlot!.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasAssignmentAsync(
        Guid appointmentSlotId,
        Guid staffId,
        SlotAssignmentRole role,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(sa =>
            sa.AppointmentSlotId == appointmentSlotId &&
            sa.StaffId == staffId &&
            sa.Role == role,
            cancellationToken);
    }
}
