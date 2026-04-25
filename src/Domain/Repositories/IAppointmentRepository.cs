using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository contract for Appointment aggregate root.
/// Clinic-centric booking-only model.
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetBySlotAsync(
        Guid slotId,
        CancellationToken cancellationToken = default);

    Task<Appointment?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> HasExistingAppointmentAsync(
        Guid patientId,
        Guid slotId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Appointment>> GetByPatientAsync(
        Guid patientId,
        AppointmentStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedAsync(
        Guid? patientId = null,
        AppointmentStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedByPatientAsync(
        Guid patientId,
        IReadOnlyCollection<AppointmentStatus>? statuses = null,
        int pageNumber = 1,
        int pageSize = 10,
        bool upcomingOnly = false,
        CancellationToken cancellationToken = default);

    Task<Dictionary<AppointmentStatus, int>> GetPatientStatusCountsAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<Dictionary<AppointmentStatus, int>> GetStatusCountsAsync(
        DateOnly? date = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Appointment>> GetUpcomingByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}
