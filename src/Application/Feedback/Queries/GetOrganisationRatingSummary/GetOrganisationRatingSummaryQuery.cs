using Application.Common.Interfaces;
using Application.Feedback.Common;

namespace Application.Feedback.Queries.GetOrganisationRatingSummary;

public record GetOrganisationRatingSummaryQuery(Guid OrganisationId) : IQuery<FeedbackRatingSummaryDto>;
