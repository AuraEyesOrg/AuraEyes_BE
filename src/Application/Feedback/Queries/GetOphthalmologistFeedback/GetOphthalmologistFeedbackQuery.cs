using Application.Common.Interfaces;
using Application.Feedback.Common;

namespace Application.Feedback.Queries.GetOphthalmologistFeedback;

public record GetOphthalmologistFeedbackQuery(Guid OphthalmologistId, Guid FeedbackId) : IQuery<OphthalmologistFeedbackDto>;
