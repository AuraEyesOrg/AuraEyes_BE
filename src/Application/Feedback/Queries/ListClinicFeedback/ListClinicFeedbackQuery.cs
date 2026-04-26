using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;

namespace Application.Feedback.Queries.ListClinicFeedback;

public record ListClinicFeedbackQuery : IQuery<PagedResult<ClinicFeedbackDto>>
{
    public Guid? DoctorId { get; init; }
    public Guid? StaffId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
