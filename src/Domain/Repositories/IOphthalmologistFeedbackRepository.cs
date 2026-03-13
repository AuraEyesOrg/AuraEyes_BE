using Domain.Common;
using Domain.Entities.Consultation;

namespace Domain.Repositories;

public interface IOphthalmologistFeedbackRepository : IRepository<OphthalmologistFeedback>
{
    Task<bool> ExistsByPatientAndSessionAsync(
        Guid patientId,
        Guid consultationSessionId,
        CancellationToken cancellationToken = default);

    Task<OphthalmologistFeedback?> GetByIdForOphthalmologistAsync(
        Guid ophthalmologistId,
        Guid feedbackId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<OphthalmologistFeedback> Items, int TotalCount)> GetPagedByOphthalmologistAsync(
        Guid ophthalmologistId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<(decimal RatingAverage, int RatingCount, Dictionary<int, int> Distribution)> GetRatingSummaryAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default);
}
