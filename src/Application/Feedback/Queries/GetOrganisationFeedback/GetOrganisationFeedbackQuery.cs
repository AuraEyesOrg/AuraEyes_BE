using Application.Common.Interfaces;
using Application.Feedback.Common;

namespace Application.Feedback.Queries.GetOrganisationFeedback;

public record GetOrganisationFeedbackQuery(Guid? OrganisationId, Guid FeedbackId) : IQuery<ClinicFeedbackDto>;
