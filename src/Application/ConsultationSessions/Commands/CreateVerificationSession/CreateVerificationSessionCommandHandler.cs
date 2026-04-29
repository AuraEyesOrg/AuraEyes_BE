using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;

namespace Application.ConsultationSessions.Commands.CreateVerificationSession;

public class CreateVerificationSessionCommandHandler
    : ICommandHandler<CreateVerificationSessionCommand, Guid>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IRepository<Ophthalmologist> _ophthalmologistRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVerificationSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IRepository<Ophthalmologist> ophthalmologistRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateVerificationSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = ConsultationSession.CreateVerification(
            request.PatientId,
            request.AiScreeningId,
            request.Price,
            request.OphthalmologistId);

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send real-time notification to Doctor [FR-47] if assigned
        if (request.OphthalmologistId.HasValue)
        {
            var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(
                request.OphthalmologistId.Value,
                cancellationToken);

            if (ophthalmologist is not null)
            {
                await _notificationService.SendAsync(
                    ophthalmologist.UserId,
                    "Yêu cầu tư vấn mới",
                    "Bạn có một yêu cầu xác minh kết quả sàng lọc AI mới từ bệnh nhân. Vui lòng xem chi tiết và phản hồi.",
                    NotificationType.NewConsultationRequest,
                    new
                    {
                        ConsultationId = session.Id,
                        PatientId = request.PatientId,
                        ScreeningId = request.AiScreeningId,
                        RouteHint = $"/ophthalmologist/screenings/{request.AiScreeningId}/review"
                    },
                    cancellationToken);
            }
        }

        return Result<Guid>.Success(session.Id);
    }
}
