using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Feedback.Queries.ListClinicFeedback;

public class ListClinicFeedbackQueryHandler
    : IQueryHandler<ListClinicFeedbackQuery, PagedResult<ClinicFeedbackDto>>
{
    private readonly IClinicFeedbackRepository _clinicFeedbackRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;

    public ListClinicFeedbackQueryHandler(
        IClinicFeedbackRepository clinicFeedbackRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService)
    {
        _clinicFeedbackRepository = clinicFeedbackRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<ClinicFeedbackDto>>> Handle(
        ListClinicFeedbackQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _clinicFeedbackRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = new List<ClinicFeedbackDto>();

        foreach (var x in items)
        {
            var patientEntity = await _patientRepository.GetByIdAsNoTrackingAsync(x.PatientId, cancellationToken);
            string? patientFullName = null;
            if (patientEntity is not null && patientEntity.IsWalkIn)
            {
                patientFullName = patientEntity.FullName;
            }
            else if (patientEntity is not null && patientEntity.UserId.HasValue)
            {
                var patientUser = await _identityService.GetUserByIdAsync(patientEntity.UserId.Value, cancellationToken);
                patientFullName = patientUser?.FullName;
            }

            dtoList.Add(new ClinicFeedbackDto
            {
                Id = x.Id,
                PatientId = x.PatientId,
                PatientFullName = patientFullName,
                AppointmentId = x.AppointmentId,
                Rating = x.Rating,
                Comment = x.Comment,
                DoctorId = x.DoctorId,
                StaffId = x.StaffId,
                CreatedAt = x.CreatedAt
            });
        }

        var pagedResult = new PagedResult<ClinicFeedbackDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<ClinicFeedbackDto>>.Success(pagedResult);
    }
}

