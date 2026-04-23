using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository contract for PatientVisit aggregate root.
/// </summary>
public interface IPatientVisitRepository : IRepository<PatientVisit>
{
    Task<PatientVisit?> GetByIdWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PatientVisit?> GetByAppointmentIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PatientVisit>> GetByPatientAsync(
        Guid patientId,
        PatientVisitStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PatientVisit>> GetByDoctorAsync(
        Guid doctorId,
        PatientVisitStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<PatientVisit> Items, int TotalCount)> GetPagedAsync(
        Guid? patientId = null,
        Guid? doctorId = null,
        PatientVisitStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<bool> HasActiveVisitAsync(
        Guid appointmentId,
        CancellationToken cancellationToken = default);
}
