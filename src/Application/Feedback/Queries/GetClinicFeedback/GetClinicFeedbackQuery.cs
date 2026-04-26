using Application.Common.Interfaces;
using Application.Feedback.Common;

namespace Application.Feedback.Queries.GetClinicFeedback;

public record GetClinicFeedbackQuery(Guid FeedbackId) : IQuery<ClinicFeedbackDto>;
