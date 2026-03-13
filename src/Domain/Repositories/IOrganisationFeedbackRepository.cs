using Domain.Common;
using Domain.Entities.Consultation;

namespace Domain.Repositories;

public interface IOrganisationFeedbackRepository : IRepository<OrganisationFeedback>
{
    Task<bool> ExistsByPatientAndAppointmentAsync(
        Guid patientId,
        Guid appointmentId,
        CancellationToken cancellationToken = default);

    Task<OrganisationFeedback?> GetByIdForOrganisationAsync(
        Guid organisationId,
        Guid feedbackId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<OrganisationFeedback> Items, int TotalCount)> GetPagedByOrganisationAsync(
        Guid organisationId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<(decimal RatingAverage, int RatingCount, Dictionary<int, int> Distribution)> GetRatingSummaryAsync(
        Guid organisationId,
        CancellationToken cancellationToken = default);
}
