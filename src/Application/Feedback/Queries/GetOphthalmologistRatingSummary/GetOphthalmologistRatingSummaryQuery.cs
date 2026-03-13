using Application.Common.Interfaces;
using Application.Feedback.Common;

namespace Application.Feedback.Queries.GetOphthalmologistRatingSummary;

public record GetOphthalmologistRatingSummaryQuery(Guid OphthalmologistId) : IQuery<FeedbackRatingSummaryDto>;
