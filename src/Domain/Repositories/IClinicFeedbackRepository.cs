using Domain.Common;
using Domain.Entities.Consultation;

namespace Domain.Repositories;

public interface IClinicFeedbackRepository : IRepository<ClinicFeedback>
{
    Task<bool> ExistsByPatientAndAppointmentAsync(
        Guid patientId,
        Guid appointmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Given a set of appointment IDs, return the subset the patient has already rated.
    /// </summary>
    Task<IReadOnlySet<Guid>> GetAppointmentIdsWithFeedbackAsync(
        Guid patientId,
        IReadOnlyCollection<Guid> appointmentIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get feedback paged for the entire clinic.
    /// </summary>
    Task<(IReadOnlyList<ClinicFeedback> Items, int TotalCount)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get rating summary for the entire clinic.
    /// </summary>
    Task<(decimal RatingAverage, int RatingCount, Dictionary<int, int> Distribution)> GetRatingSummaryAsync(
        CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Get rating summary for a specific doctor.
    /// </summary>
    Task<(decimal RatingAverage, int RatingCount)> GetDoctorRatingSummaryAsync(
        Guid doctorId,
        CancellationToken cancellationToken = default);
}
