using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Feedback.Queries.GetClinicFeedback;

public class GetClinicFeedbackQueryHandler : IQueryHandler<GetClinicFeedbackQuery, ClinicFeedbackDto>
{
    private readonly IClinicFeedbackRepository _clinicFeedbackRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;

    public GetClinicFeedbackQueryHandler(
        IClinicFeedbackRepository clinicFeedbackRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService)
    {
        _clinicFeedbackRepository = clinicFeedbackRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
    }

    public async Task<Result<ClinicFeedbackDto>> Handle(GetClinicFeedbackQuery request, CancellationToken cancellationToken)
    {
        var feedback = await _clinicFeedbackRepository.GetByIdAsync(
            request.FeedbackId,
            cancellationToken);

        if (feedback is null)
            return Result<ClinicFeedbackDto>.NotFound($"Clinic feedback '{request.FeedbackId}' not found.");

        var patientEntity = await _patientRepository.GetByIdAsync(feedback.PatientId, cancellationToken);
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

        var dto = new ClinicFeedbackDto
        {
            Id = feedback.Id,
            PatientId = feedback.PatientId,
            PatientFullName = patientFullName,
            AppointmentId = feedback.AppointmentId,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            DoctorId = feedback.DoctorId,
            StaffId = feedback.StaffId,
            CreatedAt = feedback.CreatedAt
        };

        return Result<ClinicFeedbackDto>.Success(dto);
    }
}
