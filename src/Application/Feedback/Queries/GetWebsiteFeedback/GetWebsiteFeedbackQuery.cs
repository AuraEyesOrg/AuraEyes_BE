using Application.Common.Interfaces;
using Application.Feedback.Common;

namespace Application.Feedback.Queries.GetWebsiteFeedback;

public record GetWebsiteFeedbackQuery(Guid FeedbackId) : IQuery<WebsiteFeedbackDto>;
