using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Screenings.Commands.CompleteAiScreening;

/// <summary>
/// Handler for CompleteAiScreeningCommand
/// Marks AI screening as processed and sends real-time notification to patient [FR-45]
/// </summary>
public class CompleteAiScreeningCommandHandler : ICommandHandler<CompleteAiScreeningCommand, CompleteAiScreeningResponse>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteAiScreeningCommandHandler> _logger;

    public CompleteAiScreeningCommandHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<CompleteAiScreeningCommandHandler> logger)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CompleteAiScreeningResponse>> Handle(
        CompleteAiScreeningCommand request,
        CancellationToken cancellationToken)
    {
        // Get the screening session
        var screening = await _screeningRepository.GetByIdAsync(request.ScreeningId, cancellationToken);

        if (screening is null)
        {
            _logger.LogWarning("AI Screening {ScreeningId} not found", request.ScreeningId);
            return Result<CompleteAiScreeningResponse>.NotFound($"AI Screening {request.ScreeningId} not found");
        }

        // Check if already processed
        if (screening.ProcessedAt.HasValue)
        {
            _logger.LogWarning("AI Screening {ScreeningId} already processed at {ProcessedAt}",
                request.ScreeningId, screening.ProcessedAt);
            return Result<CompleteAiScreeningResponse>.Failure("Screening has already been processed");
        }

        // Mark as processed
        screening.Process(request.RawJsonOutput);
        await _screeningRepository.UpdateAsync(screening, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "AI Screening {ScreeningId} completed for Patient {PatientId}. Status: {Status}",
            screening.Id, screening.PatientId, request.ResultStatus);

        // Send real-time notification to patient [FR-45]
        var notificationTitle = GetNotificationTitle(request.ResultStatus);
        var notificationMessage = GetNotificationMessage(request.ResultStatus);

        var patient = await _patientRepository.GetByIdAsync(screening.PatientId, cancellationToken);
        if (patient is null)
        {
            _logger.LogWarning(
                "Patient profile {PatientId} not found for screening {ScreeningId}; skipping notification.",
                screening.PatientId,
                screening.Id);
        }
        else
        {
            await _notificationService.SendAsync(
                patient.UserId,
                notificationTitle,
                notificationMessage,
                NotificationType.AiScreeningCompleted,
                new { ScreeningId = screening.Id, ResultStatus = request.ResultStatus },
                cancellationToken);

            _logger.LogInformation(
                "Real-time notification sent to User {UserId} for screening {ScreeningId}",
                patient.UserId,
                screening.Id);
        }

        return Result<CompleteAiScreeningResponse>.Success(new CompleteAiScreeningResponse
        {
            ScreeningId = screening.Id,
            ResultStatus = request.ResultStatus,
            ProcessedAt = screening.ProcessedAt!.Value
        });
    }

    private static string GetNotificationTitle(string resultStatus)
    {
        return resultStatus.ToLowerInvariant() switch
        {
            "normal" => "Kết quả sàng lọc AI: Bình thường",
            "abnormal" => "Kết quả sàng lọc AI: Cần lưu ý",
            "requiresreview" => "Kết quả sàng lọc AI: Cần chuyên gia đánh giá",
            _ => "Kết quả sàng lọc AI đã sẵn sàng"
        };
    }

    private static string GetNotificationMessage(string resultStatus)
    {
        return resultStatus.ToLowerInvariant() switch
        {
            "normal" => "Kết quả sàng lọc đáy mắt của bạn hiện đang bình thường. Vui lòng xem chi tiết kết quả trong ứng dụng.",
            "abnormal" => "Kết quả sàng lọc đáy mắt của bạn có một số dấu hiệu cần lưu ý. Bạn nên đặt lịch tư vấn với bác sĩ nhãn khoa.",
            "requiresreview" => "Kết quả sàng lọc đáy mắt của bạn cần được chuyên gia y tế đánh giá. Vui lòng xem chi tiết và đặt lịch tư vấn.",
            _ => "Kết quả sàng lọc đáy mắt AI của bạn đã sẵn sàng. Vui lòng xem chi tiết trong ứng dụng."
        };
    }
}
