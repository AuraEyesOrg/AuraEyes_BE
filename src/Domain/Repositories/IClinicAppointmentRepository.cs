using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for ClinicAppointment aggregate root.
/// Contains domain-specific query methods for organisation clinic appointments.
/// </summary>
public interface IClinicAppointmentRepository : IRepository<ClinicAppointment>
{
    /// <summary>
    /// Get appointments for a specific organisation.
    /// </summary>
    Task<IReadOnlyList<ClinicAppointment>> GetByOrganisationAsync(
        Guid organisationId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        AppointmentStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments for a specific patient.
    /// </summary>
    Task<IReadOnlyList<ClinicAppointment>> GetByPatientAsync(
        Guid patientId,
        AppointmentStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments for a specific slot.
    /// </summary>
    Task<IReadOnlyList<ClinicAppointment>> GetBySlotAsync(
        Guid slotId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointment with all navigation properties included.
    /// </summary>
    Task<ClinicAppointment?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if patient already has an appointment at the given slot.
    /// </summary>
    Task<bool> HasExistingAppointmentAsync(
        Guid patientId,
        Guid slotId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments for a specific date at an organisation.
    /// </summary>
    Task<IReadOnlyList<ClinicAppointment>> GetByOrganisationAndDateAsync(
        Guid organisationId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated appointments with filters.
    /// </summary>
    Task<(IReadOnlyList<ClinicAppointment> Items, int TotalCount)> GetPagedAsync(
        Guid? organisationId = null,
        Guid? patientId = null,
        AppointmentStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get status counts for reporting.
    /// </summary>
    Task<Dictionary<AppointmentStatus, int>> GetStatusCountsAsync(
        Guid organisationId,
        DateOnly? date = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get upcoming appointments for a patient (confirmed, not completed/cancelled).
    /// </summary>
    Task<IReadOnlyList<ClinicAppointment>> GetUpcomingByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}
