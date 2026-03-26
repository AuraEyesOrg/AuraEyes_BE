using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Feedback.Queries.ListOphthalmologistFeedback;

public class ListOphthalmologistFeedbackQueryHandler
    : IQueryHandler<ListOphthalmologistFeedbackQuery, PagedResult<OphthalmologistFeedbackDto>>
{
    private readonly IOphthalmologistFeedbackRepository _ophthalmologistFeedbackRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;

    public ListOphthalmologistFeedbackQueryHandler(
        IOphthalmologistFeedbackRepository ophthalmologistFeedbackRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService)
    {
        _ophthalmologistFeedbackRepository = ophthalmologistFeedbackRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<OphthalmologistFeedbackDto>>> Handle(
        ListOphthalmologistFeedbackQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _ophthalmologistFeedbackRepository.GetPagedByOphthalmologistAsync(
            request.OphthalmologistId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = new List<OphthalmologistFeedbackDto>();

        foreach (var x in items)
        {
            var patientEntity = await _patientRepository.GetByIdAsync(x.PatientId, cancellationToken);
            var patientUser = patientEntity != null
                ? await _identityService.GetUserByIdAsync(patientEntity.UserId, cancellationToken)
                : null;

            dtoList.Add(new OphthalmologistFeedbackDto
            {
                Id = x.Id,
                PatientId = x.PatientId,
                PatientFullName = patientUser?.FullName,
                OphthalmologistId = x.OphthalmologistId,
                ConsultationSessionId = x.ConsultationSessionId,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAt = x.CreatedAt,
            });
        }

        var pagedResult = new PagedResult<OphthalmologistFeedbackDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<OphthalmologistFeedbackDto>>.Success(pagedResult);
    }
}
