using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for unified Appointment aggregate root.
/// Supports both ONLINE_CONSULTATION and CLINIC_VISIT appointment types.
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    /// <summary>
    /// Get appointments for a specific organisation (clinic visits).
    /// </summary>
    Task<IReadOnlyList<Appointment>> GetByOrganisationAsync(
        Guid organisationId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        AppointmentStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments for a specific doctor (online consultations or assigned clinic).
    /// </summary>
    Task<IReadOnlyList<Appointment>> GetByDoctorAsync(
        Guid doctorId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        AppointmentStatus? status = null,
        AppointmentType? type = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments for a specific patient.
    /// </summary>
    Task<IReadOnlyList<Appointment>> GetByPatientAsync(
        Guid patientId,
        AppointmentType? type = null,
        AppointmentStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointments for a specific slot.
    /// </summary>
    Task<IReadOnlyList<Appointment>> GetBySlotAsync(
        Guid slotId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointment with all navigation properties included.
    /// </summary>
    Task<Appointment?> GetByIdWithDetailsAsync(
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
    Task<IReadOnlyList<Appointment>> GetByOrganisationAndDateAsync(
        Guid organisationId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated appointments with filters.
    /// </summary>
    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedAsync(
        Guid? organisationId = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        AppointmentType? type = null,
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
        Guid? organisationId = null,
        Guid? doctorId = null,
        DateOnly? date = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get upcoming appointments for a patient (confirmed, not completed/cancelled).
    /// </summary>
    Task<IReadOnlyList<Appointment>> GetUpcomingByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get upcoming appointments for a doctor.
    /// </summary>
    Task<IReadOnlyList<Appointment>> GetUpcomingByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointment by consultation session ID.
    /// </summary>
    Task<Appointment?> GetByConsultationSessionIdAsync(
        Guid consultationSessionId,
        CancellationToken cancellationToken = default);
}
