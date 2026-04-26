using Application.Common.Interfaces;
using Application.Feedback.Common;

namespace Application.Feedback.Queries.GetClinicRatingSummary;

public record GetClinicRatingSummaryQuery(Guid? DoctorId = null, Guid? StaffId = null) : IQuery<FeedbackRatingSummaryDto>;
